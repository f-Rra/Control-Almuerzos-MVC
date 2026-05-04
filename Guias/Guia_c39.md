# Guía Commit 39 — Inicialización de CLAUDE.md y permisos

## Commit
`ade234b` · `docs: inicializar CLAUDE.md y configurar permisos`

---

## Contexto

`CLAUDE.md` es el archivo de instrucciones persistentes de Claude Code: todo lo que está en él se inyecta automáticamente en cada sesión. Inicializarlo formalmente marca un punto de inflexión en la metodología: en lugar de repetir el contexto del proyecto en cada conversación, Claude Code lo tiene disponible desde el arranque.

---

## Qué se hizo

### `SCA-MVC/CLAUDE.md`
Creado con tres secciones principales:

1. **Comandos de desarrollo**: `dotnet build`, `dotnet run`, `dotnet ef` — los comandos que Claude Code tiene permiso de ejecutar.
2. **Arquitectura**: descripción de capas (Controllers, Services/Interfaces, ViewModels, Views), stack tecnológico y decisiones clave (EF Core sobre ADO.NET, Bootstrap 5, QuestPDF).
3. **Decisiones técnicas documentadas**: por qué se eligió cada tecnología, qué patrones se siguen, qué anti-patrones evitar.

### `.claude/settings.json`
Configurada la `allowlist` de permisos para comandos de dotnet build y migraciones EF Core, evitando que Claude Code pida confirmación manual en cada ejecución de estos comandos.

---

## Por qué CLAUDE.md cambia la dinámica de trabajo

Sin `CLAUDE.md`, cada sesión empieza desde cero. Claude no sabe que el proyecto usa Bootstrap Icons (no Font Awesome), que las queries deben tener `AsNoTracking`, o que el formulario de empleados sigue un layout de 2 columnas. Con `CLAUDE.md`, ese conocimiento es persistente y no se pierde entre conversaciones.

---

## Herramientas de IA utilizadas

Se usó el skill `/init` de Claude Code (incluido por defecto) para generar el esqueleto inicial del CLAUDE.md, que luego fue completado manualmente con las decisiones específicas del proyecto.
