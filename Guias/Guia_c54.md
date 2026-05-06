# Guía Commit 54 — Sistema de temas Solar/Marino

## Commit
`feat: agregar sistema de temas Solar/Marino con toggle en sidebar`

---

## Contexto

El sistema iba a presentarse en dos empresas distintas: Laboratorios Roemmers y Grupo Southex. Cada una tiene su paleta de colores corporativa. En lugar de mantener dos versiones del proyecto, se implementó un sistema de temas intercambiables al estilo dark/light mode, con persistencia en `localStorage` y sin tocar el backend.

---

## Paletas definidas

| Variable | Solar (Roemmers) | Marino (Southex) |
|---|---|---|
| `--primary` | `#FFC107` (amarillo) | `#0076B6` (azul) |
| `--primary-dark` | `#e6a800` | `#004080` |
| `--accent` | `#FFD54F` | `#E6332A` (rojo) |
| `--accent-rgb` | — | `230, 51, 42` |
| `--value-color` | `#B8860B` | `var(--primary)` |
| `--bg-base` | `#FDF6E3` (crema) | `#F5F9FC` (azul claro) |
| `--icon-active` | `#a07800` | `#004f8c` |

---

## Arquitectura del sistema de temas

### CSS custom properties en cascada

El tema Solar vive en `:root`. El tema Marino sobreescribe las mismas variables en `[data-theme="marino"]` aplicado al elemento `<html>`. Todo el CSS del sistema usa `var()` — cambiar el atributo del `<html>` redefine la apariencia completa sin recargar la página.

```css
/* :root define Solar */
:root { --primary: #FFC107; }

/* [data-theme="marino"] redefine Marino */
[data-theme="marino"] { --primary: #0076B6; }
```

### Variables de RGB para opacidades

Se usó el patrón `--primary-rgb: R, G, B` para poder usar el color primario con cualquier opacidad sin una variable extra por cada nivel:

```css
background: rgba(var(--primary-rgb), 0.15);
```

Esto cubre los ~40 usos de `rgba(255, 193, 7, x)` del Solar con un solo override en Marino.

### Script anti-parpadeo

El atributo `data-theme` se aplica en el `<head>` antes del render del body, evitando el flash de Solar antes de que cargue el JS:

```html
<script>(function(){
    var t = localStorage.getItem('sca-theme');
    if(t) document.documentElement.setAttribute('data-theme', t);
})();</script>
```

Se agregó a `_Layout.cshtml`, `Login.cshtml` y `AccessDenied.cshtml` (páginas standalone sin Layout).

---

## Toggle en el sidebar

Botón `bi-palette2` al fondo del sidebar, usando la misma estructura de ítems de navegación pero sin `href`. Persiste en `localStorage` como `sca-theme`.

---

## Limpieza del CSS en el proceso

Aprovechando el refactor del tema se realizaron mejoras estructurales al `site.css`:
- `#4CAF50` → `var(--green)` y `#F44336` → `var(--red)` en ~50 lugares
- `rgba(255, 193, 7, x)` → `rgba(var(--primary-rgb), x)` en ~40 lugares
- Variable circular `--value-color` corregida
- Override huérfano de `.usr-badge.usr-admin` movido al bloque Marino
- 18 comentarios obvios eliminados

---

## Herramientas de IA utilizadas

Claude Code implementó el sistema de temas en múltiples iteraciones con feedback visual del usuario en cada paso. El proceso requirió ~8 ciclos de revisión de screenshots para cubrir todos los componentes: botones, modales, tablas, páginas standalone, barras de progreso y la notificación de registro.
