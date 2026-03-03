# Guía del Commit 25: Vistas Parciales Reutilizables

## 🎯 Propósito de este Commit

Al crecer un proyecto MVC, empiezan a aparecer fragmentos de HTML que se repiten en múltiples vistas. Por ejemplo:

- El mismo bloque de KPI aparece 13 veces en `Estadistica/Index.cshtml` con datos distintos.
- El filtro de fechas "Desde / Hasta" existe en `Reportes` y podría reaparecer en otras secciones.
- La fila de empleado con sus 6 celdas de links tiene siempre la misma estructura.
- La tarjeta de servicio del Dashboard lleva 10 atributos `data-*` que se repetirían en cada ítem.

Copiar y pegar ese HTML en cada lugar viola el principio **DRY (Don't Repeat Yourself)**. Si mañana hay que cambiar el diseño de una tarjeta KPI, habría que editarla en 13 sitios distintos, con alto riesgo de quedar inconsistente.

La solución de ASP.NET MVC Razor son las **Vistas Parciales**: archivos `.cshtml` cuyo nombre empieza con `_` que renderizan un fragmento de HTML, reciben su propio modelo fuertemente tipado, y se incluyen en otras vistas con una sola línea.

---

## 📝 Concepto Central: Vistas Parciales

### ¿Qué es una Vista Parcial?

Un archivo `.cshtml` que:

1. **Empieza con `_`** por convención, indicando que no es una vista completa (no tiene `_Layout`).
2. Vive en `Views/Shared/` si se usa desde múltiples controladores, o en `Views/[Controlador]/` si es exclusiva de ese módulo.
3. Puede declarar su propio **`@model`** fuertemente tipado.
4. Se incluye en otra vista usando el **tag helper `<partial>`** o el helper `Html.PartialAsync`.

### Las dos formas de incluir una partial

```razor
@* Forma 1 — Tag Helper (recomendada, sintaxis limpia y moderna) *@
<partial name="_KpiCard" model="@((Icono: "bi-people", IcoColor: "activo",
                                   Label: "Activos", Valor: "25"))" />

@* Forma 2 — HTML Helper (más verbosa, compatible con versiones anteriores) *@
@await Html.PartialAsync("_KpiCard", (Icono: "bi-people", IcoColor: "activo",
                                      Label: "Activos", Valor: "25"))
```

Este proyecto usa el **tag helper `<partial>`** (Forma 1). Es la sintaxis recomendada a partir de ASP.NET Core 2.1.

---

## 💡 Concepto Clave: Tuplas C# como Modelo de Parciales

Para pasar datos tipados a un partial **sin crear una clase ViewModel separada**, usamos **tuplas con nombre** de C# (disponibles desde C# 7). Son livianas, seguras en tiempo de compilación, y no requieren ningún archivo extra:

```razor
@* En el @model de la partial — declara la tupla como tipo *@
@model (string Icono, string IcoColor, string Label, string Valor)

@* En la vista que la include — pasa los valores *@
<partial name="_KpiCard"
         model="@((Icono: "bi-person-check", IcoColor: "activo",
                   Label: "Activos", Valor: Model.EmpleadosActivos.ToString()))" />
```

**¿Por qué doble paréntesis en `model="@(( ... ))"`?**

- El paréntesis exterior `@(...)` es la expresión Razor.
- El paréntesis interior `(...)` es el literal de tupla de C#.

---

## 📁 Archivos Creados

| Archivo | Modelo de Tupla | Propósito |
|---|---|---|
| `Views/Shared/_KpiCard.cshtml` | `(string Icono, string IcoColor, string Label, string Valor)` | Bloque de indicador KPI con ícono, color, etiqueta y valor |
| `Views/Shared/_ServicioCard.cshtml` | `(ServicioReporte Servicio, bool IsSelected)` | Tarjeta de servicio del Dashboard con `data-*` atributos |
| `Views/Shared/_EmpleadoRow.cshtml` | `(Empleado Empleado, int? SeleccionadoId, string Filtro, int? EmpresaFiltroId, int Numero)` | Fila de la tabla de empleados con links y badge de estado |
| `Views/Shared/_FiltroFechas.cshtml` | `(DateTime Desde, DateTime Hasta)` | Par de inputs `Desde` / `Hasta` para filtros de fecha |
| `Views/Shared/_Paginacion.cshtml` | `(int PaginaActual, int TotalPaginas, string Accion, string Controlador)` | Controles de paginación (preparada para uso futuro) |

---

## 📝 Archivos Modificados

| Vista | Qué se reemplazó | Por qué |
|---|---|---|
| `Views/Estadistica/Index.cshtml` | 13 bloques `<div class="est-kpi">` | Todos usan la misma estructura, solo cambian el ícono, color, etiqueta y valor |
| `Views/Home/Index.cshtml` | El bloque `<div class="svc-card">` dentro del `foreach` | La tarjeta de servicio tiene 10 atributos data-* que ahora vive en un solo lugar |
| `Views/Reporte/Index.cshtml` | Los dos `<div class="cfg-item">` de fecha | El par Desde/Hasta puede reutilizarse en cualquier filtro de fecha futuro |
| `Views/Empleado/Index.cshtml` | Las filas del `foreach` en la tabla de empleados | Cada fila tenía 6 celdas con 4 `asp-route-*` repetidos por celda = 24 repeticiones |

---

## ⚡ Detalle de cada Vista Parcial

### 1. `_KpiCard.cshtml` — Indicador numérico

El bloque más repetido del proyecto. Antes aparecía **13 veces** dentro de `Estadistica/Index.cshtml`, cada vez con el mismo HTML:

```razor
@* ANTES — 13 copias de esto con datos distintos *@
<div class="est-kpi">
    <div class="est-kpi-ico activo"><i class="bi bi-person-check"></i></div>
    <div class="est-kpi-info">
        <span class="est-kpi-label">Activos</span>
        <span class="est-kpi-value activo">@Model.EmpleadosActivos</span>
    </div>
</div>

@* AHORA — una sola línea *@
<partial name="_KpiCard"
         model="@((Icono: "bi-person-check", IcoColor: "activo",
                   Label: "Activos", Valor: Model.EmpleadosActivos.ToString()))" />
```

El parámetro `IcoColor` acepta los valores de clase CSS ya definidos: `activo`, `inactivo`, `empresa`, `servicio`, `promedio`, `asist`, `empl`, `top`, o vacío `""` para el color neutro.

---

### 2. `_ServicioCard.cshtml` — Tarjeta de servicio del Dashboard

La tarjeta de servicio en `Home/Index.cshtml` tenía **10 atributos `data-*`** y lógica de cómputo (cobertura, ícono de lugar, clase CSS "selected"). Todo eso queda encapsulado en la partial:

```razor
@* ANTES — ~20 líneas de HTML por cada servicio *@
@foreach (var s in Model.UltimosServicios)
{
    var isComedor = s.Lugar.ToLower().Contains("comedor");
    var css = first ? "svc-card selected" : "svc-card";
    first = false;
    <div class="@css" data-id="@s.IdServicio" data-lugar="@s.Lugar"
         data-fecha="@s.Fecha.ToString("dd/MM/yyyy")" ...>
        ...
    </div>
}

@* AHORA — 3 líneas *@
@{ bool first = true; }
@foreach (var s in Model.UltimosServicios)
{
    <partial name="_ServicioCard" model="@((Servicio: s, IsSelected: first))" />
    first = false;
}
```

La partial recibe `IsSelected: first` al momento de la llamada, capturando el valor `true` para el primer ítem y `false` para el resto.

---

### 3. `_EmpleadoRow.cshtml` — Fila de la tabla de empleados

Cada fila en `Empleado/Index.cshtml` tenía **6 celdas**, y cada celda contenía un `<a>` con los mismos 4 parámetros `asp-route-*` repetidos:

```
asp-route-idEmpleado, asp-route-filtro, asp-route-empresaFiltroId
```

Eso es **24 apariciones del mismo set de atributos** por fila. La partial los centraliza en un solo lugar. El modelo usa una tupla con los 5 datos que necesita la fila:

```razor
<partial name="_EmpleadoRow"
         model="@((Empleado: emp,
                   SeleccionadoId: Model.EmpleadoSeleccionadoId,
                   Filtro: Model.Filtro,
                   EmpresaFiltroId: Model.EmpresaFiltroId,
                   Numero: num))" />
```

> **Nota sobre Tag Helpers en parciales:** los `asp-action` y `asp-route-*` dentro de `_EmpleadoRow.cshtml` resuelven la acción correctamente porque heredan el contexto de ruta del controlador padre. Al ser invocada desde `EmpleadoController`, todos los links generan rutas a `Empleado/Index`.

---

### 4. `_FiltroFechas.cshtml` — Selector de rango de fechas

Renderiza el par de controles `Desde` / `Hasta` como dos `<div class="cfg-item">`. Sus `name="desde"` y `name="hasta"` coinciden exactamente con los parámetros que espera el controlador de Reportes (y cualquier otro que filtre por fecha en el futuro):

```razor
<partial name="_FiltroFechas"
         model="@((Desde: Model.FechaDesde, Hasta: Model.FechaHasta))" />
```

Reemplaza 10 líneas de HTML en `Reporte/Index.cshtml` y está lista para ser incluida en cualquier vista que necesite filtrar por rango de fechas.

---

### 5. `_Paginacion.cshtml` — Controles de paginación

Partial preparada para uso futuro cuando se implemente paginación en las tablas de empleados, registros o reportes. Recibe la página actual, el total, y el controlador/acción destino:

```razor
@* Ejemplo de uso futuro cuando exista paginación *@
<partial name="_Paginacion"
         model="@((PaginaActual: ViewBag.Pagina,
                   TotalPaginas: ViewBag.TotalPaginas,
                   Accion: "Index",
                   Controlador: "Empleado"))" />
```

Si `TotalPaginas <= 1`, la partial no renderiza nada (no hay nada que paginar). Los botones Anterior/Siguiente se deshabilitan en los extremos.

---

## 🛠️ Pasos Realizados

1. **Creadas 5 vistas parciales** en `Views/Shared/`, cada una con su propio `@model` de tupla y comentario de uso en el encabezado.
2. **`Estadistica/Index.cshtml`**: 13 bloques `<div class="est-kpi">` reemplazados por `<partial name="_KpiCard">` — la vista pasó de ~190 líneas de HTML repetitivo a usar una abstracción coherente.
3. **`Home/Index.cshtml`**: el bloque `<div class="svc-card">` del foreach (~20 líneas) reemplazado por `<partial name="_ServicioCard">` (3 líneas).
4. **`Reporte/Index.cshtml`**: los dos `<div class="cfg-item">` de fecha reemplazados por `<partial name="_FiltroFechas">` (1 línea).
5. **`Empleado/Index.cshtml`**: las filas del foreach (~55 líneas de HTML anidado) reemplazadas por `<partial name="_EmpleadoRow">` (8 líneas).

---

## 🔎 Comparativa: Antes vs Después

| Sección | Líneas antes | Líneas después | Reducción |
|---|---|---|---|
| KPIs en Estadística (×13) | ~130 líneas | 13 líneas | ~90% |
| Tarjeta de servicio en Dashboard | ~25 líneas | 5 líneas | ~80% |
| Fila de empleado (×N empleados) | ~55 líneas | 8 líneas | ~85% |
| Filtro de fechas en Reportes | 10 líneas | 1 línea | ~90% |

---

## 🚀 Cómo hacer tu Commit

```bash
git add SCA-MVC/Views/Shared/_KpiCard.cshtml
git add SCA-MVC/Views/Shared/_ServicioCard.cshtml
git add SCA-MVC/Views/Shared/_EmpleadoRow.cshtml
git add SCA-MVC/Views/Shared/_FiltroFechas.cshtml
git add SCA-MVC/Views/Shared/_Paginacion.cshtml
git add SCA-MVC/Views/Estadistica/Index.cshtml
git add SCA-MVC/Views/Home/Index.cshtml
git add SCA-MVC/Views/Reporte/Index.cshtml
git add SCA-MVC/Views/Empleado/Index.cshtml
git add Guias/Guia_c25.md
git commit -m "feat: crear vistas parciales reutilizables"
```
