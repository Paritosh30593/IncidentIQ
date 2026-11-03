# RBAC Backend Entity Spec

## Purpose

Defines the database entities, configuration and seeding rules for role-based access control. The backend is the single source of truth for roles and permissions. The frontend only receives a resolved permission list (see "API contract").

## Model

```
Users ──< UserRoles >── Roles ──< RolePermissions >── Permissions >── Features
```

- Identity comes from Entra ID. The access token's `oid` claim maps to `Users.ExternalId`.
- Roles and permissions are never stored in the token.
- Permissions are code-defined and seeded. Roles and their mappings are data and can be managed at runtime.
- Features group permissions for UI/admin display and feature-flag-style enablement. Adding a Feature does **not** create a new enforceable permission — enforcement still requires a corresponding `[Authorize(Policy=...)]` check in code, and the Permission row itself is still synced from the Domain `Permissions` constants via the seeder. `SuperAdmin` manages Features and grants permissions to other roles; see "Roles" and "Seeding" below.

## Layer placement (Clean Architecture)

| Layer          | Contents                                                               |
| -------------- | ---------------------------------------------------------------------- |
| Domain         | Entities below, `Permissions` constants                                |
| Application    | `IPermissionService`, role-assignment commands/queries                 |
| Infrastructure | EF Core configurations, migrations, seeding, permission cache          |
| API            | Authorization policies/handler, `/api/me/permissions`, admin endpoints |

## Entities

### User

| Property              | Type           | Constraints                          |
| --------------------- | -------------- | ------------------------------------ |
| Id                    | Guid           | PK                                   |
| ExternalId            | string(64)     | Required, unique index (Entra `oid`) |
| Email                 | string(256)    | Required, unique index               |
| DisplayName           | string(200)    | Required                             |
| IsActive              | bool           | Default true                         |
| CreatedAt / UpdatedAt | DateTimeOffset | Required                             |

### Role

| Property    | Type           | Constraints                               |
| ----------- | -------------- | ----------------------------------------- |
| Id          | int            | PK                                        |
| Name        | string(100)    | Required, unique index                    |
| Description | string(500)    | Optional                                  |
| IsSystem    | bool           | System roles cannot be deleted or renamed |
| CreatedAt   | DateTimeOffset | Required                                  |

### Permission

| Property    | Type        | Constraints                                      |
| ----------- | ----------- | ------------------------------------------------ |
| Id          | int         | PK                                               |
| Key         | string(100) | Required, unique index, format `resource:action` |
| Description | string(500) | Optional                                         |
| FeatureId   | int         | FK -> Feature, required                          |

### Feature

| Property              | Type           | Constraints                          |
| ---------------------- | -------------- | ------------------------------------- |
| Id                    | int            | PK                                    |
| Name                  | string(100)    | Required, unique index                |
| Description           | string(500)    | Optional                              |
| IsEnabled             | bool           | Default true                          |
| CreatedAt / UpdatedAt | DateTimeOffset | Required                              |

- Managed exclusively by `SuperAdmin` (permission `features:manage`) via the admin UI — see "API contract".
- A Feature only groups existing code-defined Permissions and optionally toggles their visibility; it does not itself grant access, and creating one does not create a new enforceable permission.

### UserRole (join)

| Property   | Type           | Constraints              |
| ---------- | -------------- | ------------------------ |
| UserId     | Guid           | FK -> User, composite PK |
| RoleId     | int            | FK -> Role, composite PK |
| AssignedBy | Guid?          | FK -> User, audit        |
| AssignedAt | DateTimeOffset | Audit                    |

### RolePermission (join)

| Property     | Type | Constraints                    |
| ------------ | ---- | ------------------------------ |
| RoleId       | int  | FK -> Role, composite PK       |
| PermissionId | int  | FK -> Permission, composite PK |

## Relationships and delete behavior

- User 1..\* UserRole: cascade delete (or soft-delete users via `IsActive`; prefer soft delete).
- Role 1..\* UserRole: restrict delete while users are assigned.
- Role 1..\* RolePermission: cascade.
- Permission 1..\* RolePermission: restrict. Permissions are removed only through a migration.
- Feature 1..\* Permission: restrict delete while permissions reference it.

