# Guía Commit 51 — Limpieza final del repositorio

## Commit
`e27518b` · `chore: eliminar archivos no utilizados y proyecto original WinForms`

---

## Contexto

Antes de continuar con la documentación de deploy y el README final, se realizó una limpieza del repositorio para que refleje con exactitud el estado del proyecto. Un repositorio limpio transmite orden y criterio al evaluador técnico que lo revise.

---

## Qué se eliminó

### Archivos trackeados en git (eliminados con `git rm`)

| Archivo | Motivo |
|---|---|
| `git_log.txt` | Archivo temporal generado para análisis interno, sin valor en el repositorio |
| `SCA-MVC/wwwroot/login-disenos.html` | Mockup HTML estático usado durante el diseño del Login; sin relación con el sistema en producción |
| `SCA-MVC/wwwroot/notificaciones-preview.html` | Preview HTML de notificaciones generado durante la exploración de UX; reemplazado por la implementación real |

### Carpeta no trackeada (eliminada del disco)

| Carpeta | Motivo |
|---|---|
| `Sistema-Control-Almuerzos/` | Proyecto WinForms original (C#, Windows Forms). No forma parte del repositorio MVC. Al abrir VS Code en la carpeta raíz del repo, aparecía como carpeta no trackeada generando ruido. |

---

## Por qué el proyecto WinForms no está en este repositorio

El sistema original (`Sistema-Control-Almuerzos`) tiene su propio repositorio separado. Mantenerlo como subcarpeta del repo MVC crearía confusión sobre el alcance del proyecto. El README del repo MVC referencia al repo original con su URL.

---

## Criterio para eliminar archivos

Un archivo debe eliminarse del repositorio cuando:
1. No es referenciado por ningún archivo de código o vista activa.
2. No tiene valor documental para entender decisiones pasadas.
3. Su presencia podría confundir a alguien que revisa el código.

Los mockups HTML cumplen los tres criterios: las decisiones de diseño que representaban ya están implementadas en las vistas Razor finales.

---

## Herramientas de IA utilizadas

Claude Code revisó el árbol de archivos del repositorio con `git ls-files` e identificó los tres archivos trackeados sin referencias activas. La decisión de eliminar la carpeta `Sistema-Control-Almuerzos/` fue del autor; Claude Code ejecutó la limpieza y verificó el estado del repositorio con `git status` antes de staged los cambios.
