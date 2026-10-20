# Plan: EF Core migrations to Azure SQL via GitHub Actions

Status: DRAFT – awaiting review. No code or workflow has been changed yet.

## 1. Current state (findings)

- EF Core 10.0.11 (`dotnet-ef` 10.0.11 pinned in `dotnet-tools.json`). Migrations live in `IC.Infrastructure/Persistence/Migrations` (one migration so far: `AddTestEntity`, which already uses `migrationBuilder.Sql(...)` inline).
- `database-standards.md` already defines `Persistence/Scripts/{Views,Functions,Procedures,Triggers,Publish}` with the `<ObjectName>_<ObjectType>.sql` naming. `Scripts/` and `Publish/` do not exist yet.
- `AppDbContext` has no design-time factory and no `Database.Migrate()` at startup (good – keep it that way).
- `CI-Dev.yml`: builds and tests the backend and frontend. No migration checks.
- `CD-Dev.yml`: builds and pushes images, then `az containerapp update`. No DB step. It logs in with GitHub OIDC using `vars.AZURE_CLIENT_ID`, and the **same** client ID is the Container App's runtime SQL identity (`AzureSql__UseManagedIdentity=true`).
- `IC.Infrastructure.csproj` has no `.sql` file handling.

## 2. Decisions (answers to the open questions)

### Q3 – Do we need another managed identity? Recommended: yes, a second one.

Today one identity is used for two jobs: (a) the pipeline login and (b) the running app. Whatever DB permissions it has, both get.

- **Migration identity** (new, used by GitHub Actions only): needs DDL (`db_ddladmin`) plus `db_datareader`/`db_datawriter` (seed data, `MERGE`) and, if procs or views are granted, `ALTER`/`CONTROL` on the schema. Federated credential for the GitHub repo and environment.
- **App identity** (existing): reduce to `db_datareader`, `db_datawriter` and `EXECUTE`. A compromised API can then no longer drop or alter schema.
- Why: least privilege and separation of duties. It also lets the prod approval gate protect the DDL identity, because the federated credential is scoped to the GitHub _environment_.
- Acceptable shortcut for Dev only: keep one identity that has `db_owner`. This works but is not best practice. The plan implements the two-identity setup, and the workflow reads the identity from a separate variable so either option works.

Manual one-time setup (outside the repo; steps documented in a new `docs/database-migrations.md`):

1. Create user-assigned identity `id-incidentiq-dev-migrator`.
2. Add a federated credential: repo `Paritosh30593/IncidentIQ`, entity type Environment, name `dev`.
3. Run in the target DB (as an Entra admin):
   `CREATE USER [id-incidentiq-dev-migrator] FROM EXTERNAL PROVIDER; ALTER ROLE db_ddladmin ADD MEMBER ...; ALTER ROLE db_datareader ...; ALTER ROLE db_datawriter ...;` (plus `GRANT EXECUTE`/`ALTER` on schema `dbo` if required by views/procs).
4. Downgrade the app identity to datareader, datawriter and `GRANT EXECUTE`.
5. Add GitHub _environment variable_ `AZURE_MIGRATION_CLIENT_ID` in environment `dev`.
6. SQL firewall: the runner must reach the server. Default option: a temporary firewall rule for the runner IP, added and removed by the workflow (needs `Contributor`-level rights on the SQL server, scoped to that resource). Alternatives: self-hosted runner or private endpoint.

### Q4 – Script or normal EF migration? EF migrations are the source of truth; a script is generated from them.

- Developers run `dotnet ef migrations add`. The EF migration and `__EFMigrationsHistory` remain authoritative.
- CI generates `migrate.sql` with `dotnet ef migrations script --idempotent`. This is the artifact that is reviewed, tested and promoted.
- Local testing: developers can run `dotnet ef database update` against local SQL (docker-compose), or run the generated script.
- No hand-written `migrate_<datetime>.sql` files. They would bypass the history table and the model snapshot.
- Delivery mechanism: the **idempotent script** (reviewable, no extra runtime). A bundle is the alternative; the script is chosen so the SQL that will run is visible in the PR and the workflow summary.
- `Scripts/Publish/` (from database-standards.md): used only for an optional committed copy of the latest generated script. Decision needed (see Open questions).

### Q5 – Run migrations only when needed.

Handled by the idempotent script and `__EFMigrationsHistory`: re-running with nothing pending is a no-op. Additional cheap guards:

- Path filter on the CD migrate job: skip when no `Persistence/Migrations/**`, `Persistence/Scripts/**` or the persistence project changed (implemented with `dorny/paths-filter` or an `git diff` step).
- PR drift check: `dotnet ef migrations has-pending-model-changes` fails CI if entities changed without a migration.

### Q2 – Mixing C# migrations with `.sql` files

Convention (matches database-standards.md):

