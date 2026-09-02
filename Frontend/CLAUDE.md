# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Status

This is early scaffolding, not a working application yet. `App.tsx` is still the unmodified Vite starter template, and several directories under `src/` exist only as empty placeholder files staking out an intended structure (see "Scaffolded but empty" below). Don't assume these files contain working code — check before building on them.

This `Frontend/` directory is one part of the larger `IncidentIQ` repo, which also contains a `Backend/` (.NET 10 Clean Architecture API — see `../Backend/.claude/guidelines/architecture.md`) and a root `docker-compose.yml` for local orchestration.

## Commands

Run from this `Frontend/` directory:

- `npm run dev` — start the Vite dev server with HMR
- `npm run build` — type-check via project references (`tsc -b`) then production-build with Vite
- `npm run lint` — run ESLint over the whole project
- `npm run preview` — serve the production build locally

There is no test runner configured yet (no test script, no Vitest/Jest dependency).

## Toolchain notes

- **Vite + Rolldown**: `vite.config.ts` uses `@vitejs/plugin-react` together with `@rolldown/plugin-babel` running the `reactCompilerPreset()` — the React Compiler is active, so avoid manual `useMemo`/`useCallback`/`React.memo` micro-optimizations; let the compiler handle it.
- **TypeScript project references**: `tsconfig.json` has no compiler options itself and just references `tsconfig.app.json` (for `src/`, DOM lib, bundler resolution) and `tsconfig.node.json` (for `vite.config.ts`, Node lib). Both enable `verbatimModuleSyntax`, `noUnusedLocals`/`noUnusedParameters`, and `erasableSyntaxOnly` — imports must use explicit `import type` where applicable, and unused locals/params fail the build via `tsc -b`.
- **Docker/nginx**: `Dockerfile` and `nginx.conf` exist for containerized production serving (SPA fallback to `index.html`), but `Dockerfile` is currently empty.

## Structure

- `src/main.tsx` mounts `App` (`src/App.tsx`) into `#root`.
- `src/pages/` — route-level components (currently just `Home.tsx`, empty).
- `src/features/<feature>/` — the intended per-feature grouping, e.g. `src/features/feature1/` pairs an `api.ts` with an `IFeature1.ts` interface file. Follow this pattern (feature folder with its own API module and type/interface file) when adding new features.
- `src/providers/` — app-wide context providers, split one-per-concern (`msal-provider.tsx` for Azure AD auth via MSAL, `query-provider.tsx` for TanStack Query). Neither `@azure/msal-*` nor `@tanstack/react-query` are in `package.json` yet — these providers are placeholders for planned dependencies, not yet installed.
- `src/config/authConfig.ts` — intended home for MSAL configuration (client ID, authority, redirect URIs).
- `src/lib/api/httpClient.ts` — intended shared HTTP client that feature `api.ts` modules would use.
- `src/lib/utils.ts` — intended shared utility helpers.

### Scaffolded but empty

These files exist (0 bytes) as placeholders for the structure above and have no implementation yet: `src/config/authConfig.ts`, `src/features/feature1/api.ts`, `src/features/feature1/IFeature1.ts`, `src/lib/api/httpClient.ts`, `src/lib/utils.ts`, `src/pages/Home.tsx`, `src/providers/msal-provider.tsx`, `src/providers/query-provider.tsx`, plus the top-level `Dockerfile`, `.env`, and `.env.local`.
