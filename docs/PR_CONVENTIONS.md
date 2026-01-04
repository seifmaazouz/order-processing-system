# Pull Request Conventions

This project uses **semantic-release**.
PR titles are used to determine the next version.

Only the **PR title** matters (especially when using squash merge).

---

## PR Title Format

<type>(optional-scope): short description

### Examples
feat: add admin order automation
fix: prevent duplicate checkout
refactor: simplify order service
docs: update API documentation
chore: update dependencies

---

## Allowed Types and Their Meaning

### Release-triggering
- feat → Minor version bump
- fix → Patch version bump

### Non-breaking / maintenance
- refactor → Patch
- perf → Patch
- docs → Patch
- test → Patch
- chore → Patch

### Breaking Changes
Use ! to indicate a breaking change:

feat!: change authentication response format
fix!: remove legacy endpoint

→ Major version bump

---

## Scope (optional)
Scope describes the affected area:

feat(backend): add order approval flow
fix(api): handle null ISBN

Common scopes:
- backend
- frontend
- api
- auth
- db

---

## Important Rules

- PR title **must follow the format**
- Use **Squash and merge**
- Feature branch commit messages can be anything
- The squash commit message = PR title

---

## Good vs Bad

❌ Bad
update backend
final changes

✅ Good
feat(backend): add admin order automation
fix(api): prevent duplicate checkout

---

## Summary

> If the PR title is correct, semantic-release will work correctly.
