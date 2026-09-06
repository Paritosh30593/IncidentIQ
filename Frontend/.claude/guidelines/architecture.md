# Frontend Architecture Guidelines

## Status

Early scaffolding. Treat anything below as intended direction, not an established pattern to imitate blindly — verify a file has real content before copying its shape.

## Structure

- `src/pages/<PageName>/` — route-level components. Each folder contains `index.tsx` (the page) and optionally `<PageName>.module.css`. Pages load asynchronously based on their folder under `src/pages`, for routing and bundle splitting.
- `src/components/common/<ComponentName>/` — shared common components used across pages (Navbar, Footer, Sidebar, etc.) — **only** this category goes here.
- `src/components/ui/<ComponentName>/` — reusable, generic UI primitives (buttons, inputs, modals, etc.), imported from `shadcn` — **only** this category goes here.
  - Both component folders contain `index.ts` (re-exports the component), `<ComponentName>.tsx`, and optionally `<ComponentName>.module.css`. Keep the folder flat — avoid deep nesting for small components.
- `src/features/<feature>/` — per-feature grouping. Each feature owns its `api.ts` (raw calls), an `I<Feature>.ts` interface file (types), and a `hooks/` folder for TanStack Query hooks wrapping those calls. Add new features as siblings of `src/features/feature1/`.
- `src/providers/` — app-wide context providers, one file per concern (auth, query client, etc.).
- `src/lib/api/httpClient.ts` — shared axios instance that feature `api.ts` modules should use rather than calling `axios`/`fetch` directly.
- `src/lib/utils.ts` — shared, cross-feature utility helpers only. Feature-specific helpers stay inside the feature folder.
- `src/config/` — app configuration (e.g. MSAL).

## Principles

- Keep feature code self-contained under `src/features/<feature>/`; avoid cross-feature imports where a shared abstraction in `src/lib/` would do instead.
- Route-level concerns (data fetching orchestration, layout) belong in `src/pages/`; presentational/reusable pieces belong in `src/components/` or the owning feature.
- This app talks to the `Backend/` .NET API — see `../Backend/.claude/guidelines/architecture.md` for the server-side shape.

See [Coding Standards](./coding-standards.md) for the tech choices (axios, TanStack Query, Redux Toolkit) and code style used within this structure.
