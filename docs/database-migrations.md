# Database migrations (EF Core + Azure SQL + GitHub Actions)

## How it works

1. Developers change entities and/or `.sql` files and add an **EF Core migration** (source of truth, tracked in `__EFMigrationsHistory`).
2. **CI** (`CI-Dev.yml`) fails the build if the model changed without a migration, or if a `Scripts/**/*.sql` file changed without a new migration.
3. **CD** (`CD-Dev.yml`, job `migrate-database`) runs `dotnet ef database update` against the live Azure SQL database **before** the backend is deployed, using the dedicated migration identity. EF applies only pending migrations (tracked in `__EFMigrationsHistory`), so it is a no-op when up to date; the job is also skipped when no migration-related path changed.

The app never migrates itself (no `Database.Migrate()` at startup).

## Developer workflow

### Schema changes (entities)
```
dotnet ef migrations add <Name> --project App/IC.Infrastructure --startup-project App/IC.WebAPI
```

### Views / functions / procedures / triggers / seed data (`.sql`)
1. Add or edit `App/IC.Infrastructure/Persistence/Scripts/<Type>/<ObjectName>_<ObjectType>.sql`.
   - Use `CREATE OR ALTER ...` (views, functions, procedures, triggers) or `MERGE` / `IF NOT EXISTS` (seed data), so re-runs are safe.
   - Do not use `GO`.
2. Add a **new** migration that applies it (never edit old migrations):
   ```
   dotnet ef migrations add Alter_OpenIncidents_View --project App/IC.Infrastructure --startup-project App/IC.WebAPI
   ```
   ```csharp
   protected override void Up(MigrationBuilder migrationBuilder)
   {
       migrationBuilder.ApplySqlScript("Views/OpenIncidents_View.sql");
   }
   // Seed scripts: migrationBuilder.ApplySqlScript("Seed/Statuses_Seed.sql", wrapInExec: false);
   ```
   `using IC.Infrastructure.Persistence.Scripts;` provides `ApplySqlScript`.
3. Deleting an object: add a migration with `migrationBuilder.Sql("DROP VIEW IF EXISTS ...")` and remove the `.sql` file only after no migration references it.

CI fails if a `Scripts/**/*.sql` file changes without a new migration, or if the model changed without a migration.

### Test locally (before pushing)
- `docker compose up -d mssql`, then
  `$env:ConnectionStrings__IC="Server=localhost;Database=IncidentIQ;User Id=sa;Password=YourStrong@Passw0rd!;TrustServerCertificate=True"` and
  `dotnet ef database update --project App/IC.Infrastructure --startup-project App/IC.WebAPI`.

## One-time Azure / GitHub setup

Identities:
- **Migration identity** `pd-az-sql-migrate-westus-02` (GitHub Actions only).
- **App identity** (existing, used by the Container App).

1. **Federated credential** on the migration identity: Managed Identity > Federated credentials > GitHub Actions > organization `Paritosh30593`, repository `IncidentIQ`, entity type **Branch**, branch `main` (the same kind of credential the app/deploy identity uses).
2. **Azure SQL permissions.** Connect to the `incident-iq` database as an Entra admin:
   ```sql
   -- migration identity: schema changes and seed data
   CREATE USER [pd-az-sql-migrate-westus-02] FROM EXTERNAL PROVIDER;
   ALTER ROLE db_ddladmin ADD MEMBER [pd-az-sql-migrate-westus-02];
   ALTER ROLE db_datareader ADD MEMBER [pd-az-sql-migrate-westus-02];
   ALTER ROLE db_datawriter ADD MEMBER [pd-az-sql-migrate-westus-02];
   -- only if migrations create objects that need it:
   -- GRANT ALTER, EXECUTE ON SCHEMA::dbo TO [pd-az-sql-migrate-westus-02];

   -- app identity: remove DDL/owner rights and keep data access only
   -- ALTER ROLE db_owner DROP MEMBER [pd-personal-projects-westus-02];   -- if currently a member
   ALTER ROLE db_datareader ADD MEMBER [pd-personal-projects-westus-02];
   ALTER ROLE db_datawriter ADD MEMBER [pd-personal-projects-westus-02];
   GRANT EXECUTE TO [pd-personal-projects-westus-02];
   ```
3. **Azure RBAC for the firewall rule**: grant the migration identity a role that can create/delete SQL server firewall rules (for example `SQL Security Manager`) scoped to the SQL server `pd-practice` only.
4. **GitHub**: set the repository variable `AZURE_MIGRATION_CLIENT_ID` to the migration identity's client ID. Optional `AZURE_SQL_RESOURCE_GROUP` if SQL is not in the same resource group as `ACR_RESOURCE_GROUP`.

## Safety and rollback
- Keep changes backward compatible (expand/contract): the previous container revision runs briefly against the new schema.
- Roll forward with a new migration; for disasters use Azure SQL point-in-time restore. `Down()` is not run by pipelines.
- Migrations run serialized (`concurrency: db-migrate`) and a failure stops the app deployment.

