# Guía Commit 46 — Vista de servicio inactivo y eliminación de código muerto

## Commit
`60e8fd6` · `feat: mejorar vista de servicio inactivo y eliminar código muerto`

---

## Contexto

Cuando no hay un servicio activo, la vista `Servicio/Index.cshtml` mostraba un contenedor vacío genérico, sin ningún mensaje ni acción sugerida. En el uso real, el operador al llegar al comedor ve esta pantalla antes de iniciar el servicio: necesita una señal clara de que el sistema está listo y qué debe hacer a continuación.

También se eliminó `Register.cshtml`, una vista que ya no tenía acción asociada desde el commit c42.

---

## Cambios en `Views/Servicio/Index.cshtml`

### Empty state con icono, mensaje y hint de acción

**Antes:** bloque vacío o spinner genérico cuando `HayServicioActivo = false`.

**Después:** pantalla idle con:

```html
<div class="srv-idle-state">
    <i class="bi bi-cup-hot srv-idle-icon"></i>
    <h2>Sin servicio activo</h2>
    <p class="srv-idle-hint">
        Configurá el lugar y la proyección, luego presioná <strong>Iniciar servicio</strong>.
    </p>
</div>
```

El ícono `bi-cup-hot` es temáticamente coherente con el dominio (comedor). El texto del hint describe exactamente los pasos que el operador debe seguir.

---

## Eliminación de `Register.cshtml`

La vista `Views/Account/Register.cshtml` fue creada como parte del scaffold de Identity pero deshabilitada funcionalmente en c42. Mantenerla en el repositorio creaba confusión: ¿está disponible o no? Se eliminó para que el código refleje con exactitud el estado real del sistema.

> **Regla aplicada:** si una vista no tiene acción asociada y no hay plan de reactivarla, eliminarla es mejor que comentarla o dejarla sin uso.

---

## Herramientas de IA utilizadas

Claude Code identificó `Register.cshtml` como código muerto durante una revisión con el skill `/verify`. El empty state de la vista de servicio fue diseñado por el autor y la implementación HTML/CSS fue generada por Claude Code siguiendo las convenciones de clase existentes en `site.css`.
