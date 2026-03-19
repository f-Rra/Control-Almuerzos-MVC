# Guía Commit 17 — Estadísticas y Dashboard con Datos Reales

## 🎯 Propósito

Dar vida al Dashboard Principal (Home del Administrador y el Módulo puro de Estadísticas) vinculándolos permanentemente al servidor de SQL para que muestren la realidad actual de la base de datos a los gerentes.

## 📝 Concepto Central

No queremos llenar los Controladores con sumatorias y listados pesados de registros. El cómputo del `Top 5 Empresas` (que requiere unificar datos de servicio, asistencias registradas y sumatorias temporales) o de los de `Porcentajes Mensuales Promedio` deben salir de la instancia SQL a través de llamadas de baja carga o resolverse mediante Agrupamientos ADO/EF nativos (`GROUP BY`), representándolos dinámicamente en los contadores (`div.kpi`).

## ⚡ Pasos a Seguir

1. **Crear Motor (`IEstadisticasNegocio.cs`)**:
   - Funciones `ObtenerKpisEmpleados()`, `ObtenerKpisEmpresas()`, `ObtenerKpisAsistencias()`. 
   - Funciones del Dashboard: `ObtenerUltimosServicios(cant)` retornando métricas condensadas para la página de inicio.
2. **El Controlador (`EstadisticaController.cs`)**:
   - Acción `Index()` que concentre la obtención total (bloque simultáneo, o mediante un Task Array estructurado).
   - Inyectarlo en un potente `EstadisticaViewModel` masivo.
3. **El Dashboard del `HomeController.cs`**:
   - `Index()` llama al negocio para cargar los 5 servicios recientes y construir su propio ViewModel `DashboardViewModel` (Últimos, Detalles activos y gráfico de barras estático para rellenar JS).
4. **Vistas Razor**:
   - Cambiar valores HardCode `<p>55</p>` por `@Model.Empleados.TotalActivos`.
   - Modificar las barras de CSS width inline: `style="width: @(Model.Top[0].Porcentaje)%"`
   - Vincular lista de servicios en el Index del Home con `data-json` (para que JS maneje cambios visuales sin postbacks).

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Services/IEstadisticasNegocio.cs` | Crear | Repositorio de analítica de consultas SQL optimizadas |
| `Controllers/EstadisticaController.cs` | Crear | Controlador de KPI general |
| `Controllers/HomeController.cs` | Modificar | Incorporar Inyección del Negocio a los paneles del index base |
| `Views/Estadistica/Index.cshtml` | Modificar | Integrar variables Razor en KPIs |
| `Views/Home/Index.cshtml` | Modificar | Rellenar componentes |

## 🚀 Mensaje de Commit

```
feat: conectar estadísticas y dashboard con datos reales

- KPIs de empleados, empresas, servicios y asistencias
- Top 5 empresas con barras de progreso
- Dashboard Index conectado a datos del servidor
- Selección dinámica de servicio en listado
```
