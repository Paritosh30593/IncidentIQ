---
title: CRUD Setup
description: Creates Entity, Repositories, Services, APIs, Unit tests and Integration tests setup for CRUD operations.
arguments: $ARGUMENTS
argument-hint: ["SpecFileRef", "Versioning [Yes/No (default No)]"]
---

You are helping to set up CRUD operations for a new feature based on the user input below. Always adhere to any rules or requirements set out in any CLAUDE.md files when responding.

User input: $ARGUMENTS

## High level behavior

Your job will be to turn the spec file `SpecFileRef` into:

- A detailed markdown plan file of setup for CRUD operations based on the spec file under the `Backend/.claude/plans/` directory to generate:
  - Entity, Repositories (interfaces and implementations)
  - Services (interfaces and implementations)
  - APIs (getById, getAll, create, update, delete, ...)
  - Unit tests and Integration tests

## Step 1. Check the template

Before generating the CRUD setup,

- Ensure that the spec file provided by the user contains all necessary information for creating entities, repositories, services, APIs, and tests. This includes field definitions, relationships, and any specific business rules that need to be enforced.

Refer to `Backend/.claude/commands/templates/crud-setup-template.md` for more details on format and terminology, and [Database Standards](../guidelines/database-standards.md) for constraint naming conventions.

## Step 2. Parse the arguments

- Extract the `SpecFileRef` and `Versioning` arguments from the user input.
- Validate that the `SpecFileRef` points to an existing spec file.
- Ensure that the `Versioning` argument is either "Yes" or "No".

```text
If `Versioning` is "Yes"
    - The entity will have system versioning enabled.
Else
    - The entity will not have system versioning enabled.
```

## Step 3. Generate CRUD setup

- Based on the parsed arguments and the spec file, generate the necessary files and code for CRUD operations.
- Ensure that the entity, repositories, services, APIs, and tests are created according to the specifications.
- Apply system versioning to the entity if the `Versioning` argument is "Yes".
- Ensure that all generated code follows the project's guidelines and best practices.

## Step 4. Review and finalize

- Verify that system versioning is correctly applied if enabled.
- Conduct a final check to ensure adherence to the project's guidelines and best practices.
- Confirm that all generated CRUD operations covered in the spec file are functioning correctly through unit and integration tests.
- Generate migrations and run migrations to apply any changes to the database schema.
- Document any deviations from the spec file and ensure they are communicated to the relevant stakeholders.
