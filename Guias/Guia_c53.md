# Guía Commit 53 — Eliminación de código muerto

## Commit
`chore: eliminar código muerto en servicios y viewmodels`

---

## Contexto

La misma revisión exhaustiva del commit anterior identificó métodos y clases que existían en el código pero no eran llamados desde ningún punto del sistema. Mantenerlos genera ruido, confunde a quien lee el código y agranda el surface area del proyecto sin ningún beneficio.

---

## Qué se eliminó

### `ServicioNegocio` — 3 métodos sin uso

| Método | Por qué existe | Por qué se eliminó |
|---|---|---|
| `ListarTodosAsync()` | Vestigio de una versión anterior del dashboard | Reemplazado por queries específicas en `ReporteNegocio` y `HomeController` |
| `ObtenerUltimoAsync()` | Nunca integrado en ningún controller | Sin caso de uso activo |
| `ListarPorFechaAsync()` | Duplica funcionalidad de `ReporteNegocio` | `ReporteNegocio` hace su propia query con filtros |

Los tres métodos se eliminaron de `ServicioNegocio.cs` y de `IServicioNegocio.cs`.

---

### `AccountController` — acción POST Register redundante

La acción `POST Register` existía como par del `GET Register`, pero ambas simplemente redirigían al Login. El `GET` es suficiente. Se eliminó el `POST` para evitar confusión sobre si el registro está habilitado.

---

### `RegisterViewModel` — ViewModel sin vista ni acción

`AccountViewModels.cs` contenía `RegisterViewModel` con 6 propiedades validadas. La vista `Register.cshtml` fue eliminada en un commit anterior (c46) y la acción `POST Register` se eliminó en este mismo commit. El ViewModel quedó huérfano: sin vista que lo renderice ni acción que lo reciba.

---

## Por qué importa eliminar código muerto

Código que no se ejecuta pero existe en el repositorio:
- Ocupa espacio mental al leer el código
- Puede confundir sobre las capacidades reales del sistema
- Puede ser instanciado por error en el futuro

---

## Herramientas de IA utilizadas

Claude Code localizó los métodos sin uso mediante análisis estático del proyecto (grep por referencias a cada método en controllers, vistas y otros servicios). El agente confirmó cero referencias antes de proponer la eliminación.
