# Guía Commit 23 — Seeding de Datos Iniciales con Entity Framework

## 🎯 Propósito

El objetivo de este commit es poblar la base de datos automáticamente con datos iniciales usando el mecanismo de **`HasData()`** de Entity Framework Core. Esto permite que al aplicar las migraciones, la base de datos ya contenga los datos necesarios para que el sistema funcione sin intervención manual.

Al implementar el seeding con EF Core, se logra:

1. **Reproducibilidad**: Cualquier desarrollador que clone el proyecto y ejecute `Update-Database` tendrá los mismos datos base
2. **Eliminación de scripts manuales**: Ya no es necesario ejecutar `Datos_Iniciales.sql` por separado
3. **Trazabilidad**: Los datos semilla quedan versionados en las migraciones junto con el esquema

---

## 📝 Concepto Central

### ¿Qué es HasData()?

`HasData()` es un método de Fluent API que permite declarar datos iniciales dentro de `OnModelCreating`. Al generar una migración, EF Core traduce estas declaraciones en sentencias `INSERT` con `IDENTITY_INSERT ON`.

**Regla clave**: `HasData()` requiere que especifiques manualmente las claves primarias (`Id`), ya que los valores autogenerados (`Identity`) no están disponibles en diseño.

```csharp
modelBuilder.Entity<Lugar>().HasData(
    new Lugar { IdLugar = 1, Nombre = "Comedor", Descripcion = "Comedor principal", Estado = true },
    new Lugar { IdLugar = 2, Nombre = "Quincho", Descripcion = "Quincho exterior", Estado = true }
);
```

### ¿Qué datos seedear?

Se deben seedear únicamente datos **estáticos y de referencia** que el sistema necesita para funcionar:
- **Lugares**: Son finitos y conocidos (Comedor, Quincho)
- **Empresas**: Las empresas del complejo industrial son conocidas de antemano
- **Empleados**: Se puede incluir un set inicial con credenciales RFID asignadas

**No se deben seedear** datos transaccionales como Servicios y Registros, ya que estos se generan en runtime con cada jornada.

---

## ⚡ Pasos a Seguir

### 1. Definir los datos semilla en `ApplicationDbContext`

Dentro de `OnModelCreating`, agregar las llamadas a `HasData()` para cada entidad. Los IDs deben ser explícitos y secuenciales.

**Lugares (2 registros):**
```csharp
modelBuilder.Entity<Lugar>().HasData(
    new Lugar { IdLugar = 1, Nombre = "Comedor", ... },
    new Lugar { IdLugar = 2, Nombre = "Quincho", ... }
);
```

**Empresas (12 registros):**
- Las mismas empresas del complejo que existían en el proyecto original
- Cada una con `IdEmpresa` explícito (1 a 12) y `Estado = true`

**Empleados (60 registros):**
- Distribuidos entre las 12 empresas (aproximadamente 5 por empresa)
- Credenciales RFID asignadas: RF001 a RF060
- Nombres y apellidos realistas para demo
- Todos con `Estado = true`

### 2. Generar la migración de seeding

```bash
dotnet ef migrations add SeedDatosIniciales
```

Esta migración contendrá únicamente los `INSERT` con `IDENTITY_INSERT ON/OFF` para cada tabla.

### 3. Aplicar la migración

```bash
dotnet ef database update
```

### 4. Verificar los datos en la base de datos

```sql
SELECT COUNT(*) FROM Lugares;    -- Esperado: 2
SELECT COUNT(*) FROM Empresas;   -- Esperado: 12
SELECT COUNT(*) FROM Empleados;  -- Esperado: 60
```

---

## ⚠️ Consideraciones Importantes

- **IDs fijos**: Al especificar IDs manualmente, se debe evitar conflictos con datos ya existentes en la base. Si la base tiene datos previos, conviene resetear las migraciones
- **Foreign Keys**: Los `IdEmpresa` en los empleados deben coincidir con los IDs definidos en el seed de empresas
- **Orden de declaración**: Declarar primero las entidades padre (Lugar, Empresa) y luego las hijas (Empleado) para respetar las FK
- **Idempotencia**: Si ya existen registros con los mismos IDs, la migración fallará. EF Core no hace upsert, solo insert

---

## 📁 Archivos a Modificar

| Archivo | Acción |
|---|---|
| `Data/ApplicationDbContext.cs` | Agregar llamadas a `HasData()` en `OnModelCreating` |
| `Migrations/[timestamp]_SeedDatosIniciales.cs` | Generada automáticamente por `dotnet ef migrations add` |

---

## 🚀 Mensaje de Commit

```
feat: implementar seed de datos iniciales con ModelBuilder y reset de migraciones

- Carga inicial de datos mediante HasData
- Servicios y registros incorporados en la migración
- Historial de migraciones sincronizado con el esquema actual
- Restauración de procedimientos almacenados, vistas y triggers SQL
```
