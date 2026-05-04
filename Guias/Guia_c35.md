# Guía Commit 35 — Actualización del README principal

## Commit
`701fb7d` · `docs: actualizar documentación principal del repositorio`

---

## Contexto

Con la aplicación funcionando end-to-end, el README original era demasiado escueto para representar el proyecto. En esta etapa el foco pasó del código a la documentación: que cualquier persona que visite el repositorio (un recruiter, un líder técnico) entienda qué es el sistema, cómo se despliega y cuál es su arquitectura sin tener que leer el código.

---

## Qué se hizo

| Sección agregada | Descripción |
|---|---|
| Stack tecnológico | ASP.NET Core MVC, EF Core, SQL Server, Bootstrap 5, Bootstrap Icons, QuestPDF, MailKit |
| Usuarios predefinidos | Tabla con credenciales de demo para Admin y Usuario estándar |
| Seeding de datos | Descripción del proceso de migración inicial con datos cargados |
| Despliegue local | Paso a paso para clonar, configurar connection string y correr migraciones |
| Esquema de arquitectura | Texto descriptivo de capas: Controllers → Services → EF Core → SQL Server |

---

## Herramientas de IA utilizadas

Este commit fue el primero en aprovechar **Claude Code** de forma directa para redactar documentación técnica. El proceso fue colaborativo: el autor proporcionó los datos reales del sistema (stack, usuarios, estructura) y Claude los formateó en markdown estructurado. No se usaron agents ni skills específicos.
