# Guía Commit 38 — Skills personalizados de Claude Code

## Commit
`6c9918a` · `feat: agregar skills personalizados de Claude al proyecto`

---

## Contexto

A medida que el proyecto creció en complejidad, las instrucciones repetitivas en cada sesión de Claude Code empezaron a ser ineficientes. Los *skills* (comandos personalizados de Claude Code) permiten encapsular instrucciones complejas en un comando slash reutilizable. Este commit agrega cuatro skills específicos para las tareas más frecuentes del proyecto.

---

## Skills agregados

| Skill | Archivo | Propósito |
|---|---|---|
| `/frontend-design` | `.claude/commands/frontend-design.md` | Auditoría y mejora de vistas Razor con Bootstrap 5, consistencia visual, responsive |
| `/db-review` | `.claude/commands/db-review.md` | Análisis de queries EF Core, relaciones e índices, detección de N+1 |
| `/report-generator` | `.claude/commands/report-generator.md` | Guía para reportes PDF con QuestPDF, estructura de datos y exportación |
| `/seed-data` | `.claude/commands/seed-data.md` | Generación y actualización de datos de prueba realistas en migraciones EF Core |

---

## Cómo funcionan los skills en Claude Code

Un skill es un archivo `.md` dentro de `.claude/commands/`. Al escribir `/nombre-del-skill` en el chat de Claude Code, el contenido del archivo se inyecta automáticamente como contexto en la conversación. Esto permite:

- **Consistencia:** Claude sigue siempre las mismas reglas para una tarea dada.
- **Reutilización:** no hay que repetir el contexto en cada sesión.
- **Especialización:** cada skill puede incluir patrones aceptados, anti-patrones a evitar, y ejemplos de código del proyecto.

---

## Por qué es relevante para el portfolio

La incorporación de skills demuestra un uso avanzado de las herramientas de IA, más allá del "preguntarle cosas a ChatGPT". Configura a Claude Code como un asistente especializado en este proyecto específico, con conocimiento del stack y las convenciones establecidas.

---

## Herramientas de IA utilizadas

Claude Code (modo interactivo) asistió en la redacción del contenido de cada skill, basándose en los patrones ya presentes en el código. El autor definió el scope de cada skill y validó que las instrucciones fueran precisas y acotadas.
