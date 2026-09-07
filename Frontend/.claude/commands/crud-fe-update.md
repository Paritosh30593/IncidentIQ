---
description: Command for updating CRUD operations in the frontend.
argument-hint: "[SpecFileRef]"
---

You are helping to update existing CRUD operations for a new feature in the frontend based on the user input below. Always adhere to any rules or requirements set out in any CLAUDE.md files when responding.

User input: $ARGUMENTS

## High level behavior

- Your job will be to turn the spec file `SpecFileRef` into:
  - Updated frontend entities for CRUD operations (refer backend DTOs for structure and types)
  - Updated API service calls to match the backend changes
  - Updated state management logic (if applicable)
  - Updated unit tests and integration tests for the frontend

## Step 1. Parse the arguments

- Extract and validate if the `SpecFileRef` points to an existing spec file.

## Step 2. Check the backend

- Before updating the frontend CRUD setup,
  - Ensure that the backend CRUD setup is correctly implemented and accessible, as the frontend will rely on the backend APIs for data operations.

## Step 3. Update CRUD setup

- Based on the parsed arguments and the spec file, update the necessary files and code for CRUD operations.
- Ensure that the entity, API calls, and tests are updated according to the specifications.
- Ensure that all updated code follows the project's guidelines and best practices.

## Step 4. Review and finalize

- Conduct a final check against the spec file.
- Confirm that all updated CRUD operations covered in the spec file are functioning correctly through unit and integration tests.
- Document any deviations from the spec file and ensure they are communicated to the relevant stakeholders.
