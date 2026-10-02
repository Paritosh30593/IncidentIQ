---
title: CRUD Update Command
description: Command for updating CRUD operations in the backend.
arguments: $ARGUMENTS
argument-hint: ["SpecFileRef", "Versioning [Yes/No (default No)]"]
---

You are helping to update existing CRUD operations for a new feature based on the user input below. Always adhere to any rules or requirements set out in any CLAUDE.md files when responding.

User input: $ARGUMENTS

## High level behavior

Your job will be to turn the spec file `SpecFileRef` into a detailed markdown plan file only. Do **NOT** generate or modify any source code, migrations, or tests as part of this command — your output is limited to the plan file.

- Produce a detailed markdown plan file of the update to CRUD operations based on the spec file under the `Backend/.claude/plans/` directory, covering:
  - Entity, Repositories (interfaces and implementations)
  - Services (interfaces and implementations)
  - APIs (getById, getAll, create, update, delete)
  - Unit tests and Integration tests

Once the plan file is written, stop and let the user review it. The user will explicitly ask you to execute the plan afterward — do not proceed to implementation on your own.

## Step 1. Check the template

Before planning the CRUD update,

- Ensure that the spec file provided by the user contains all necessary information for updating entities, repositories, services, APIs, and tests. This includes specific business rules that need to be enforced.

Refer to `Backend/.claude/commands/templates/crud-update-template.md` for more details on format and terminology, and [Database Standards](../guidelines/database-standards.md) for constraint naming conventions.

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

## Step 3. Plan the CRUD update

- Based on the parsed arguments and the spec file, plan out the necessary file and code changes for CRUD operations and document them in the plan file — do not create or modify them yet.
- The plan should describe how the entity, repositories, services, APIs, and tests will be created/updated according to the specifications.
- Ensure the planned approach follows the project's guidelines and best practices.

## Step 4. Plan review and finalization steps

- Include a final review checklist in the plan to ensure adherence to the project's guidelines and best practices.
- Include in the plan how unit and integration tests will confirm that all CRUD operations covered in the spec file work correctly.
- Include in the plan the migrations that will need to be generated and run to apply schema changes.
- Include a note in the plan to document any deviations from the spec file and how they'll be communicated to relevant stakeholders.

These steps describe what the plan file should contain — none of them should be carried out until the user reviews the plan and explicitly asks you to execute it.
