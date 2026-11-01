# Frontend RBAC Standards

## Status

Early scaffolding. This is intended direction for how role-based access control should gate component visibility, not an established pattern yet — no roles/permissions map exists in the codebase today. Build it following these guidelines rather than copying an existing example.

## Core model

- **Roles** are the set of named roles a user can hold (e.g. `Admin`, `Manager`, `Agent`, `Viewer`). Roles come from the authenticated identity — with MSAL (see `src/config/authConfig.ts`), from token claims (e.g. `roles` / app roles on the ID token), not from local component state.
- **Permissions** are the fine-grained actions/visibility checks components actually care about (e.g. `incidents:create`, `incidents:delete`, `reports:view`). Components should check **permissions**, not roles, directly — roles are a backend/identity concept that resolves to a set of permissions, so UI code stays correct even if role-to-permission mappings change.

## Single source of truth

- Keep the role → permission mapping in exactly one place: `src/lib/rbac/permissionsMap.ts` (or equivalent under `src/lib/`, consistent with [Architecture Guidelines](./architecture.md)'s rule that cross-feature shared code lives in `src/lib/`).
- All roles and all permissions should be defined as a single enumerated union/const object there — do not redeclare role or permission names inline in feature or component code.
- Features/components import from this map; they never hardcode a role name or permission string literal outside of it.

## Central permissions map

- This map is a frontend-friendly projection of the backend's relational RBAC model (`Users ─< UserRoles >─ Roles ─< RolePermissions >─ Permissions`) — it does not need to mirror that normalization; it should be shaped for fast, readable lookups instead.
- Author the map nested by feature, not as a flat permission string array — this keeps it readable as features grow:
  ```ts
  export const ROLE_PERMISSIONS: Record<Role, Partial<Record<Feature, Action[]>>> = {
    Admin: { incidents: ['create', 'delete'], reports: ['view'] },
    Manager: { incidents: ['create'], reports: ['view'] },
    Agent: { incidents: ['create'] },
    Viewer: {},
  };
  ```
- Don't check against this nested shape directly in hot paths — flatten it once (e.g. at module init, or memoized per role) into a `Set<string>` of `"feature:action"` keys, and have the resolver check membership in that set. This gives O(1) lookups regardless of how large the nested map grows; the nesting is for human readability, the `Set` is for lookup speed — they're separate concerns.
- Expose a single resolver (e.g. `hasPermission(userRoles, 'feature:action')`) that components call — don't let components walk `ROLE_PERMISSIONS` directly, and don't re-flatten it ad hoc in multiple places.
- Surface the current user's roles/permissions through a provider/hook (e.g. `useAuth()` / `usePermissions()` under `src/providers/`), not by re-deriving them from the MSAL token in every component.

## Component-level checks

- Check permissions at the component level, before rendering — gate at the point a component would otherwise render restricted UI, not only at the route/page level. Page-level guards are not sufficient on their own since a page can contain a mix of visible and restricted sub-components.
- Prefer a small wrapper/guard component (e.g. `<RequirePermission permission="incidents:delete">...</RequirePermission>`) or a hook (`const canDelete = usePermission('incidents:delete')`) over ad-hoc conditionals scattered through JSX, so the check reads the same way everywhere.
- A failed check should render nothing (or an explicit fallback), not a disabled/broken version of the UI — don't rely on hiding via CSS.
- Remember this is a UI-visibility concern only: the backend must independently enforce the same permissions, since frontend checks can be bypassed.

## Page-level feature scope

- Each page declares the feature(s) it uses, as an array (a page can combine more than one, e.g. an incidents page embedding a reports widget: `features: ['incidents', 'reports']`). Provide this via route metadata or a context provider wrapping the page.
- Components under that page can then call `usePermission('create')` — the hook resolves `'create'` against each of the page's declared feature scopes without the caller repeating the feature prefix.
- Disambiguation: if more than one of the page's declared features defines the same action name (e.g. both `incidents` and `reports` have `view`), an unscoped call is ambiguous — the hook should throw/warn in that case rather than silently picking one, forcing the caller to use the explicit `'reports:view'` form instead.
- A component that's genuinely cross-feature (e.g. a shared sidebar not tied to one page's scope) should always use the explicit `'feature:action'` form rather than relying on page scope.

## Keeping the map current

- When a role's permissions change, update `ROLE_PERMISSIONS` in the single source-of-truth file only — never patch an individual component's logic as a workaround.
- When a new feature introduces a new permission, add it to the central `Permission` union/type first, then reference it from the feature's components — don't invent a new string literal at the call site.
- Treat the permissions map as reviewed alongside backend role definitions; if the backend's role/claim shape changes (see `../Backend/.claude/guidelines/architecture.md`), update this map in the same change.

See [Architecture Guidelines](./architecture.md) for where RBAC code should live, and [Coding Standards](./coding-standards.md) for general TypeScript/React conventions to follow while implementing it.
