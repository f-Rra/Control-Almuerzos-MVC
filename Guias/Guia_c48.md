# Guía Commit 48 — Animaciones táctiles y feedback visual para tablet

## Commit
`2dc14f6` · `feat: agregar animaciones táctiles y feedback visual para tablet`

---

## Contexto

En una tablet táctil, el usuario no recibe el feedback visual que da el cursor del mouse (hover, cursor pointer, estados de foco). Al registrar un comensal por credencial, la única confirmación visible era un mensaje de texto. En un entorno de comedor con ruido y apuro, el operador necesita confirmación visual inmediata y sin ambigüedad. Este commit agrega micro-interacciones específicas para uso táctil.

---

## Cambios aplicados

### 1. `:active` states en botones y cards

CSS puro, sin JavaScript. Los estados `:active` se disparan en el momento exacto del toque y dan feedback inmediato antes de que el servidor responda:

```css
.btn-primary:active,
.srv-card:active {
    transform: scale(0.97);
    opacity: 0.85;
    transition: transform 0.08s, opacity 0.08s;
}
```

La transición de 80ms es suficientemente rápida para parecer instantánea al tacto.

---

### 2. Flash verde en nueva fila al registrar un comensal

Cuando se registra un comensal por credencial, la nueva fila se agrega dinámicamente a la tabla vía JavaScript. Se le aplica una clase `.fila-nueva` que dispara una animación CSS de fondo verde que desaparece en 1.5 segundos:

```css
@keyframes flashVerde {
    0%   { background: rgba(46, 204, 113, 0.4); }
    100% { background: transparent; }
}
.fila-nueva { animation: flashVerde 1.5s ease-out forwards; }
```

---

### 3. `animarContador()`: count-up animado

Los contadores "Registrados" y "Faltan" no saltan directamente al nuevo número. La función `animarContador(elemento, inicio, fin, duracion)` incrementa el número suavemente usando `requestAnimationFrame`:

```javascript
function animarContador(el, inicio, fin, duracion) {
    const startTime = performance.now();
    function step(now) {
        const t = Math.min((now - startTime) / duracion, 1);
        el.textContent = Math.round(inicio + (fin - inicio) * t);
        if (t < 1) requestAnimationFrame(step);
    }
    requestAnimationFrame(step);
}
```

Usa `performance.now()` para precisión de milisegundos y `requestAnimationFrame` para sincronizar con el ciclo de render del navegador.

---

### 4. `numPop`: pulso visual en el número al actualizarse

Complementario al count-up: cuando el número termina de animarse, se agrega brevemente la clase `.num-pop` que escala el elemento a 1.15x y vuelve a 1x en 200ms. Refuerza visualmente que el dato cambió.

---

## Decisión: animaciones CSS vs. JavaScript

Las animaciones de feedback táctil (`:active`, flash de fila) se implementaron en CSS puro. Las animaciones de datos (count-up, numPop) en JavaScript, porque dependen de valores dinámicos. Esta separación mantiene el CSS declarativo y el JS enfocado en lógica de datos.

---

## Herramientas de IA utilizadas

Claude Code (modo agente con acceso a la vista de Servicio) implementó las 4 micro-interacciones en una operación. El autor definió el comportamiento esperado para cada una y aprobó la implementación tras verificarla en el navegador de la tablet.
