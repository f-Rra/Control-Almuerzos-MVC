---
name: ef-core
description: >
  Entity Framework Core patterns. Covers DbContext configuration, migrations workflow,
  compiled queries, ExecuteUpdateAsync, ExecuteDeleteAsync, value converters, and
  query optimization. Load this skill when working with databases, writing queries,
  managing schema changes, or when the user mentions "EF Core", "Entity Framework",
  "DbContext", "migration", "LINQ query", "database", "SQL", "N+1", "Include",
  "AsNoTracking", "value converter", or "compiled query".
---

# EF Core (.NET 9)

## Core Principles

1. **DbContext is a unit of work** — No wrapping en otro UoW. EF Core ya implementa Unit of Work y Repository internamente.
2. **Queries should be projections** — Usar `.Select()` para proyectar a DTOs. Evita over-fetching y N+1.
3. **Migrations are code** — Revisarlas como cualquier PR. Nunca auto-aplicar en producción.
4. **Read-only queries need AsNoTracking** — Cualquier query que no vaya a hacer SaveChanges debe usar `.AsNoTracking()`.

## Patterns

### Query Projections (Avoid Over-Fetching)

```csharp
// GOOD — project to DTO, only loads needed columns
public async Task<List<EmpleadoViewModel>> ListarAsync()
{
    return await _context.Empleados
        .AsNoTracking()
        .Where(e => e.Estado)
        .Select(e => new EmpleadoViewModel
        {
            IdEmpleado = e.IdEmpleado,
            NombreCompleto = e.Nombre + " " + e.Apellido,
            EmpresaNombre = e.Empresa!.Nombre
        })
        .ToListAsync();
}
```

### Bulk Operations (sin cargar entidades)

```csharp
// Update sin cargar entidades — mucho más eficiente para 100+ rows
await _context.Empleados
    .Where(e => e.IdEmpresa == idEmpresa)
    .ExecuteUpdateAsync(s => s.SetProperty(e => e.Estado, false));

// Delete sin cargar entidades
await _context.Registros
    .Where(r => r.Fecha < fechaArchivo)
    .ExecuteDeleteAsync();
```

### Compiled Queries (hot-path)

```csharp
// Para queries que se ejecutan muy frecuentemente con la misma forma
public static readonly Func<ApplicationDbContext, int, CancellationToken, Task<Servicio?>> GetActivo =
    EF.CompileAsyncQuery((ApplicationDbContext db, int idLugar, CancellationToken ct) =>
        db.Servicios
            .Include(s => s.Lugar)
            .FirstOrDefault(s => s.IdLugar == idLugar && s.DuracionMinutos == null));
```

### Migrations Workflow

```bash
# Crear migración — nombre descriptivo del cambio, no de la entidad
dotnet ef migrations add AddEmpleadoTelefono

# Revisar el SQL generado SIEMPRE antes de aplicar
dotnet ef migrations script --idempotent

# Aplicar a desarrollo
dotnet ef database update

# Rollback
dotnet ef database update <MigracionAnterior>
```

### Global Query Filters

```csharp
// Soft delete filter en OnModelCreating
builder.HasQueryFilter(e => e.Estado);

// Bypass cuando necesitás ver inactivos
var todos = await _context.Empleados.IgnoreQueryFilters().ToListAsync();
```

## Anti-patterns

### No wrappear DbContext en un Repository genérico

```csharp
// BAD — abstracción innecesaria que limita el poder de EF Core
public interface IEmpleadoRepository { Task<Empleado?> GetByIdAsync(int id); }

// GOOD — usar el servicio de negocio directamente con DbContext
public class EmpleadoNegocio : IEmpleadoNegocio
{
    private readonly ApplicationDbContext _context;
    // ...
}
```

### No usar Lazy Loading

```csharp
// BAD — causa N+1 queries ocultos
// GOOD — Include explícito o proyección
var empleados = await _context.Empleados
    .Include(e => e.Empresa)
    .AsNoTracking()
    .ToListAsync();
```

### No filtrar en memoria

```csharp
// BAD — carga TODOS, filtra en C#
var todos = await _context.Registros.ToListAsync();
var hoy = todos.Where(r => r.Fecha == DateTime.Today);

// GOOD — filtrar en la BD
var hoy = await _context.Registros
    .AsNoTracking()
    .Where(r => r.Fecha == DateTime.Today)
    .ToListAsync();
```

### El workaround de triggers de SQL Server en este proyecto

`RegistroNegocio.RegistrarAsync()` usa `ExecuteSqlRawAsync` en lugar de `Add` + `SaveChanges`.
Esto es intencional: la tabla `Registros` tiene triggers activos y EF Core usa `OUTPUT` clause
para recuperar el PK generado, lo que es incompatible con triggers en SQL Server.
**No cambiar este patrón** sin remover o modificar el trigger primero.

## Decision Guide

| Escenario | Recomendación |
|-----------|--------------|
| CRUD estándar | DbContext con proyecciones |
| Bulk updates (100+ rows) | `ExecuteUpdateAsync` |
| Bulk deletes | `ExecuteDeleteAsync` |
| Query frecuente mismo shape | Compiled query |
| Reporte complejo | Raw SQL con `FromSqlInterpolated` |
| Tabla con triggers | `ExecuteSqlRawAsync` |
| Producción | Script SQL idempotente, nunca auto-migrate |
