# Guía Commit 37 — Metodología de IA y sincronización de guía inicial

## Commit
`237b647` · `docs: actualizar metodología de IA y depurar guía inicial`

---

## Contexto

Con el avance del proyecto quedó claro que la sección "Herramientas" del README original no reflejaba con precisión cómo se usó la IA. La narrativa inicial era genérica. Este commit la reemplaza con una descripción honesta del modelo de trabajo real: *Agentic Coding*, donde el desarrollador actúa como supervisor de arquitectura y la IA ejecuta tareas acotadas bajo instrucciones precisas.

También se sincronizó la `Guia_Inicial.md` para que sus pasos coincidan 1:1 con el historial de git real, eliminando referencias a pasos que nunca ocurrieron o que ocurrieron en un orden distinto.

---

## Qué se hizo

| Cambio | Descripción |
|---|---|
| README — sección Herramientas | Reemplazada por descripción del modelo Agentic Coding |
| `Guia_Inicial.md` | Pasos sincronizados con el historial de commits real |
| Bloques de mensajes redundantes | Eliminados de la guía inicial para reducir ruido |

---

## Sobre Agentic Coding

El término describe un flujo donde el desarrollador no escribe código línea por línea sino que:
1. Define el problema y el contexto
2. Le da instrucciones precisas a la IA
3. Revisa el output y decide si se acepta, ajusta o descarta
4. Itera hasta el resultado correcto

La IA actúa como un par de programación que ejecuta; el desarrollador mantiene la visión arquitectónica y el criterio de calidad.

---

## Herramientas de IA utilizadas

Este commit fue de edición pura. Claude Code leyó el estado actual de los archivos, identificó inconsistencias entre la documentación y el historial de git, y propuso los cambios. El autor validó cada modificación antes de aceptarla.
