# Guía Commit 43 — Optimización de queries con AsNoTracking y agregaciones en BD

## Commit
`a685f49` · `perf: optimizar queries EF Core con AsNoTracking y agregaciones en BD`

---

## Contexto

Con ~500 empleados en seed data, las queries que antes funcionaban bien con datos mínimos empezaron a mostrar overhead innecesario. El skill `/ef-core` reveló tres anti-patrones presentes en los servicios de negocio. Este commit los corrige.

---

## Cambios aplicados

### 1. `AsNoTracking` en todas las queries de solo lectura

EF Core, por defecto, rastrea cada entidad que devuelve en memoria (change tracking). Esto permite detectar cambios y persistirlos con `SaveChanges`. Para queries de solo lectura (listados, estadísticas, reportes), este tracking es overhead puro.

```csharp
// Antes
var empleados = await _context.Empleados.ToListAsync();

// Después
var empleados = await _context.Empleados.AsNoTracking().ToListAsync();
```

Se aplicó en los 6 servicios de negocio: `EmpleadoNegocio`, `EmpresaNegocio`, `ServicioNegocio`, `RegistroNegocio`, `EstadisticasNegocio`, `ReporteNegocio`.

---

### 2. `EstadisticasNegocio`: contar en BD con `CountAsync`

**Antes:** Se traía la lista completa de registros a memoria y se contaba con `.Count`.

**Después:** `CountAsync()` genera un `SELECT COUNT(*)` en SQL Server. La diferencia es que los datos nunca viajan por la red — solo el número final.

```csharp
// Antes: trae miles de registros para contar
var registros = await _context.Registros.ToListAsync();
int total = registros.Count;

// Después: SELECT COUNT(*) FROM Registros
int total = await _context.Registros.CountAsync();
```

---

### 3. `EstadisticasNegocio`: `GroupBy` por empresa en BD

**Antes:** Se traían todos los registros con sus empleados (navigation property), y el agrupamiento se hacía en memoria con LINQ to Objects.

**Después:** El `GroupBy` se traduce a `GROUP BY IdEmpresa` en SQL Server. El servidor agrupa; la aplicación recibe solo los totales.

---

### 4. `ReporteNegocio`: distribución por día de semana en BD

Mismo patrón: el cálculo de cuántos registros hay por día de la semana se movió de memoria a BD con `GroupBy(r => r.Fecha.DayOfWeek)`.

---

## Impacto

A escala de producción (~500 empleados, servicios diarios durante meses), estas optimizaciones reducen significativamente el tiempo de carga de las vistas de estadísticas y reportes, y disminuyen la memoria utilizada por el proceso de la aplicación.

---

## Herramientas de IA utilizadas

**Skill `/ef-core`** ejecutado para identificar anti-patrones. **Skill `/db-review`** para validar que los cambios generaran el SQL correcto. Claude Code propuso todos los cambios; el autor los revisó y validó contra el comportamiento esperado de las vistas.
