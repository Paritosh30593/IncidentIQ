# CRUD Setup Plan — Test Entity

Source spec: `Backend/.claude/_specs/test-entity.md`

## Summary

- **Table Name**: `Test`
- **Entity Name**: `Test` (derived from table name)
- **Type**: New
- **System Versioning**: **No** (confirmed with user — spec did not specify; minimal scope does not warrant temporal tracking)
- **CRUD Coverage**: Only `Get` (by id) is in scope, per explicit note in the spec. `GetAll`, `Create`, `Update`, `Delete` are **out of scope** for this setup and will not be planned or implemented.
- **Seed Data**: Required — seed rows must be added via the EF Core migration (`HasData`), per spec note.
- **API Wiring**: Reuse/modify the existing stub endpoint at `App/IC.WebAPI/Endpoints/Tests/Get.cs` to actually query the `Test` entity instead of echoing the request string.

This repository is currently greenfield for entities — only `IC.Domain.Common.BaseEntity<T>` and unit-of-work plumbing (`IUnitOfWork`, `IDapperUnitOfWork`) exist. There is no prior entity/repository/service example to mirror 1:1, so this plan establishes the first vertical slice, following `Backend/.claude/guidelines/architecture.md` and `Backend/.claude/guidelines/database-standards.md` folder conventions.

## Open Question / Assumption to confirm before execution

- The spec does not state the primary key type for `Test`. `BaseEntity<T>` is generic over `Id`. This plan assumes **`int` (identity, auto-increment)** as the Id type, consistent with a simple lookup table. If a `Guid` or other key type is intended, this should be corrected before implementation. Do not add primary key, created at and updated at entity property, only SampleText field is needed in the entity.

---

## 1. Domain Layer (`IC.Domain`)

**File**: `App/IC.Domain/Entities/Test.cs` (new folder `Entities/`)

```csharp
namespace IC.Domain.Entities
{
    public class Test : BaseEntity<int>
    {
        public string SampleText { get; set; }
    }
}
```

- Inherits `Id` (`int`), `CreatedAt`, `UpdatedAt` from `BaseEntity<T>`.
- `SampleText`: `string`, max length 20 (enforced via EF Core Fluent API in the entity configuration, not data annotations, to keep the Domain layer persistence-agnostic).

---

## 2. Infrastructure Layer (`IC.Infrastructure`)

### 2.1 Entity Configuration

**File**: `App/IC.Infrastructure/Persistence/EntityConfigurations/TestConfiguration.cs`

- Implements `IEntityTypeConfiguration<Test>`.
- `ToTable("Test")`.
- `HasKey(x => x.Id)` with constraint name `PK_Test_Id`.
- `Property(x => x.SampleText).HasMaxLength(20)`.
- Seed data via `.HasData(...)` with 2–3 sample rows (fixed `Id`, `SampleText`, `CreatedAt`, `UpdatedAt` values — seed data must use static values, not `DateTime.UtcNow`, since EF `HasData` snapshots values at migration-build time).
- No system versioning configuration (`ValidFrom`/`ValidTo` omitted per the "No" decision).

### 2.2 DbContext

**File**: `App/IC.Infrastructure/Persistence/DBContext/AppDbContext.cs` (modify existing)

- Add `public DbSet<Test> Tests { get; set; }`.
- Apply `TestConfiguration` either via `OnModelCreating` → `modelBuilder.ApplyConfiguration(new TestConfiguration())`, or `ApplyConfigurationsFromAssembly` if adopted (this is the first configuration being wired up, so `OnModelCreating` must be added to the `DbContext`, which currently has none).

### 2.3 Repository (Query side only — CQRS per database-standards.md)

Since only `Get` is in scope, only the **Query** side of the repository is needed (no `Command` repository, since Create/Update/Delete are out of scope).

**Interface** — `App/IC.Application/RepositoryContracts/Query/Test/ITestQueries.cs`:

```csharp
public interface ITestQueries
{
    Task<Test> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
```

**Implementation** — `App/IC.Infrastructure/Repositories/Query/TestQueries.cs`:

- Constructor-injects `AppDbContext`.
- `GetByIdAsync` uses `_context.Tests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)`.

### 2.4 DI Registration

**File**: `App/IC.Infrastructure/ServiceExtensions.cs` (modify existing `ConfigureInfrastructureServices`)

- Add: `services.AddScoped<ITestQueries, TestQueries>();`

---

## 3. Application Layer (`IC.Application`)

### 3.1 DTO

