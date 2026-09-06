# Frontend Unit Testing Guidelines

## Status

No test runner is configured yet — no test script in `package.json`, no Vitest/Jest dependency installed. Do not assume tests can be run until this is set up.

## Intended direction

- When a test runner is added, prefer **Vitest** (pairs naturally with the existing Vite config) over Jest.
- For component tests, pair Vitest with **React Testing Library** — test behavior/output, not implementation details.
- Colocate unit tests next to the code they cover (e.g. `Foo.tsx` + `Foo.test.tsx`) rather than a parallel `__tests__` tree.
- Feature `api.ts` modules should be tested against a mocked `httpClient`, not real network calls.

Update this file with real conventions (file naming, coverage expectations, CI wiring) once a test runner is actually installed and in use.
