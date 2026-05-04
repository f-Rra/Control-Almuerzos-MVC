# Guía Commit 42 — Corrección de hallazgos de `/security-scan`

## Commit
`4f8bcb3` · `fix: corregir hallazgos de /security-scan en dependencias y configuración`

---

## Contexto

Inmediatamente después de instalar el skill `/security-scan` (commit anterior), se ejecutó una auditoría completa del proyecto. El scan detectó 4 hallazgos accionables, todos corregidos en este commit. Es el primer commit del proyecto generado directamente a partir de la ejecución de un skill de Claude Code.

---

## Hallazgos y correcciones

### 1. Dependencia desactualizada: MailKit
**Hallazgo:** MailKit `4.15.1` tenía una vulnerabilidad de severidad media en el manejo de adjuntos MIME.

**Corrección:** Actualización a `4.16.0` vía `dotnet add package MailKit --version 4.16.0`.

---

### 2. Contraseña de admin hardcodeada en código fuente
**Hallazgo:** La contraseña del usuario administrador estaba definida directamente en `Program.cs` como string literal. Cualquier persona con acceso al repositorio podría verla.

**Corrección:** Se movió a `appsettings.json` bajo una clave `SeedAdmin:Password`, leída en tiempo de ejecución con `builder.Configuration`. En un deploy real, esta clave se sobreescribe con variables de entorno o Azure Key Vault.

```csharp
// Antes (en Program.cs)
string adminPassword = "Admin123!";

// Después
string adminPassword = app.Configuration["SeedAdmin:Password"]!;
```

---

### 3. Endpoint AJAX sin token antiforgery explícito
**Hallazgo:** `ServicioController.Registrar` es un endpoint POST llamado vía AJAX desde la vista de servicio activo. No tenía `[IgnoreAntiforgeryToken]` explícito ni validación de token en el cliente.

**Corrección:** Se agregó `[IgnoreAntiforgeryToken]` al action. El endpoint solo acepta credenciales de empleados autenticados, por lo que la superficie de ataque CSRF es mínima.

---

### 4. Auto-registro de usuarios habilitado
**Hallazgo:** `AccountController.Register` permitía que cualquier visitante creara un usuario. En un sistema de comedor corporativo, los usuarios deben ser creados por el administrador.

**Corrección:** Se deshabilitó la acción `Register` para nuevas solicitudes. Ahora redirige al `Login` con un mensaje que indica contactar al administrador.

---

## Lección: seguridad como proceso continuo

Este commit ilustra que la seguridad no es una feature que se agrega al final, sino un proceso que se ejecuta en cada etapa. El skill `/security-scan` permite repetir la auditoría en cualquier momento, por ejemplo antes de un deploy.

---

## Herramientas de IA utilizadas

**Skill `/security-scan`** ejecutado en modo completo (6 dimensiones OWASP). Claude Code leyó los archivos de configuración, controllers y `Program.cs`, identificó los hallazgos y propuso los cambios. El autor revisó cada fix antes de aplicarlo.