## EF Core configuration

```csharp
b.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
b.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
b.Entity<User>().HasIndex(x => x.ExternalId).IsUnique();
b.Entity<User>().HasIndex(x => x.Email).IsUnique();
b.Entity<Role>().HasIndex(x => x.Name).IsUnique();
b.Entity<Permission>().HasIndex(x => x.Key).IsUnique();
b.Entity<Feature>().HasIndex(x => x.Name).IsUnique();
b.Entity<Permission>()
    .HasOne<Feature>()
    .WithMany(f => f.Permissions)
    .HasForeignKey(x => x.FeatureId)
    .OnDelete(DeleteBehavior.Restrict);
```

## Permission key convention

- Format: `<resource>:<action>`, lowercase, e.g. `incidents:create`, `reports:export`.
- Feature/action specific, never page specific.
- Standard verbs: `view`, `create`, `update`, `delete`; domain verbs allowed (`assign`, `close`, `export`, `manage-roles`).
- Keys are declared once in a static `Permissions` class in Domain; the seeder syncs the table from it.
- Each key's `FeatureId` is assigned by the seeder based on a feature declared alongside the constant; Features are seeded first so the FK resolves.

## Seeding

- Seed all `Feature` rows referenced by `Permissions` constants (idempotent upsert by `Name`), then seed all `Permissions` constants (idempotent upsert by `Key`).
- Seed system roles (`SuperAdmin`, `Admin`, `Manager`, `Agent`, `Viewer`) with default permission mappings:

| Role       | Permissions                                                              |
| ---------- | ------------------------------------------------------------------------- |
| SuperAdmin | all, including `features:manage` and `roles:manage-permissions`           |
| Admin      | all, except `features:manage` and `roles:manage-permissions`              |
| Manager    | incidents:view/create/update/assign/close, reports:view/export            |
| Agent      | incidents:view/create/update                                              |
| Viewer     | incidents:view, reports:view                                              |

- `SuperAdmin` and `Admin` are both seeded with `IsSystem = true` (cannot be deleted or renamed). `features:manage` (create/edit Features) and `roles:manage-permissions` (edit a role's permission set) are exclusive to `SuperAdmin` — `Admin` cannot grant itself or others those permissions through the UI.
- New users default to `Viewer` (least privilege).

## Effective permissions

```csharp
var keys = await db.UserRoles
    .Where(ur => ur.User.ExternalId == oid && ur.User.IsActive)
    .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Key))
    .Distinct()
    .ToListAsync();
```

- Cache per user for about 5 minutes. Invalidate on role assignment or role-permission changes.
- Inactive or unknown users resolve to an empty set.

## Enforcement

- Every endpoint uses `[Authorize(Policy = Permissions.X)]`, backed by a `PermissionRequirement` and `PermissionHandler` that call `IPermissionService`.
- Return 401 when unauthenticated and 403 when authenticated but lacking permission.
- Resource-level rules (e.g. only the assignee may close) use resource-based authorization on top of permissions.
- Role-assignment commands must reject a change that would leave zero users holding the `SuperAdmin` role, to prevent locking the system out of role/feature management entirely.

## API contract

`GET /api/me/permissions`

```json
{
  "userId": "guid",
  "roles": ["Manager"],
  "permissions": ["incidents:create", "reports:view"]
}
```

Admin endpoints (require `users:manage-roles` unless noted):

- `GET /api/roles`
- `PUT /api/users/{id}/roles`
- `PUT /api/roles/{id}/permissions` — requires `roles:manage-permissions` (`SuperAdmin` only)

Feature admin endpoints (require `features:manage`, `SuperAdmin` only):

- `GET /api/features`
- `POST /api/features`
- `PUT /api/features/{id}`

## Auditing

Role assignments record `AssignedBy` and `AssignedAt`. Log all role and permission changes with actor, target and timestamp. Feature create/edit is logged the same way.

## Out of scope (for now)

Per-user permission overrides, tenant-scoped roles, deny rules, and dynamic/no-code permission creation (new `Permission` rows still require a code change and seeder sync — `Feature` management only groups and toggles visibility of existing ones). Add `TenantId` to `UserRole` (and its PK) if tenancy is needed later.
