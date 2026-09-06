# Frontend Coding Standards

## TypeScript

- `verbatimModuleSyntax` is on — use `import type { X }` / `export type { X }` for type-only imports and exports.
- `noUnusedLocals` and `noUnusedParameters` are on — the build fails on unused bindings; prefix an intentionally-unused parameter with `_`.
- `erasableSyntaxOnly` is on — avoid TS syntax that requires runtime transformation (e.g. `enum`, parameter properties); prefer plain objects/unions and explicit fields.
- Prefer explicit `interface`/`type` definitions colocated with the feature (`I<Feature>.ts`) over inline object types for anything shared across files.

## React

- The React Compiler (`reactCompilerPreset()` via `@rolldown/plugin-babel`) is active — do not hand-write `useMemo`, `useCallback`, or `React.memo` for optimization; let the compiler handle it. Only reach for them if correctness (not performance) requires a stable reference.
- Use axios for HTTP requests, wrapped in TanStack Query (`@tanstack/react-query`) hooks (see [Architecture Guidelines](./architecture.md) for where hooks live) — TanStack Query owns caching, background updates, and server sync; don't hand-roll that with `useEffect`.
- For local component state, prefer React's built-in `useState`/`useReducer`; reach for Redux Toolkit (`@reduxjs/toolkit`) only for state that's genuinely shared across unrelated parts of the app.
- Constant functional components (only one per file for pages, components).
  ```tsx
  export const MyComponent = () => {
    return <div>Hello World</div>;
  };
  ```

See [Architecture Guidelines](./architecture.md) for folder/file structure (pages, components, features).

## Linting

- Run `npm run lint` (ESLint over the whole project) before considering frontend work done.
- Run `npm run build` (`tsc -b && vite build`) to catch type errors — `noUnusedLocals`/`noUnusedParameters` failures only surface here, not in the dev server.

## General

- Follow the existing minimal style — no speculative abstractions ahead of a second real use case.
