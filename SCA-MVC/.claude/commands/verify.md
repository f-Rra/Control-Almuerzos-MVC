---
description: >
  7-phase verification pipeline: build → diagnostics → antipatterns → tests →
  security → format → diff. Invoke before creating a PR, after a feature, after
  a refactor, or when the user says "verify", "check this", "is this ready".
---

# /verify

## What

Runs a sequential, 7-phase verification pipeline that catches issues at every level —
from compiler errors to subtle antipatterns to formatting drift. Each phase produces
a clear PASS or FAIL result. Critical failures (phases 1-2) short-circuit the pipeline
because later phases cannot produce meaningful results on broken code.

The pipeline is designed to answer one question: **"Is this code ready for review?"**

### The 7 Phases

| Phase | Tool | What It Catches |
|-------|------|-----------------|
| 1. Build | `dotnet build` | Compilation errors, missing references, type mismatches |
| 2. Diagnostics | Analyzer warnings | Nullable reference issues, code quality warnings |
| 3. Antipatterns | Source analysis | async void, sync-over-async, DateTime.Now, broad catch, missing CancellationToken, EF queries without AsNoTracking |
| 4. Tests | `dotnet test` | Failing tests (skip if no test project) |
| 5. Security | Security scan | Hardcoded secrets, SQL injection patterns, missing auth attributes |
| 6. Format | `dotnet format --verify-no-changes` | Code style drift, formatting inconsistencies |
| 7. Diff Review | `git diff` analysis | Accidental changes, debug leftovers, TODO/HACK markers |

## When

- After completing a feature or bug fix
- Before creating a pull request
- After a major refactor
- After merging upstream changes
- When the user says "verify", "check this", "is this ready", "run checks"
- As the final step before marking a task complete

**Quick check alternative:** For small changes where the full pipeline is overkill,
run just phase 1 (build).

## How

### Phase 1: Build (Critical — short-circuits on failure)

```bash
dotnet build --no-restore --verbosity quiet
```

- If the build fails, STOP. Report errors and fix them before continuing.
- Output: PASS (0 errors) or FAIL (with error list)

### Phase 2: Diagnostics (Critical — short-circuits on failure)

Check for compiler warnings in the build output:
- Nullable reference warnings
- Unused variables or imports
- Any new warnings introduced by the current changes

Output: PASS (0 new warnings) or FAIL (with diagnostic list)

### Phase 3: Antipattern Detection

Scan modified files for:
- `async void` methods (except event handlers)
- Sync-over-async (`.Result`, `.GetAwaiter().GetResult()`)
- `DateTime.Now` instead of `DateTime.Today` / `DateTime.UtcNow` where appropriate
- Broad `catch (Exception)` without specific handling
- String interpolation in logging (`logger.LogInformation($"...")`)
- EF Core queries without `AsNoTracking()` for read-only scenarios (in `*Negocio.cs`)
- Missing null checks on navigation properties

Output: PASS (0 antipatterns) or WARN (with findings) or FAIL (critical antipatterns)

### Phase 4: Tests

```bash
dotnet test --no-build --verbosity quiet
```

- Skip this phase if no test project exists in the solution (note it as SKIP)
- Any failing test is a FAIL

Output: PASS / FAIL / SKIP

### Phase 5: Security Scan

Review the changes for:
- Hardcoded connection strings, passwords, or API keys
- `ExecuteSqlRawAsync` / `FromSqlRaw` with string concatenation (SQL injection)
- Missing `[Authorize]` attributes on controllers or actions that should be protected
- Missing `[ValidateAntiForgeryToken]` on POST actions
- Sensitive data in logs

Output: PASS (no findings) or WARN/FAIL (with findings by severity)

### Phase 6: Format Check

```bash
dotnet format --verify-no-changes --verbosity quiet
```

Output: PASS (no formatting issues) or WARN (with file list)

### Phase 7: Diff Review

Analyze `git diff` (staged and unstaged) for:
- Accidental file changes (unrelated modifications)
- Debug leftovers (`Console.WriteLine`, commented-out code)
- TODO/HACK/FIXME markers that should be resolved before merge
- Sensitive files modified (`appsettings.json` with real credentials)

Output: PASS (clean diff) or WARN (with findings)

### Final Summary

After all phases complete, output a summary table:

```
## Verification Results

| Phase | Result | Details |
|-------|--------|---------|
| 1. Build      | PASS | 0 errors |
| 2. Diagnostics | PASS | 0 new warnings |
| 3. Antipatterns | WARN | 1 query sin AsNoTracking |
| 4. Tests      | SKIP | No test project |
| 5. Security   | PASS | No findings |
| 6. Format     | PASS | Clean |
| 7. Diff Review | PASS | Clean |

**Verdict: READY FOR REVIEW** (1 non-blocking warning)
```

Verdicts:
- **READY FOR REVIEW** — All phases PASS or only non-blocking WARNs
- **NEEDS FIXES** — Any phase FAIL, with specific remediation steps

## Related

- `/build-fix` — Auto-fix build errors when Phase 1 fails
- `/security-scan` — Focused deep security review (beyond Phase 5 basics)

$ARGUMENTS
