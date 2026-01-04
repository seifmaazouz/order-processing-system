# Project Git Workflow & Branch Naming Conventions

This document defines the Git workflow, branch naming rules, pull request process, and branch protection for a project with a **React frontend**, **.NET 10 backend**, and **PostgreSQL database scripts**, all inside a single repository using **semantic-release** for automated versioning.

---

# 1. Branch Structure

```
main          → production (semantic-release creates tags)
dev           → integration branch
feature/*     → new features

backend/feature/...   # Backend features
backend/fix/...       # Backend fixes
backend/bugfix/...    # Backend bug fixes
backend/hotfix/...    # Backend urgent fixes

frontend/feature/...  # Frontend features
frontend/fix/...      # Frontend fixes
frontend/bugfix/...   # Frontend bug fixes
frontend/hotfix/...   # Frontend urgent fixes

database/feature/...  # Database features/migrations
database/fix/...      # Database fixes
database/bugfix/...   # Database bug fixes
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
backend/fix/auth-null-reference
backend/bugfix/payment-calculation-error
backend/hotfix/critical-security-patch
```

## Frontend Branch Examples

```
frontend/feature/navbar-ui
frontend/fix/button-hover-bug
frontend/bugfix/modal-not-closing
frontend/hotfix/urgent-login-fix
```

## Database Branch Examples

```
database/feature/add-user-table
database/fix/null-constraint-issue
database/bugfix/migration-script-failure
database/hotfix/emergency-data-fix
```

---

# 3. Workflow Diagram

```
(feature branches) → dev → main (semantic-release)
```

Simplified Flow:

```
backend/feature/...   ─┐
frontend/feature/...   ├──→  dev  ───→  main (automatic versioning)
database/feature/...  ─┘
misc/...              ─┘
```

**Semantic-release automatically:**
- Creates version tags (v1.0.0, v1.1.0, v1.1.1)
- Generates GitHub releases
- Builds and pushes Docker images

---

# 4. Pull Request Workflow

## Development Flow

### Step 1 — Create feature branches from `dev`

```
git checkout dev
git pull origin dev  # Ensure latest
git checkout -b backend/feature/add-login-endpoint
```

### Step 2 — Open PR to `dev`

PR rules:
* **Title must follow conventional commit format** (e.g., `feat: add login endpoint`)
* All CI checks must pass (backend tests, frontend build, PR title validation)
* At least one teammate review required
* No merge conflicts
* PR template must be filled out

### Step 3 — Squash merge to `dev`

* 1 feature = 1 PR = 1 squash commit
* Delete merged branch after merge

## Release Flow (Automated)

### Step 4 — Merge `dev` to `main` (when ready for release)

```
# Create PR: dev → main (only team leads can merge)
# Semantic-release automatically:
# - Analyzes commit messages for version bump
# - Creates GitHub release with changelog
# - Builds and pushes Docker images
# - Tags repository with new version
```

**No manual release branches needed!** Semantic-release handles everything automatically based on your PR titles.

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
* Require review from Code Owners (team leads)
* Require at least 2 reviewers
* Require status checks to pass (backend tests, frontend build, semantic-release)
* Require linear history (no merge commits)
* Require signed commits
* Dismiss stale pull request approvals
* Block direct pushes
* Ensure all conversations are resolved
* **Only allow PRs from `dev`** (no direct feature branches)

## For `dev`

* Require pull request before merging
* Require review from at least 1 teammate
* Require status checks to pass (PR title validation, backend tests)
* Require linear history
* Require signed commits
* Dismiss stale pull request approvals
* Block direct pushes
* Ensure all conversations are resolved
* Only allow PRs from structured branches:

  * `backend/feature/*`, `backend/fix/*`, `backend/bugfix/*`, `backend/hotfix/*`
  * `frontend/feature/*`, `frontend/fix/*`, `frontend/bugfix/*`, `frontend/hotfix/*`
  * `database/feature/*`, `database/fix/*`, `database/bugfix/*`, `database/hotfix/*`
  * `misc/*`

---

# 7. Semantic Versioning & Conventional Commits

This project uses **semantic versioning** with **conventional commits** for automated releases:

## Version Format: MAJOR.MINOR.PATCH

- `1.0.0` → First production release
- `1.0.1` → Bug fix (PATCH)
- `1.1.0` → New feature (MINOR)
- `2.0.0` → Breaking change (MAJOR)

## Conventional Commit Format

```
type(scope): description

[optional body]

[optional footer]
```

### Commit Types
- `feat:` → New feature (triggers MINOR version)
- `fix:` → Bug fix (triggers PATCH version)
- `docs:` → Documentation changes
- `style:` → Code style changes
- `refactor:` → Code refactoring
- `test:` → Adding/updating tests
- `chore:` → Maintenance tasks

### Examples
```
feat: add user authentication
fix: resolve shopping cart calculation bug
docs: update API documentation
feat: implement JWT authentication
BREAKING CHANGE: remove old login method
```

## Automatic Releases

- Semantic-release analyzes commit messages
- Creates version tags and GitHub releases automatically
- Builds and pushes Docker images with version tags

## Why Simplified Workflow

**Direct dev → main merges** because:

- **Semantic-release handles versioning automatically** based on commit messages
- **No need for manual release branches** - automation does it all
- **Faster deployment cycle** with less branch management overhead
- **Conventional commits ensure proper version bumps** (feat → minor, fix → patch)

**This is modern Git Flow** used by projects with automated CI/CD.

---

# 8. CODEOWNERS

Add `.github/CODEOWNERS`:

```
src/Backend/** @seifmaazouz @m-shalll
src/Frontend/** @A-agha2607
src/Database/** @MostafaMohamed666
```

* Automatically requests reviews from listed users when files in those paths are modified
* Subfolders are automatically included

---

# 9. Summary Table

| Action              | From Branch          | To Branch          | Merge Method |
| ------------------- | -------------------- | ------------------ | ------------ |
| Feature → dev       | backend/feature/...  | dev                | Squash Merge |
| Feature → dev       | frontend/feature/... | dev                | Squash Merge |
| Feature → dev       | database/feature/... | dev                | Squash Merge |
| Feature → dev       | misc/...             | dev                | Squash Merge |
| Dev → main          | dev                  | main               | Merge Commit |
| Main → dev sync     | main                 | dev                | Merge Commit |
| Feature branch sync | dev                  | feature branch     | Rebase       |

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