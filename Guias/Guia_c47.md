# Guía Commit 47 — Panel Admin con métricas reales y estado del día en dashboard

## Commit
`8c86cf4` · `feat: panel Admin con métricas reales y estado del día en dashboard`

---

## Contexto

El panel de administración mostraba números hardcodeados ("12 empresas", "340 empleados") desde el commit en que se diseñó la vista. En esta etapa el sistema ya tiene datos reales en la BD, y el dashboard del operador no reflejaba el estado del servicio del día. Ambas vistas se conectaron a datos reales.

---

## Cambios en `AdminController`

Se inyectaron los servicios necesarios y se calculan los 4 KPIs en tiempo real:

```csharp
public class AdminController : Controller
{
    // Inyección de 4 servicios
    public async Task<IActionResult> Index()
    {
        var vm = new AdminViewModel
        {
            TotalEmpresas   = await _empresaNegocio.ContarAsync(),
            TotalEmpleados  = await _empleadoNegocio.ContarAsync(),
            TotalAsistencias= await _registroNegocio.ContarHoyAsync(),
            TotalUsuarios   = await _userManager.Users.CountAsync()
        };
        return View(vm);
    }
}
```

---

## `AdminViewModel` y `DashboardViewModel`

Se crearon dos ViewModels nuevos:

- **`AdminViewModel`:** 4 contadores enteros (Empresas, Empleados, AsistenciasHoy, Usuarios).
- **`DashboardViewModel`:** campos para el estado del servicio del día: `HayServicioHoy`, `RegistradosHoy`, `LugarHoy`. Consumido por `HomeController` para mostrar el banner de estado en el dashboard del operador.

---

## Banner de estado en `Home/Index.cshtml`

El dashboard del operador ahora muestra un banner contextual según el estado del servicio:

| Estado | Banner |
|---|---|
| Servicio activo | Verde — "Servicio en curso en [Lugar] · X registrados" |
| Servicio finalizado hoy | Gris — "Servicio de hoy finalizado · X registrados" |
| Sin servicio hoy | Neutro — "No hay servicio programado para hoy" |

---

## Patrón: datos reales solo donde hace falta

Se inyectaron servicios únicamente en los controllers que los necesitan. El patrón de "un controller, un viewmodel, una vista" se mantuvo estrictamente para evitar que los controllers crezcan en responsabilidades.

---

## Herramientas de IA utilizadas

Claude Code (modo agente) implementó los cambios en los cuatro archivos afectados en una sola operación: `AdminController`, `AdminViewModel`, `DashboardViewModel`, `HomeController` y `Home/Index.cshtml`. El autor definió la lógica de los 3 estados del banner y Claude Code los tradujo a la condicional Razor correspondiente.
