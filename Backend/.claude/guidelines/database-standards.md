# Database Standards

## Design Principles

- Use Entity Framework Core conventions
- Keep migration files versioned and documented
- The Scripts folder contains raw SQL scripts for views, functions, procedures, and triggers. Ensure:
  - Each script follows the naming convention: `<ObjectName>_<ObjectType>.sql`
  - Include script in migration files when applicable (create or alter) to ensure database consistency

## Persistence Folder Structure

Folder structure under `IC.Infrastructure` (CQRS pattern):

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
│   │   └── Triggers/ # contains SQL trigger definitions <TriggerName>_Trigger.sql
│   └── Migrations/
└── Queries/
    └── ProductQueries.cs
```

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

## System Versioning

- Entities may opt into system versioning (temporal tables) during CRUD setup via the `Versioning` argument (`Yes`/`No`).
- Versioned tables carry `ValidFrom` and `ValidTo` columns to track the record's valid date range:
  ```csharp
  public DateTime ValidFrom { get; set; } // Table versioning: record valid from date
  public DateTime ValidTo { get; set; } // Table versioning: record valid to date
  ```
