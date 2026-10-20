## Entity Setup

- Table Name: Test
- Entity: Test (optional, if not specified, will be derived from the <TableName>)
- Type: New

```Pseudocode
SampleText String Size=20 --Add some random text
```

## CRUD Coverage Standard

- Get: Yes

**Note**: Only the Get operation is covered for this entity. Use(../../App/IC.WebAPI/Endpoints/Tests/Get.cs) to connect to the API. Also add seed data in the database for testing purposes (Add in migration as well).
