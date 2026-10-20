# Test Entity — Frontend CRUD Setup Plan

Source spec: `../../../Backend/.claude/_specs/test-entity.md`

## Backend verification (Step 2)

Confirmed implemented and wired (all files present under `Backend/App/`, currently untracked/uncommitted on this branch):

| Layer | File | Notes |
|---|---|---|
| Entity | `IC.Domain/Entities/Test.cs` | `SampleText: string`. Keyless (`HasNoKey()`), matches spec (no PK requested). |
| EF config | `IC.Infrastructure/Persistence/EntityConfigurations/TestConfiguration.cs` | Maps to table `Test`, `SampleText` max length 20. |
| DTO | `IC.Application/DTOs/Test/TestDto.cs` | `SampleText: string` — 1:1 with entity. |
| Query contract/impl | `RepositoryContracts/Query/Test/ITestQueries.cs`, `Infrastructure/Repositories/Query/TestQueries.cs` | `GetAsync` → `FirstOrDefaultAsync` (single row, no filter/id — matches "Get: Yes" only, no list). |
| Service | `IC.Application/Services/Test/TestService.cs` + `ITestService.cs` | Maps entity → `TestDto`, returns `null` if none found. |
| Endpoint | `IC.WebAPI/Endpoints/Tests/Get.cs` | `GET /api/tests`, no route param. Returns `200 TestDto` or `404`. |
| DI | `IC.Application/ServiceExtensions.cs`, `IC.Infrastructure/ServiceExtensions.cs` | `ITestService`/`ITestQueries` both registered. |
| Migration | `Migrations/20261020043758_AddTestEntity.cs` | Creates `Test` table and seeds one row: `SampleText = 'Sample Text A'` — satisfies spec's seed-data requirement. |

No `PropertyNamingPolicy`/`JsonNamingPolicy` override found anywhere in `Backend/`, so ASP.NET Core's default `System.Text.Json` camelCase policy applies → the wire payload is `{ "sampleText": "Sample Text A" }`. **Verify this in the network tab during implementation** since it's inferred, not configured explicitly.

Coverage matches spec exactly: **Get only**, no Create/Update/Delete. The plan below only covers Get.

## Frontend plan

Feature folder: `src/features/test/` (new), following the `feature1` scaffold shape in [Architecture Guidelines](../guidelines/architecture.md).

### 1. Entity type — `src/features/test/ITest.ts`

```ts
export interface ITest {
  sampleText: string;
}
```

1:1 with `TestDto`.

### 2. API service call — `src/features/test/api.ts`

```ts
import { httpClient } from '../../lib/api/httpClient';
import type { ITest } from './ITest';

export const getTest = async (): Promise<ITest> => {
  const { data } = await httpClient.get<ITest>('/tests');
  return data;
};
```

Uses the already-implemented shared `httpClient` (`src/lib/api/httpClient.ts`, baseURL from `VITE_API_BASE_URL` = `https://localhost:7117/api`). Named `getTest` rather than `getTestAll` — the endpoint returns a single object (`FirstOrDefaultAsync`), not a collection, so the `...All` suffix from the naming convention doesn't apply. A `404` response rejects the promise; let TanStack Query surface that as query error state rather than catching it here.

### 3. State management — TanStack Query

**Pre-requisite (app-wide infra, not feature-specific):** `src/providers/query-provider.tsx` is still an empty placeholder and no `QueryClientProvider` is mounted anywhere (`src/main.tsx` renders `<App />` directly). This needs to exist before any TanStack Query hook will work:

`src/providers/query-provider.tsx`:
```tsx
import type { ReactNode } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const queryClient = new QueryClient();

export const QueryProvider = ({ children }: { children: ReactNode }) => (
  <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
);
```

`src/main.tsx` — wrap `<App />` in `<QueryProvider>`.

Hook — `src/features/test/hooks/useTestGet.ts`:
```ts
import { useQuery } from '@tanstack/react-query';
import { getTest } from '../api';

export const useTestGet = () =>
  useQuery({
    queryKey: ['test'],
    queryFn: getTest,
  });
```

### 4. Consumption point

`src/App.tsx` currently has a scratch `useEffect` that calls `httpClient.get('/test')` directly and `console.log`s the response — this predates the feature wiring and hits the wrong path (`/test`, not `/tests`). Replace it with `useTestGet()` and render `sampleText`, e.g.:

```tsx
const { data, isLoading, error } = useTestGet();
```

Leaving `src/pages/Home.tsx` / routing untouched — it's still an unrelated empty placeholder and wiring it up is outside this entity's scope.

### 5. Tests

CLAUDE.md states: **"DO NOT add test cases for now"**, which overrides the command's instruction to generate unit/integration tests. No test files will be generated for this feature. (See [Unit Testing Guidelines](../guidelines/unit-testing.md) / [Integration Testing Guidelines](../guidelines/integration-testing.md) for the intended future direction, not applied here.)

## Generation checklist (Step 3)

- [ ] `src/features/test/ITest.ts`
- [ ] `src/features/test/api.ts`
- [ ] `src/features/test/hooks/useTestGet.ts`
- [ ] Implement `src/providers/query-provider.tsx`
- [ ] Wire `QueryProvider` into `src/main.tsx`
- [ ] Update `src/App.tsx` to consume `useTestGet` instead of the scratch `useEffect`
- [ ] `npm run lint`
- [ ] `npm run build`

## Review / finalization (Step 4)

- [ ] Start backend (`dotnet run` in `Backend/App/IC.WebAPI`) and confirm `GET https://localhost:7117/api/tests` returns `{ "sampleText": "Sample Text A" }` (or whatever casing the server actually emits — adjust `ITest`/`api.ts` if it's PascalCase instead).
- [ ] Run `npm run dev`, load the app, confirm the seeded value renders without console errors.
- [ ] Confirm no Create/Update/Delete code was added — spec covers Get only.

## Deviations from spec/command

- Skipped unit/integration test generation per CLAUDE.md's explicit override ("DO NOT add test cases for now"), even though the command instructions ask for them.
- `query-provider.tsx` wiring and its mount in `main.tsx` are new app-wide infrastructure additions, not strictly part of the `Test` entity itself, but required for the TanStack Query hook to function — flagged here rather than silently expanding scope.
- API/hook naming (`getTest` / `useTestGet`, no `All`/`ById` suffix) deviates from the example convention in the architecture guidelines since the backend endpoint takes no parameters and returns a single record.
