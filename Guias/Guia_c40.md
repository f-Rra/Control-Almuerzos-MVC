# Guía Commit 40 — Instalación de skills dotnet-claude-kit

## Commit
`8fe6ef7` · `feat: instalar skills de dotnet-claude-kit`

---

## Contexto

Además de los skills personalizados creados en el commit anterior, se incorporaron skills de propósito general del ecosistema de Claude Code para proyectos .NET. Estos skills cubren flujos de trabajo completos que serían costosos de describir desde cero en cada sesión.

---

## Skills instalados

| Skill | Propósito |
|---|---|
| `/migrate` | Workflow seguro para migraciones EF Core: genera, revisa el SQL, aplica y verifica. Evita migraciones rotas en producción. |
| `/verify` | Pipeline de 7 fases antes de hacer un commit: build limpio, anti-patrones, seguridad, diff de cambios. |
| `/security-scan` | Auditoría OWASP completa en 6 dimensiones: autenticación, autorización, inyección, dependencias, configuración, datos sensibles. |
| `/build-fix` | Loop autónomo: Claude Code intenta compilar, lee el error, aplica el fix, reintenta. |
| `/ef-core` | Skill de referencia con patrones modernos de EF Core: AsNoTracking, proyecciones, relaciones, evitar N+1. |

---

## Decisión: skills externos vs. skills propios

La diferencia entre estos skills y los del commit anterior es el origen:
- **Skills propios (c38):** escritos desde cero para este proyecto, con conocimiento específico de las vistas, modelos y convenciones.
- **Skills externos (este commit):** frameworks de trabajo genéricos para .NET que son útiles en cualquier proyecto con EF Core y ASP.NET Core.

Ambos coexisten en `.claude/commands/` y se complementan.

---

## Impacto en el flujo de desarrollo

A partir de este commit, el flujo de trabajo para tareas complejas pasó a ser:

1. Elegir el skill adecuado (`/security-scan`, `/migrate`, etc.)
2. Claude Code ejecuta el workflow completo con múltiples herramientas
3. El desarrollador revisa el resultado y aprueba o ajusta

Esto redujo drásticamente el tiempo de setup de cada sesión y garantizó consistencia en tareas críticas como migraciones y security reviews.

---

## Herramientas de IA utilizadas

Todos los skills fueron instalados mediante Claude Code. El skill `/security-scan` fue ejecutado inmediatamente después de su instalación para validar el estado de seguridad del proyecto (resultados documentados en el commit siguiente, c42).
