# Frontend Architecture Guidelines

## Status

Early scaffolding. Treat anything below as intended direction, not an established pattern to imitate blindly — verify a file has real content before copying its shape.

## Structure

The IncidentIQ frontend follows a layered architecture pattern:

```plaintext
src/
├── pages/          # Page route-level components. Pages load asynchronously for routing and bundle splitting
|   └── <PageName>/
|      ├── index.tsx
|      └── <PageName>.module.css
├── components/      # Shared and UI components
|   ├── common/      # Shared common components used across pages (Navbar, Footer, Sidebar, etc.) — **only** this category goes here.
|   |   └── <ComponentName>/
|   |       ├── index.ts
|   |       ├── <ComponentName>.tsx
|   |       └── <ComponentName>.module.css
|   └── ui/          # Reusable, generic UI primitives (buttons, inputs, modals, etc.) — **only** this category goes here.
|       └── <ComponentName>/
|           ├── index.ts
|           ├── <ComponentName>.tsx
|           └── <ComponentName>.module.css
├── features/                   # Feature-specific modules, each containing API calls, types, and hooks
|   └── <FeatureName>/
|       ├── api.ts              # All API calls for the feature using axios (Endpoint function naming convention: <type><Entity><Action|Params> e.g., getUserAll, getUserByIdAndName, createUser, updateUser, deleteUser, ...). Should use the shared axios instance from `src/lib/api/httpClient.ts`.
|       ├── I<FeatureName>.ts   # Entity Type definitions for the feature (e.g., IUser, IPost corresponds to backend DTO models)
|       ├── components/         # Feature-specific components used only within this feature.
|       |   └── <ComponentName>/
|       |       ├── index.ts
|       |       ├── <ComponentName>.tsx
|       |       └── <ComponentName>.module.css
|       ├── lib/                # (optional) Feature-specific library code (helpers, utilities, etc.)
|       |   └── <HelperName>.ts
|       └── hooks/              # TanStack Query hooks for the feature.
|           └── use<FeatureName><Action>.ts # Hook naming convention: use<Entity><Action|Endpoint Params>.ts e.g., useUserAll, useUserByIdAndName, useCreateUser, useUpdateUser, useDeleteUser, ...
├── providers/              # app-wide context providers, one file per concern (auth, query client, etc.).
├── lib/                    # Shared, cross-feature library code (e.g., API instances, utility helpers)
|   ├── api/                # Shared global API instances
|   |   └── httpClient.ts
|   └── utils.ts            # Shared, cross-feature utility helpers. Feature-specific helpers stay inside the feature folder.
└── config/                 # App configuration (e.g., MSAL)
```

## Principles

- Keep feature code self-contained under `src/features/<feature>/`; avoid cross-feature imports where a shared abstraction in `src/lib/` would do instead.
- Route-level concerns (data fetching orchestration, layout) belong in `src/pages/`; presentational/reusable pieces belong in `src/components/` or the owning feature.
- This app talks to the `Backend/` .NET API — see `../Backend/.claude/guidelines/architecture.md` for the server-side shape.

See [Coding Standards](./coding-standards.md) for the tech choices (axios, TanStack Query, Redux Toolkit) and code style used within this structure.
