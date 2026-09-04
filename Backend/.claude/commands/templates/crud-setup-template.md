## Entity Setup

- Table Name: <TableName>
- Entity: <EntityName> (optional, if not specified, will be derived from the <TableName>)
- Type: New/Existing

```Pseudocode
<Entity Declaration>
```

## CRUD Coverage Standard

- Get: Yes/No (Default Yes)
- GetAll: Yes/No (Default Yes)
- Create: Yes/No (Default Yes)
- Update: Yes/No (Default Yes)
- Delete: Yes/No (Default Yes)
- GetBy<Key>: Yes/No (Default No).

## CRUD Coverage Extended (Optional can be removed from spec if not needed)

- `FunctionName` 1
  ...
- `FunctionName` 2
  ...

**Note**: `FunctionName` needs to be translated to services, repository, api, test methodname.

## Open Questions

- ...
