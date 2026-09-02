**Important Notes**:

- This template is used for defining the extended CRUD operations for updating entities.
- Ensure that all table names, columns, and operations are correctly specified and already exist according to the database schema.

## CRUD Coverage Extended (Required)

**Format**: (TableName, Columns, Operation at last)

- FunctionName: <FunctionName>
  - Table 1: (Use <TableName>)
    - Columns (optional): [<ColumnName1>, <ColumnName2>, ...]
    - Join (optional): INNER|LEFT|RIGHT
  - Table 2: (If there is another table involved in the extended CRUD operation)
    - Columns (optional): [<ColumnName1>, <ColumnName2>, ...]
    - Join (optional): INNER|LEFT|RIGHT
  - Table n: (If there is another table involved in the extended CRUD operation)
    - Columns (optional): [<ColumnName1>, <ColumnName2>, ...]
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
- If Columns (optional) is not specified, all columns from the table will be included by default.

## Open Questions

- ...
