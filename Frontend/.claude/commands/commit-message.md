---
description: Generate a commit message for your code changes.
allowed-tools:
  - git status *
  - git diff --staged
  - git commit *
---

## Context:

- Current git status: !`git status`
- Current git diff: !`git diff --staged`

## Your Task:

Analyze the code changes and generate a concise and descriptive commit message that accurately summarizes the modifications made and should follow best practices.

## Commit types with emojis:

- **feat**: ✨ A new feature
- **fix**: 🐛 A bug fi!x
- **refactor**: ♻️ Code refactoring
- **docs**: 📝 Documentation changes
- **style**: 💄 Code style changes (formatting, missing semi-colons, etc.)
- **test**: ✅ Adding or updating tests
- **perf**: ⚡ Performance improvements
- **chore**: 🔧 Other changes that don't modify src or test files

## Format:

Use the following format for your commit message:

```
<emoji> <type>: <concise summary>
<optional_body_explaining_why>
```

## Output:

1. Show summary of the changes made.
2. Propose commit message with appropriate emoji and type.
3. Ask for confirmation before committing.

DO NOT commit the changes automatically. Commit after user approval.
