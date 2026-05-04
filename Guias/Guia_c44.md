# Guía Commit 44 — Consistencia visual y corrección de bugs de frontend

## Commit
`3bf83b0` · `feat: mejorar consistencia visual y corregir bugs de frontend`

---

## Contexto

El skill `/frontend-design` detectó inconsistencias en el CSS: colores definidos como valores hexadecimales inline repetidos en múltiples archivos, y un bloque de estilo duplicado entre el HTML y el JavaScript de una vista. Este commit los resuelve y además mejora el flujo del Login.

---

## Cambios aplicados

### 1. Variables CSS `--green` y `--red` en `:root`

**Antes:** El color de éxito (`#2ecc71`) y el color de error (`#e74c3c`) aparecían hardcodeados en múltiples reglas de `site.css` y en estilos inline dentro de las vistas.

**Después:** Definidos como custom properties en `:root`:

```css
:root {
    --green: #2ecc71;
    --red:   #e74c3c;
}
```

Ahora cualquier cambio de color se hace en un solo lugar. Las vistas y el JS los referencian con `var(--green)` y `var(--red)`.

---

### 2. Eliminación de estilos duplicados en `det-icon-wrap`

El componente de ícono de detalle en la vista de Home tenía su estilo definido dos veces: una en el `<style>` del HTML y otra seteada por JavaScript al momento de renderizar. El JS sobreescribía el CSS, pero el bloque CSS seguía existiendo sin efecto. Se eliminó el bloque CSS redundante y el JS quedó como única fuente de verdad.

---

### 3. Login: reemplazar link de registro por texto informativo

El link "¿No tenés cuenta? Registrate" apuntaba a la acción `Register` que ya había sido deshabilitada en el commit c42 (security scan). En lugar de dejarlo roto o eliminarlo, se reemplazó por un texto estático:

> *"Para acceder al sistema contactá al administrador."*

Esto mejora la experiencia del usuario que llega al login sin credenciales: entiende el proceso sin encontrar un error 404.

---

## Herramientas de IA utilizadas

**Skill `/frontend-design`** ejecutado sobre `site.css` y las vistas principales. Claude Code identificó los valores hardcodeados y los estilos duplicados mediante análisis estático del CSS. El autor aprobó cada cambio y verificó visualmente que las vistas mantuvieran el mismo aspecto.