- SQL lives in `Persistence/Scripts/{Views|Functions|Procedures|Triggers}/<Name>_<Type>.sql`, written as `CREATE OR ALTER ...` so re-applying is safe.
- Files are **embedded resources** (`<EmbeddedResource Include="Persistence\Scripts\**\*.sql" />` in `IC.Infrastructure.csproj`), so they ship inside the assembly and the migration does not depend on the file system or working directory.
- Small helper `Persistence/Scripts/SqlScriptLoader.cs` (or a `MigrationBuilder` extension `ApplySqlScript("Views/OpenIncidents_View.sql")`) reads the embedded resource and calls `migrationBuilder.Sql(...)`. Since `CREATE VIEW/PROC/FUNCTION/TRIGGER` must be the only statement in a batch, the helper wraps the content in `EXEC(N'...')` (escaping `'`) unless the file has no batch-sensitive statement. Files must not contain `GO`.
- Whenever a `.sql` file is created or altered, a **new migration** is added that applies it (`dotnet ef migrations add Alter_OpenIncidents_View`, then `Up()` calls the helper). `Down()` re-applies the previous version, or drops the object for new ones. Old migrations are never edited, because their embedded file would change under them. For that reason, when a script changes, the previous version is kept as a versioned copy, or `Down()` is documented as roll-forward only (decision needed).
- Deletions: new migration with `DROP VIEW/PROC/FUNCTION IF EXISTS`. The `.sql` file is removed from the repo in the same change, after earlier migrations stop referencing it (see the versioning decision below).
- Seed data: idempotent `MERGE` in a `Scripts/Seed/` file, or `HasData` for small static data.
- Guard in CI: a lightweight check script that fails the PR if any `Persistence/Scripts/**/*.sql` changed without a new file under `Persistence/Migrations/`.

## 3. Implementation steps

1. **Design-time factory** (`IC.Infrastructure/Persistence/DbContext/AppDbContextFactory.cs`, implementing `IDesignTimeDbContextFactory<AppDbContext>`). It reads the connection string from an env var and falls back to local SQL, so `dotnet ef` works in CI without starting the WebAPI or needing secrets.
2. **csproj**: add the embedded-resource glob for `.sql`.
3. **Scripts structure**: create `Persistence/Scripts/{Views,Functions,Procedures,Triggers,Seed}` (with `.gitkeep`) and `SqlScriptLoader` helper.
4. **CI-Dev.yml** (backend job, after build):
   - `dotnet tool restore`.
   - `dotnet ef migrations has-pending-model-changes` (drift check).
   - `dotnet ef migrations script --idempotent --no-build -o migrate.sql`.
   - SQL-change-without-migration guard.
   - Apply `migrate.sql` to a throwaway SQL Server 2022 service container (`services: mssql`), run twice to prove idempotency.
   - Upload `migrate.sql` as an artifact and print a summary.
5. **CD-Dev.yml**: new job `migrate-database`, placed after the image push and **before** `deploy-backend` (`deploy-backend` gets `needs: [backend, migrate-database]`):
   - `environment: dev`, `permissions: id-token: write, contents: read`.
   - Path-filter step to skip when nothing relevant changed (the job still reports success so `deploy-backend` is not blocked).
   - Checkout, setup .NET, `dotnet tool restore`, generate the idempotent script from the same commit (or download the CI artifact when available).
   - `azure/login@v2` with `vars.AZURE_MIGRATION_CLIENT_ID`.
   - Add the runner IP to the SQL firewall, run the script (`azure/sql-action@v2` with `Authentication=Active Directory Default`, or `sqlcmd -G`), and remove the firewall rule in an `if: always()` step.
   - Write `migrate.sql` to the job summary and attach it as an artifact.
   - Concurrency group `db-migrate-dev`, `cancel-in-progress: false`, so two migrations never run at once.
6. **Docs**: add `docs/database-migrations.md` (setup steps from Q3, developer workflow for code and `.sql` migrations, rollback approach) and a short update to `database-standards.md` (embedded-resource rule, "one new migration per script change", and the role of `Publish/`).
7. **Validation**: run the drift check and script generation locally, apply the script twice to a local SQL container, and verify CI YAML with `actionlint` if available.

## 4. Safety and rollback

- Backward-compatible (expand/contract) changes only, because the old container revision runs briefly against the new schema while the app deploys.
- Rollback = roll-forward migration, or Azure SQL point-in-time restore. `Down()` is not used in pipelines.
- Migration runs before the app deploy, and a failure stops the deploy.
- When prod is added: reuse the same job in a CD-Prod workflow with a required-reviewer environment.

## 5. Open questions for review

1. **Second identity**: create the dedicated migration identity (recommended), or keep the single identity for Dev? Already created second in Azure "pd-az-sql-migrate-westus-02"
2. **Script versioning**: when a view/proc changes, should the old version be kept as a versioned file (for example `OpenIncidents_View.v2.sql`) so old migrations still compile and replay, or should we accept that migrations always embed the _latest_ file (simpler; safe because scripts are idempotent `CREATE OR ALTER`)? The plan defaults to the simpler option.
3. **`Scripts/Publish/`**: commit the latest generated `migrate.sql` there, or keep it as a pipeline artifact only (the plan default)? Since every migration can be tested directly in dev environment, kindly ingore extra step of generating migrate.sql if not necessary.
4. **SQL network access**: temporary runner-IP firewall rule (the plan default), or a self-hosted runner? Whichever is best suitable
5. **Local test DB**: use the SQL container from `docker-compose.yml` for local runs, or Azure SQL dev? No need for containerized SQL in local, also refer the snapshot in chat for more details
