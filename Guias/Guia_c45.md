# Guía Commit 45 — Fecha/hora en tiempo real en topbar y footer personalizado

## Commit
`f712385` · `feat: mejorar layout global con fecha/hora en topbar y footer personalizado`

---

## Contexto

En el uso real del sistema en el comedor, el operador no tiene el celular a mano mientras registra comensales. Mostrar la fecha y hora directamente en la topbar elimina la necesidad de mirar otro dispositivo para saber la hora actual del servicio. También se agregó un footer que identifica el sistema con versión y año.

---

## Cambios en `Views/Shared/_Layout.cshtml`

### Fecha y hora en tiempo real

Se agregó en la topbar un bloque con dos íconos Bootstrap Icons (`bi-calendar3` y `bi-clock`) acompañados de spans actualizados por JavaScript:

```javascript
function actualizarReloj() {
    const ahora = new Date();
    const dias = ['Domingo','Lunes','Martes','Miércoles','Jueves','Viernes','Sábado'];
    const meses = ['Enero','Febrero','Marzo','Abril','Mayo','Junio',
                   'Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];

    document.getElementById('fecha-display').textContent =
        `${dias[ahora.getDay()]} ${ahora.getDate()} de ${meses[ahora.getMonth()]}`;

    const h = String(ahora.getHours()).padStart(2, '0');
    const m = String(ahora.getMinutes()).padStart(2, '0');
    const s = String(ahora.getSeconds()).padStart(2, '0');
    document.getElementById('hora-display').textContent = `${h}:${m}:${s}`;
}
setInterval(actualizarReloj, 1000);
actualizarReloj();
```

El reloj actualiza cada 1 segundo con `setInterval`. Se llama también de forma inmediata para evitar el flash de texto vacío al cargar la página.

### Footer

```html
<footer class="app-footer">
    Sistema Control de Almuerzos &nbsp;·&nbsp; v1.0 &nbsp;·&nbsp; 2026
</footer>
```

Simple, sin links. Identifica el sistema con nombre, versión y año.

---

## Decisión de diseño: JavaScript inline vs. `site.js`

El código del reloj se colocó en `_Layout.cshtml` directamente y no en `site.js`. Razón: el reloj es parte del layout global y no tiene dependencias con otras vistas ni módulos. Moverlo a `site.js` crearía acoplamiento innecesario entre el layout y el bundle de scripts.

---

## Herramientas de IA utilizadas

Claude Code (modo interactivo) propuso la implementación del reloj incluyendo el manejo correcto de `padStart` para los segundos. El autor indicó el formato de fecha deseado (día de la semana + día + mes en español) y Claude generó el array de días y meses en castellano.
