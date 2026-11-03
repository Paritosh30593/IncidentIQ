# Database Standards

## Design Principles

- Use Entity Framework Core conventions
- Keep migration files versioned and documented
- The Scripts folder contains raw SQL scripts for views, functions, procedures, and triggers. Ensure:
  - Each script follows the naming convention: `<ObjectName>_<ObjectType>.sql`
  - Include script in migration files when applicable (create or alter) to ensure database consistency
  - Scripts are embedded resources (`Persistence/Scripts/**/*.sql`) and applied from a migration with `migrationBuilder.ApplySqlScript("<Folder>/<File>.sql")`; every script change needs a **new** migration (never edit applied migrations)
  - Scripts must be re-runnable (`CREATE OR ALTER`, `MERGE`) and must not contain `GO`
  - Migrations are applied by the CD pipeline with `dotnet ef database update`; test them against a local DB first. See [docs/database-migrations.md](../../../docs/database-migrations.md)

## Data Access Technology (CQRS)

- **Queries** (`Repositories/Query/*`) always use **Dapper**, via `IDapperUnitOfWork`/`DapperContext` (`Connection`/`Transaction`). Reads are plain parameterized SQL — no LINQ-to-entities.
- **Commands** (`Repositories/Command/*`) always use **EF Core**, via `AppDbContext`/`IUnitOfWork`. Writes go through tracked entities and change-tracking so EF enforces the configured constraints/behaviors.
- Never mix the two within the same repository class.

## Folder structure under `IC.Infrastructure` (CQRS pattern):

```plaintext
IC.Infrastructure
├── Persistence
│   ├── DbContext/
│   │   └── AppDbContext.cs
│   ├── EntityConfigurations/
│   │   └── ProductConfiguration.cs
│   ├── Functions/
│   │   ├── TopCustomer.cs
│   │   ├── SalesByMonth.cs
│   │   └── FunctionConfiguration.cs
│   ├── Views/
│   │   ├── ProductSales.cs
│   │   ├── CustomerSummary.cs
│   │   └── ViewConfiguration.cs
│   ├── Seed/
│   │   └── ProductData.cs
│   ├── Scripts/ (*.sql)
│   │   ├── Views/ # contains SQL view definitions <ViewName>_View.sql
│   │   ├── Functions/ # contains SQL function definitions <FunctionName>_Function.sql
│   │   ├── Procedures/ # contains SQL procedure definitions <ProcedureName>_Procedure.sql
│   │   ├── Publish / # optional, currently unused (migrations are applied by the pipeline via EF)
│   │   └── Triggers/ # contains SQL trigger definitions <TriggerName>_Trigger.sql
│   └── Migrations/
└── Repositories/
    ├── Query/
    │   └── ProductQueries.cs
    └── Command/
        └── ProductCommands.cs
```

Explanation:

- `Persistence/` contains all database-related configurations, including the DbContext, entity configurations, functions, views, seed data, raw SQL scripts, and migrations.
- `Repositories/` contains query and command repositories for accessing and manipulating data.

## Folder structure under `IC.Application` (CQRS pattern):

```plaintext
IC.Application
├── DTOs/
│   └── ProductDto.cs
├── Services/
│   ├── Product/
│   │   ├── IProductService.cs
│   │   └── ProductService.cs
└── RepositoryContracts/
    ├── Query/
    │   └── IProductQueries.cs
    └── Command/
        └── IProductCommands.cs
```

Explanation:

- `DTOs/` contains data transfer objects for the application layer.
- `Services/` contains service interfaces and implementations for business logic.
- `RepositoryContracts/` contains query and command repository interfaces for accessing and manipulating data.

## Constraint Naming Conventions

Commonly used terms and their constraint naming conventions, referenced when defining or updating entities via the `/crud-setup` and `/crud-update` commands:

| Term                        | Definition                    | Constraints Naming Convention                        |
| --------------------------- | ----------------------------- | ---------------------------------------------------- |
| PK                          | Primary Key                   | `PK_TableName_ColumnName`                            |
| UUID                        | Universally Unique Identifier | `UUID_TableName_ColumnName`                          |
| UK                          | Unique Key                    | `UK_TableName_ColumnName`                            |
| FK (TableName, ColumnName)  | Foreign Key                   | `FK_TableName_ColumnName_RefTableName_RefColumnName` |
| CUK (params1, params2, ...) | Composite Unique Key          | `CUK_TableName_ColumnName1_ColumnName2_...`          |
| IDX                         | Index Constraint              | `IDX_TableName_ColumnName`                           |

## Audit Columns

- `BaseEntity<T>` (`IC.Domain.Common`) provides only `Id`. Audit timestamp columns are opt-in per entity via `IHasCreatedAt` and/or `IHasUpdatedAt` — implement whichever the table actually needs (both, one, or neither), rather than inheriting them unconditionally.

## System Versioning

- Entities may opt into system versioning (SQL Server temporal tables) during CRUD setup via the `Versioning` argument (`Yes`/`No`).
- Enable via EF Core's native `IsTemporal()` in the entity's `IEntityTypeConfiguration`, naming the period columns `ValidFrom`/`ValidTo`:
  ```csharp
  builder.ToTable("Product", b => b.IsTemporal(t =>
  {
      t.HasPeriodStart("ValidFrom");
      t.HasPeriodEnd("ValidTo");
  }));
  ```
- `ValidFrom`/`ValidTo` must stay EF shadow properties — SQL Server and EF Core manage them automatically on every insert/update/delete. **Do not** also declare them as regular CLR properties on the entity; EF Core rejects that mapping (`Period property 'X' must be a shadow property`).
- This gives automatic, DB-enforced history of every row version (including deleted rows) with zero application code, queryable via `.TemporalAsOf()` / `.TemporalAll()`. It does **not** track *who* made a change — pair it with explicit actor columns (e.g. `AssignedBy` on `UserRole`) when accountability requires knowing who, not just when.
