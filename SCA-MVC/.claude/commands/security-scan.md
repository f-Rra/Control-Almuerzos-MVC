---
description: >
  Comprehensive security audit across 6 dimensions: vulnerable dependencies,
  hardcoded secrets, OWASP Top 10 patterns, auth/authorization configuration,
  CORS policy, and data protection. Invoke before production deploys, after
  adding auth/payment features, or when the user says "security scan",
  "check for vulnerabilities", "is this secure?".
---

# /security-scan

## What

Comprehensive security audit that scans the solution across six dimensions:

1. **Vulnerable dependencies** — NuGet packages with known CVEs
2. **Secrets detection** — Hardcoded connection strings, API keys, tokens in source
3. **OWASP Top 10 patterns** — Injection, broken auth, sensitive data exposure, security misconfiguration, and more
4. **Authentication and authorization** — Missing `[Authorize]` attributes, weak Identity configuration, insecure cookie settings
5. **CORS policy** — Overly permissive origins, missing restrictions
6. **Data protection** — PII in logs, unencrypted sensitive data, missing input validation, anti-forgery tokens

The output is a structured security report with findings ranked by severity (Critical, High, Medium, Low) and actionable remediation steps for each finding.

## When

- Before deploying to production or staging
- During a security review
- After adding authentication, authorization, or email features
- After adding new controllers or actions that handle sensitive data
- After updating NuGet packages
- User says: "security scan", "check for vulnerabilities", "is this secure?"

## How

### Phase 1: Vulnerable Dependencies

```bash
dotnet list package --vulnerable
dotnet list package --deprecated
```

Flag any package with a known CVE. Report severity, CVE ID, and the fixed version.

### Phase 2: Secrets Detection

Scan source files for patterns that indicate hardcoded secrets:
- Connection strings with passwords in `appsettings.json` or `.cs` files
- SMTP credentials (`SmtpPass`, `SmtpUser`) with real values instead of placeholders
- API keys, tokens, or credentials in source code
- `.env` files tracked in git

Search for patterns: `password=`, `SmtpPass`, `secret`, `apikey` with literal values (not placeholders or environment variable references).

### Phase 3: OWASP Top 10 Patterns

Check for:
- **A01 Broken Access Control** — Controllers or actions missing `[Authorize]`, insecure direct object references (accessing records by ID without ownership check)
- **A02 Cryptographic Failures** — Weak password hashing, unencrypted sensitive data
- **A03 Injection** — `ExecuteSqlRawAsync` / `FromSqlRaw` with string concatenation instead of parameterized queries
- **A04 Insecure Design** — Missing input validation on public endpoints, no rate limiting on login
- **A05 Security Misconfiguration** — Debug mode in production config, default credentials left in code
- **A07 Auth Failures** — Lockout not configured, weak password policy, missing `RequireUniqueEmail`
- **A08 Data Integrity** — Missing `[ValidateAntiForgeryToken]` on POST/PUT/DELETE actions
- **A09 Logging Failures** — PII (email, nombre, credencial) logged at Information level
- **A10 SSRF** — Unvalidated URLs in outbound HTTP calls

### Phase 4: Auth Configuration Review

Review `Program.cs` Identity configuration:
- Password policy strength (length, complexity)
- Lockout settings (attempts, duration)
- Cookie expiration (`ExpireTimeSpan`, `SlidingExpiration`)
- `RequireConfirmedAccount` setting and its implications
- `[Authorize]` coverage across all controllers
- `[Authorize(Roles = "Admin")]` on admin-only actions

### Phase 5: CORS Policy

Check if CORS is configured and origins are explicitly listed (no `AllowAnyOrigin`).

### Phase 6: Data Protection

- Check for PII (email, nombre, credencial de empleado) in log statements
- Verify `[PersonalData]` attribute on sensitive `ApplicationUser` properties
- Confirm `UseHttpsRedirection()` is present in `Program.cs`
- Verify `[ValidateAntiForgeryToken]` on all form POST actions

### Report Format

```markdown
## Security Scan Report

### Summary
[X findings: N critical, N high, N medium, N low]

### Critical
- **[Category] [Title]** — [File:Line] [Description]. [Remediation].

### High / Medium / Low
- ...

### Passed Checks
- [List of security checks that passed]
```

## Example

```
User: /security-scan

Claude: Running security scan across 6 dimensions...

  Phase 1: Vulnerable Dependencies — PASS (no CVEs found)

  Phase 2: Secrets Detection
  - HIGH: appsettings.json contiene SmtpPass con valor real.
    Mover a user-secrets o variable de entorno.

  Phase 3: OWASP Top 10
  - MEDIUM: No hay rate limiting en /Account/Login.

  Phase 4: Auth Configuration — PASS

  Phase 5: CORS — N/A (no CORS configurado, app solo server-rendered)

  Phase 6: Data Protection
  - LOW: EmailService.cs:34 loguea dirección de email a nivel Information.

  Summary: 2 findings (1 high, 1 low)
```

## Related

- `/verify` — General verification pass including a basic security dimension
- `/security-review` — Revisión de seguridad integrada de Anthropic

$ARGUMENTS
