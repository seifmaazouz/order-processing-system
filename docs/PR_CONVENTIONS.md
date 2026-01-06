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

### Release-triggering (will create a release and changelog entry)
- feat → Minor version bump
- fix → Patch version bump
- docs → Patch version bump
- refactor → Patch version bump
- perf → Patch version bump
- test → Patch version bump

### Breaking Changes
To trigger a major version bump (breaking change), use one of the following:

- Add an exclamation mark after the type: `feat!:` or `fix!:`
- Or, add `BREAKING CHANGE:` in the body or footer of the PR description or commit message:

```
feat(api): remove deprecated endpoint

BREAKING CHANGE: The /old-endpoint route has been removed and will break clients using it.
```

Both methods are supported by semantic-release and will trigger a major version bump.

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
- Use **Squash and merge** (when merging to the dev branch)
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
