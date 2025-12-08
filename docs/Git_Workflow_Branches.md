# Project Git Workflow & Branch Naming Conventions

This document defines the Git workflow, branch naming rules, and pull request process for a project with a **React frontend**, **.NET backend**, and **database scripts**, all inside a single repository.

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
```

---

# 2. Branch Naming Conventions

## Core Branches
| Branch | Purpose |
|--------|---------|
| `main` | Stable, production-ready code |
| `dev` | Integration branch used for all teams |

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
```

---

# 4. Pull Request Workflow

## Step 1 — Create branches from `dev`
Example:
```
git checkout dev
git checkout -b backend/feature/add-login-endpoint
```

## Step 2 — Open a Pull Request into `dev`
PR rules:
- Clear title (e.g., `Feature: Add Login Endpoint`)  
- All checks must pass  
- At least one teammate review  
- No conflicts  

## Step 3 — Merge using **Squash Merge**
- 1 branch = 1 PR = 1 commit in `dev`  

## Step 4 — Promote `dev` → `main`
- Only for releases or milestones  
- PR title example:
```
Release: v1.0.0
```
- Merge using **Squash Merge**

---

# 5. Updating Your Feature Branch with Latest dev

Always **rebase your feature branch onto dev** to keep it up-to-date before opening a PR:

```
git fetch origin
git checkout backend/feature/add-login-endpoint
git rebase origin/dev
```

- **Why force-push is needed:**  
  Rebase rewrites your branch’s commit history. The remote branch still has the old commits, so a normal push would be rejected. Use:

```
git push --force-with-lease
```

- `--force-with-lease` ensures you don’t accidentally overwrite others’ work.  
- This updates the PR with your rebased commits so CI/CD and branch protection rules can run correctly.

---

# 6. GitHub Branch Protection Rules

Apply to **main** (and optionally `dev`):

- Require pull request before merging  
- Require review from Code Owners  
- Require status checks to pass  
- Require linear history (no merge commits)  
- Require signed commits  
- Dismiss stale pull request approvals  
- Block direct pushes  
- Ensure all conversations are resolved  

---

# 7. CODEOWNERS

Add `.github/CODEOWNERS`:

```
src/backend/ @seifmaazouz @m-shall
src/frontend/ @A-agha2607
src/database/ @MostafaMohamed666
```

- Automatically requests reviews from listed users when files in those paths are modified.  
- Subfolders are automatically included.

---

# 8. Summary Table

| Action | Branch | Merge Method |
|--------|--------|--------------|
| Backend → dev | backend/feature/... | Squash Merge |
| Frontend → dev | frontend/feature/... | Squash Merge |
| Database → dev | database/feature/... | Squash Merge |
| dev → main | Release PR | Squash Merge |
| Feature branch sync | Rebase from dev | Local rebase |

---

# 9. Why This Structure?

- Separates frontend, backend, and database work cleanly  
- Avoids long-lived backend/frontend/database branches (reduces conflicts)  
- Each task has an isolated branch  
- Clean PR history using squash merge  
- Easy collaboration across teams  
- CI/CD integrates smoothly  
- `main` stays production-ready  
- `dev` stays stable but evolving