**File**: `App/IC.Application/DTOs/Test/TestDto.cs` (new folder `DTOs/`)

```csharp
public class TestDto
{
    public int Id { get; set; }
    public string SampleText { get; set; }
}
```

- `CreatedAt`/`UpdatedAt` excluded from the DTO unless the spec requires exposing them (not mentioned — keeping DTO minimal to the business field).

### 3.2 Service

**Interface** — `App/IC.Application/Services/Test/ITestService.cs`:

```csharp
public interface ITestService
{
    Task<TestDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
```

**Implementation** — `App/IC.Application/Services/Test/TestService.cs`:

- Constructor-injects `ITestQueries`.
- `GetByIdAsync` calls `_testQueries.GetByIdAsync(id, cancellationToken)`, maps `Test` → `TestDto` (manual mapping, no AutoMapper currently referenced in the project), returns `null` if not found (API layer translates to 404).

### 3.3 DI Registration

**File**: `App/IC.Application/ServiceExtensions.cs` (modify existing `ConfigureApplicationServices`)

- Add: `services.AddScoped<ITestService, TestService>();`

---

## 4. API Layer (`IC.WebAPI`)

**File**: `App/IC.WebAPI/Endpoints/Tests/Get.cs` (modify existing stub, per spec note)

- Change route/request type from `string` echo to `int id`:
  ```csharp
  [Route("api/tests")]
  public class Get : EndpointBaseAsync
      .WithRequest<int>
      .WithActionResult<TestDto>
  {
      [HttpGet("{id}")]
      public override async Task<ActionResult<TestDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
      {
          TestDto result = await _testService.GetByIdAsync(id, cancellationToken);
          return result is not null ? Ok(result) : NotFound();
      }
  }
  ```
- Constructor-injects `ITestService` (Ardalis endpoint classes support constructor DI).
- Only `GetById` is added — no `GetAll`, `Create`, `Update`, `Delete` endpoints, per spec's CRUD Coverage Standard (`Get: Yes` only).

---

## 5. Migration

- Generate an EF Core migration (e.g. `AddTestEntity`) under `App/IC.Infrastructure/Persistence/Migrations/` capturing:
  - Creation of the `Test` table (`Id`, `SampleText`, `CreatedAt`, `UpdatedAt`).
  - The `PK_Test_Id` primary key constraint.
  - The seed data rows defined via `HasData` in `TestConfiguration`.
- No `ValidFrom`/`ValidTo` columns or temporal table SQL, since system versioning is disabled.
- Migration must be applied to the local/dev database as part of execution, and the generated `Up`/`Down` methods reviewed for correctness (table creation + seed inserts in `Up`, table drop in `Down`).

---

## 6. Final Review Checklist (for execution phase)

- [ ] Entity, DTO, interfaces, and implementations follow naming conventions in `coding-standards.md` (PascalCase classes/methods, `I`-prefixed interfaces, `_camelCase` private fields).
- [ ] `SampleText` max length of 20 enforced in `TestConfiguration`, matching the spec's `Size=20`.
- [ ] Constraint name `PK_Test_Id` matches `database-standards.md` naming convention (no FK/UK/CUK/IDX needed — single field, no relationships).
- [ ] No system versioning columns/config added (per confirmed "No" decision).
- [ ] Only `GetById` is wired up end-to-end (Domain → Infrastructure → Application → API) — confirm no incidental `GetAll`/`Create`/`Update`/`Delete` scaffolding was added.
- [ ] `Get.cs` endpoint correctly returns `404 NotFound` for a missing id and `200 OK` with `TestDto` otherwise.
- [ ] DI registrations added in both `IC.Application/ServiceExtensions.cs` and `IC.Infrastructure/ServiceExtensions.cs`.
- [ ] Migration generated, reviewed, and applied; seed data present and queryable via the new endpoint.
- [ ] Any deviation from this plan or the source spec (e.g., if the assumed `int` Id type turns out to be wrong) is called out explicitly to the user/stakeholders before or immediately after execution, rather than silently resolved.

---

## 7. Deviation Reporting

If, during execution, implementation details diverge from this plan (e.g., the Id type assumption is corrected, additional fields are discovered, or an existing pattern elsewhere in the codebase conflicts with what's proposed here), the deviation and its reason will be explicitly called out to the user in the conversation at the time it occurs, rather than being silently absorbed into the implementation.

---

## Note on Testing

Per `CLAUDE.md` ("DO NOT add test cases for now"), this plan intentionally excludes Unit Test and Integration Test planning sections that the `/crud-setup` command would otherwise require.
