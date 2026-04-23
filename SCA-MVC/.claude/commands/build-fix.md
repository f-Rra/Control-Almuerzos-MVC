---
description: >
  Autonomous loop that iteratively resolves dotnet build errors: build → parse errors →
  categorize → fix → rebuild, up to 5 iterations. Invoke when the build is broken,
  after a major refactor, after NuGet updates, or when the user says "fix the build",
  "make it compile", "build errors".
---

# /build-fix

## What

Autonomous loop that resolves `dotnet build` compilation errors iteratively.
Parses compiler errors, categorizes them by root cause, applies targeted fixes,
and rebuilds until the build is green or the iteration limit (5) is reached.

Designed for: post-refactor breakage, NuGet major version upgrades, merged conflicts
that introduced type mismatches, and cascading errors from a single root cause.

## When

- Multiple compiler errors are blocking the build
- After a major refactor that altered types, namespaces, or method signatures
- After NuGet package updates that introduced breaking changes
- After resolving merge conflicts that haven't been compiled yet
- User says: "fix the build", "make it compile", "build is broken"

## How

### Iteration Loop (max 5 cycles)

**Each cycle:**

1. **Build & Parse**
```bash
dotnet build --verbosity quiet 2>&1
```
Extract all error codes (CS0xxx) with file paths and line numbers.

2. **Categorize** — group by root cause:
   - **Missing references** — `CS0246`, `CS0234`: unknown type or namespace → check usings, missing NuGet
   - **Type mismatches** — `CS0029`, `CS1503`: wrong type passed → check method signatures
   - **API changes** — `CS1061`: member doesn't exist → method renamed or moved in updated package
   - **Nullability violations** — `CS8600`–`CS8629`: nullable reference warnings promoted to errors
   - **Ambiguous references** — `CS0104`: two types with same name → add explicit namespace
   - **Unimplemented members** — `CS0535`: interface not fully implemented

3. **Fix** — prioritize root-cause errors that will resolve cascading failures:
   - Fix missing `using` statements before fixing downstream type errors
   - Fix interface implementations before fixing call sites
   - Fix one category at a time; don't scatter fixes

4. **Rebuild & Evaluate**
   - If error count decreased: continue loop
   - If error count unchanged after 2 consecutive cycles: STOP and report remaining errors
   - If build succeeds: exit loop

### Exit Conditions

- ✅ Build succeeds → run `dotnet build` one final time to confirm, report success
- ❌ Iteration limit reached → report remaining errors with recommended manual steps
- ❌ No progress detected → report the specific errors that need manual investigation

### Final Check (on success)

```bash
dotnet build --verbosity quiet
```

## Example

```
User: /build-fix

Claude: Build attempt 1/5...
  12 errors found. Root causes:
  - CS0246 x3: missing 'using SCA_MVC.ViewModels'
  - CS1061 x8: 'Empleado' does not contain 'FullName' (renamed to 'NombreCompleto')
  - CS0535 x1: 'EmpleadoNegocio' doesn't implement 'IEmpleadoNegocio.BuscarPorNombreAsync'

  Fixing: adding missing using, renaming FullName → NombreCompleto in 4 files,
          adding stub implementation for BuscarPorNombreAsync...

  Build attempt 2/5...
  0 errors. Build: green ✅
```

## Related

- `/verify` — Full 7-phase pipeline once the build is green
- `/migrate` — If build errors are caused by pending EF Core model changes

$ARGUMENTS
