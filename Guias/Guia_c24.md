# Guía del Commit 24: Configuraciones Fluent API Separadas por Entidad

## 🎯 Propósito de este Commit

Hasta el Commit 21-22, todas las reglas de mapeo de EF Core vivían en un solo método enorme (`OnModelCreating`) dentro de `ApplicationDbContext.cs`. Cuanto más crece el proyecto, más difícil es encontrar la configuración de una entidad específica.

Este commit aplica un **principio de responsabilidad única (Single Responsibility Principle)**: cada entidad del dominio tiene ahora **su propio archivo de configuración**. El método `OnModelCreating` queda reducido a una sola línea funcional, y EF Core encuentra y aplica la configuración de cada entidad automáticamente.

---

## 📝 Concepto Central: `IEntityTypeConfiguration<T>`

Esta es una interfaz de EF Core que permite encapsular toda la configuración de Fluent API de una entidad en una clase separada. Cada clase implementa un único método llamado `Configure(EntityTypeBuilder<T> builder)`.

Es el equivalente a tener el mismo código que estaba en `OnModelCreating`, pero usando el parámetro local `builder` en lugar de `modelBuilder.Entity<T>()`.

### Antes (todo junto en ApplicationDbContext):
```csharp
modelBuilder.Entity<Empresa>()
    .HasMany(e => e.Empleados)...
    .HasMany(e => e.Registros)...
    .Property(e => e.Estado).HasDefaultValue(true);

modelBuilder.Entity<Empleado>()
    .HasMany(...)...
    .HasIndex(...)...

// ... 100+ líneas más ...
```

### Después (separado por entidad):
```csharp
// EmpresaConfiguration.cs
public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.HasMany(e => e.Empleados)...
        builder.HasMany(e => e.Registros)...
        builder.Property(e => e.Estado).HasDefaultValue(true);
    }
}
```

---

## 📁 Archivos Creados

| Archivo | Responsabilidad |
|---|---|
| `Data/Configurations/EmpresaConfiguration.cs` | Relaciones 1:N con Empleados y Registros, Estado por defecto |
| `Data/Configurations/EmpleadoConfiguration.cs` | Relación con Registros (nullable), índice único de credencial, Estado por defecto |
| `Data/Configurations/LugarConfiguration.cs` | Relaciones con Servicios y Registros, Estado por defecto |
| `Data/Configurations/ServicioConfiguration.cs` | Relación con Registros, defaults de totales, check constraint de fecha |
| `Data/Configurations/RegistroConfiguration.cs` | Índice único compuesto filtrado (`IdEmpleado` + `IdServicio`) |

---

## ⚡ La Línea Mágica: `ApplyConfigurationsFromAssembly`

Una vez que creamos las 5 clases de configuración, el `OnModelCreating` se reduce drásticamente:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Esta línea escanea el ensamblado buscando todas las clases
    // que implementan IEntityTypeConfiguration<T> y las aplica.
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    SeedData(modelBuilder);
}
```

`typeof(ApplicationDbContext).Assembly` le dice a EF Core: *"Busca en el mismo DLL donde vive ApplicationDbContext"*. EF encuentra automáticamente `EmpresaConfiguration`, `EmpleadoConfiguration`, etc., y las aplica sin que tengamos que registrarlas una a una. Si en el futuro agregamos una nueva entidad con su `Configuration`, simplemente existe y ya está aplicada.

## 🛠️ Pasos a Seguir
1. Creadas las 5 clases de configuración en `Data/Configurations/`.
2. El bloque enorme de `OnModelCreating` fue reemplazado por la llamada a `ApplyConfigurationsFromAssembly`.
3. El proyecto compila y pasa el build de verificación.

---

## 📋 Mensaje para tu Commit

**Asunto / Mensaje principal:**
```text
refactor: separar configuraciones Fluent API por entidad
```

**Descripción / Cuerpo:**
```text
- IEntityTypeConfiguration<T> para cada modelo
- ApplyConfigurationsFromAssembly en OnModelCreating
- Mejor organización y mantenibilidad del contexto
```
