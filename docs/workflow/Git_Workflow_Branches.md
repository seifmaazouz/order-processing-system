# Project Git Workflow & Branch Naming Conventions

This document defines the Git workflow, branch naming rules, pull request process, and branch protection for a project with a **React frontend**, **.NET 10 backend**, and **PostgreSQL database scripts**, all inside a single repository.

---

# 1. Branch Structure

```
main          # Production-ready code
dev           # Integration branch (all features merge here)

backend/feature/...   # Backend features
backend/bugfix/...    # Backend fixes
backend/hotfix/...    # Backend urgent fixes

frontend/feature/...  # Frontend features
frontend/bugfix/...   # Frontend fixes
frontend/hotfix/...   # Frontend urgent fixes

database/feature/...  # Database features/migrations
database/bugfix/...   # Database fixes
database/hotfix/...   # Database urgent fixes
misc/...              # Miscellaneous general-purpose branches
```

---

# 2. Branch Naming Conventions

## Core Branches

| Branch | Purpose                               |
| ------ | ------------------------------------- |
| `main` | Stable, production-ready code         |
| `dev`  | Integration branch used for all teams |

---

## Backend Branch Examples

```
backend/feature/login-endpoint
backend/bugfix/fix-auth-nullref
backend/hotfix/critical-auth-crash
```

## Frontend Branch Examples

```
frontend/feature/navbar-ui
frontend/bugfix/button-hover
frontend/hotfix/build-fix
```

## Database Branch Examples

```
database/feature/add-user-table
database/bugfix/fix-null-constraint
database/hotfix/critical-index-fix
```

---

# 3. Workflow Diagram

```
(feature branches) → dev → main
```

Detailed:

```
backend/feature/...   ─┐
backend/bugfix/...     ├──→  dev  ───→  main
frontend/feature/...  ─┘
frontend/bugfix/...   ─┘
database/feature/...  ─┘
database/bugfix/...   ─┘
misc/...              ─┘
```

---

# 4. Pull Request Workflow

## Step 1 — Create branches from `dev`

```
git checkout dev
git checkout -b backend/feature/add-login-endpoint
```

## Step 2 — Open a Pull Request into `dev`

PR rules:

* Clear title (e.g., `Feature: Add Login Endpoint`)
* All checks must pass
* At least one teammate review
* No conflicts

## Step 3 — Merge using **Squash Merge**

* 1 branch = 1 PR = 1 commit in `dev`

## Step 4 — Promote `dev` → `main`

* Only for releases or milestones
* PR title example:

```
Release: v1.0.0
```

* Merge using **Squash Merge**
* Must pass branch protection rules (Code Owners, checks, reviewers)

---

# 5. Updating Your Feature Branch with Latest dev

Always **rebase your feature branch onto dev** before opening a PR:

```
git fetch origin
git checkout backend/feature/add-login-endpoint
git rebase origin/dev
```

* **Force-push required:**

```
git push --force-with-lease
```

* Ensures PR is updated correctly without overwriting others’ work

---

# 6. GitHub Branch Protection Rules

## For `main`

* Require pull request before merging
* Require review from Code Owners
* Require at least 3 reviewers
* Require status checks to pass
* Require linear history (no merge commits)
* Require signed commits
* Dismiss stale pull request approvals
* Block direct pushes
* Ensure all conversations are resolved
* Only allow PRs from `dev` or `hotfix/*`

## For `dev`

* Require pull request before merging
* Require review from Code Owners
* Require status checks to pass
* Require linear history
* Require signed commits
* Dismiss stale pull request approvals
* Block direct pushes
* Ensure all conversations are resolved
* Only allow PRs from structured branches:

  * `backend/feature|hotfix|bugfix/*`
  * `frontend/feature|hotfix|bugfix/*`
  * `database/feature|hotfix|bugfix/*`
  * `misc/*`

---

# 7. CODEOWNERS

Add `.github/CODEOWNERS`:

```
src/Backend/** @seifmaazouz @m-shalll
src/Frontend/** @A-agha2607
src/Database/** @MostafaMohamed666
```

* Automatically requests reviews from listed users when files in those paths are modified
* Subfolders are automatically included

---

# 8. Summary Table

| Action              | Branch               | Merge Method |
| ------------------- | -------------------- | ------------ |
| Backend → dev       | backend/feature/...  | Squash Merge |
| Frontend → dev      | frontend/feature/... | Squash Merge |
| Database → dev      | database/feature/... | Squash Merge |
| Misc → dev          | misc/...             | Squash Merge |
| dev → main          | Release PR           | Squash Merge |
| Feature branch sync | Rebase from dev      | Local rebase |

---

# 9. Why This Structure?

* Separates frontend, backend, and database work cleanly
* Avoids long-lived branches (reduces conflicts)
* Each task has an isolated branch
* Clean PR history using squash merge
* Easy collaboration across teams
* CI/CD integrates smoothly
* `main` stays production-ready
* `dev` stays stable but evolving
* Enforces merge restrictions via GitHub Actions workflows