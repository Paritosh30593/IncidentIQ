## Entity Setup (Optional can be removed from spec if not needed)

- Table Name: <TableName>
- Entity: <EntityName> (optional, if not specified, will be derived from the <TableName>)
- Type: New/Existing
- Declaration:
  ...

**Terminologies**: Below will be the commonly used terms in the CRUD setup (combination of SQL and pseudocode):

- PK: Primary Key
- FK(TableName, ColumnName): Foreign Key
- UK: Unique Key
- UUID: Universally Unique Identifier
- CUK (params1, params2, ...): Composite Unique Key
- IDX (params1, params2, ...): Index Constraint

## CRUD Coverage Standard

- Get: Yes/No (Default No)
- GetAll: Yes/No (Default No)
- Create: Yes/No (Default No)
- Update: Yes/No (Default No)
- Delete: Yes/No (Default No)

## CRUD Coverage Extended (Optional can be removed from spec if not needed)

**Format**: (TableName, Columns, Operation at last)

- FunctionName: <FunctionName>
  - Table 1: (If `Enitity Setup` is provided, use <TableName> else provide explicitly)
    - Columns: [<ColumnName1>, <ColumnName2>, ...]
    - Join (optional): INNER|LEFT|RIGHT
  - Table 2: (If there is another table involved in the extended CRUD operation)
    - Columns: [<ColumnName1>, <ColumnName2>, ...]
    - Join (optional): INNER|LEFT|RIGHT
  - Table n: (If there is another table involved in the extended CRUD operation)
    - Columns: [<ColumnName1>, <ColumnName2>, ...]
    - Join (optional): INNER|LEFT|RIGHT
  - Operation: Get|GetAll|Create|Update|Delete

- FunctionName: <FunctionName>
  ...

**Note**:

- **Required** primary key for target <TableName> when performing Get, Create, Update or Delete Operation.
- **Always** perform joins when Get or GetAll is used under Operation and Has multiple tables involved, follow ER hierarchy.
  - Refer Join (optional) in the table definitions above.
  - If not specified, default join type will be INNER.
  - Not required for Create, Update, or Delete Operation.
- **DO NOT** allow multiple tables when perform Create, Update, or Delete Operation.
- <FunctionName> needs to be translated to services, repository, api, test methodname.

## Open Questions

- ...
