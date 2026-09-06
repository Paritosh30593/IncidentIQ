# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Status

This is early scaffolding, not a working application yet. `App.tsx` is still the unmodified Vite starter template, and several directories under `src/` exist only as empty placeholder files staking out an intended structure (see "Scaffolded but empty" below). Don't assume these files contain working code — check before building on them.

This `Frontend/` directory is one part of the larger `IncidentIQ` repo, which also contains a `Backend/` (.NET 10 Clean Architecture API — see `../Backend/.claude/guidelines/architecture.md`) and a root `docker-compose.yml` for local orchestration.

## Guidelines

The following guidelines provide standards and conventions for development in this project — check them before adding structure or picking a pattern, since several codify intended direction (e.g. tech not yet installed) rather than what's already in use:

- [Architecture Guidelines](./.claude/guidelines/architecture.md) — folder structure (`pages`, `components`, `features`, `providers`, `lib`) and where new code goes
- [Coding Standards](./.claude/guidelines/coding-standards.md) — TypeScript/React conventions and the intended tech stack (axios, TanStack Query, Redux Toolkit, shadcn)
- [Unit Testing Guidelines](./.claude/guidelines/unit-testing.md)
- [Integration Testing Guidelines](./.claude/guidelines/integration-testing.md)

## Commands

Run from this `Frontend/` directory:

- `npm run dev` — start the Vite dev server with HMR
- `npm run build` — type-check via project references (`tsc -b`) then production-build with Vite
- `npm run lint` — run ESLint over the whole project
- `npm run preview` — serve the production build locally

There is no test runner configured yet — see [Unit Testing Guidelines](./.claude/guidelines/unit-testing.md) and [Integration Testing Guidelines](./.claude/guidelines/integration-testing.md) for intended direction.

## Toolchain notes

- **Vite + Rolldown**: `vite.config.ts` uses `@vitejs/plugin-react` together with `@rolldown/plugin-babel` running the `reactCompilerPreset()` — the React Compiler is active (see [Coding Standards](./.claude/guidelines/coding-standards.md) for what that means for your code).
- **TypeScript project references**: `tsconfig.json` has no compiler options itself and just references `tsconfig.app.json` (for `src/`, DOM lib, bundler resolution) and `tsconfig.node.json` (for `vite.config.ts`, Node lib) — see [Coding Standards](./.claude/guidelines/coding-standards.md) for the strictness flags this enables and what they require of your code.
- **Docker/nginx**: `Dockerfile` and `nginx.conf` exist for containerized production serving (SPA fallback to `index.html`), but `Dockerfile` is currently empty.

## Structure

`src/main.tsx` mounts `App` (`src/App.tsx`) into `#root`. See [Architecture Guidelines](./.claude/guidelines/architecture.md) for the full intended folder layout (`pages`, `components`, `features`, `providers`, `lib`, `config`).

None of `@azure/msal-*`, `@tanstack/react-query`, `axios`, `@reduxjs/toolkit`, or `shadcn` are in `package.json` yet — the providers, `httpClient`, and the tech-stack choices in the coding standards are all placeholders for planned dependencies, not yet installed.

### Scaffolded but empty

These files exist (0 bytes) as placeholders for the structure above and have no implementation yet: `src/config/authConfig.ts`, `src/features/feature1/api.ts`, `src/features/feature1/IFeature1.ts`, `src/lib/api/httpClient.ts`, `src/lib/utils.ts`, `src/pages/Home.tsx`, `src/providers/msal-provider.tsx`, `src/providers/query-provider.tsx`, plus the top-level `Dockerfile`, `.env`, and `.env.local`.
