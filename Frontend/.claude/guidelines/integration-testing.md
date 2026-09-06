# Frontend Integration Testing Guidelines

## Status

No test runner or integration/e2e tooling is configured yet. Do not assume any integration tests can be run until this is set up.

## Intended direction

- For end-to-end flows against the running app (Vite dev server, or the Dockerized build behind nginx), prefer **Playwright**.
- Integration tests should exercise real user flows across `src/pages/`, hitting the actual `Backend/` API (or a lightweight mock server) via `src/lib/api/httpClient.ts` — not a mocked React tree.
- Once the MSAL provider (`src/providers/msal-provider.tsx`) is implemented, integration tests will need an auth bypass or test-account flow; document that setup here when it exists.
- The root `docker-compose.yml` is the reference for how Frontend + Backend run together locally; integration tests should target that composed environment rather than assuming services are already up.

Update this file with real conventions (test locations, fixtures, CI wiring) once integration tooling is actually installed and in use.
