# Contributing to Whisp

## Development Scope

Whisp is organized into two project areas:
- `frontend` -> `src/frontend`
- `backend` -> `src/backend`

When contributing, make sure your commit clearly indicates which area it affects.

## Commit Convention

This project uses Conventional Commits with mandatory scope:

```text
type(scope): short description
```

Where:
- `type` is one of: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `build`, `ci`, `perf`, `style`, `revert`
- `scope` must include the related project area:
  - `frontend`
  - `backend`
  - `frontend,backend` (if both are changed)

### Examples

```text
feat(frontend): add conversation sidebar with unread badges
fix(backend): validate conversation membership in chat hub
docs(frontend): update setup instructions for tanstack start
refactor(frontend,backend): align message dto naming across layers
```

## Pull Request Guidelines

- Keep PRs focused and small when possible.
- Link related issues/tasks in the PR description.
- Describe user-visible behavior changes.
- Include test notes (what you ran and what passed/failed).
- Update docs when behavior or setup changes.

## Local Checks

For frontend changes, run in `src/frontend`:

```bash
pnpm lint
pnpm test
pnpm build
```

For backend changes, run the corresponding lint/test/build commands for the backend project once available.

## Branching

- Use descriptive branch names, for example:
  - `feat/frontend-conversation-list`
  - `fix/backend-token-refresh`

## Code Style

- Follow existing project conventions and formatting.
- Prefer clear, maintainable code over clever solutions.
- Add or update tests for significant behavior changes.
