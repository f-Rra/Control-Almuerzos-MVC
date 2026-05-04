# Guía Commit 41 — Principios de Karpathy y registro de plugins en CLAUDE.md

## Commit
`2d4e8b1` · `docs: agregar principios de Karpathy y registro de plugins al CLAUDE.md`

---

## Contexto

Andrej Karpathy (ex-Director de IA de Tesla, investigador de OpenAI) publicó una serie de principios sobre cómo trabajar de forma efectiva con modelos de lenguaje en contextos de ingeniería de software. Estos principios se incorporaron a `CLAUDE.md` para guiar el comportamiento de Claude Code en este proyecto.

---

## Principios incorporados

Los principios se derivaron del skill `andrej-karpathy-skills` y se adaptaron al contexto del proyecto:

| Principio | Aplicación en el proyecto |
|---|---|
| **Pensar antes de codificar** | Claude Code debe analizar el impacto de un cambio antes de editar archivos |
| **Simplicidad sobre abstracción** | No crear clases, interfaces o métodos si un bloque inline resuelve el problema |
| **Cambios quirúrgicos** | Modificar solo lo necesario; no refactorizar código que no está en scope |
| **Ejecución orientada a metas** | Cada tarea tiene un objetivo claro; si algo no contribuye al objetivo, no se hace |

---

## Registro de plugins

También se agregó a `CLAUDE.md` una tabla con todos los skills instalados, su origen y propósito. Esto sirve como referencia rápida para saber qué herramientas están disponibles sin tener que explorar la carpeta `.claude/commands/` manualmente.

---

## Por qué documentar principios de trabajo en el repositorio

En un equipo real, los principios de trabajo con IA deben ser explícitos para que todos los miembros usen las herramientas de la misma manera. En el contexto de portfolio, demuestra que el uso de IA no fue improvisado sino que siguió un framework deliberado.

---

## Herramientas de IA utilizadas

Claude Code (con acceso a CLAUDE.md) generó la tabla de plugins leyendo el contenido de `.claude/commands/`. Los principios de Karpathy fueron redactados por el autor basándose en material original y adaptados al contexto del proyecto.
