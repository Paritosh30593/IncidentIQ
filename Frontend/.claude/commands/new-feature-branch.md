---
description: Create a new git branch from a short feature idea
argument-hint: "[Short feature description]"
allowed-tools: Bash(git switch:*)
---

You are helping to spin up a new git branch for this application, from a short idea provided in the user input below. Always adhere to any rules or requirements set out in any CLAUDE.md files when responding.

User input: $ARGUMENTS

## High level behavior

Your job will be to turn the user input above into a safe git branch name not already taken (e.g. `new-heist-form`), switch to it, and report back to the user.

## Step 1. Check the current branch

Check the current Git branch, and abort this entire process if there are any uncommitted, unstaged, or untracked files in the working directory. Tell the user to commit or stash changes before proceeding, and DO NOT GO ANY FURTHER.

## Step 2. Parse the arguments

From `$ARGUMENTS`, derive `branch_name`:

- Rules:
  - Lowercase
  - Kebab-case
  - Only `a-z`, `0-9` and `-`
  - Replace spaces and punctuation with `-`
  - Collapse multiple `-` into one
  - Trim `-` from start and end
  - Maximum length 40 characters
- Example: `card-component` or `card-component-dashboard`.

If you cannot infer a sensible `branch_name`, ask the user to clarify instead of guessing.

## Step 3. Switch to a new Git branch

Switch to a new Git branch using the `branch_name` derived above. If the branch name is already taken, then append a version number to it: e.g. `card-component-01`.

## Step 4. Final output to the user

After switching branches, respond to the user with a short summary in this exact format:

Branch: <branch_name>
