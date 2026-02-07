# Plan de Commits - Unidad 1: Fundamentos de ASP.NET MVC

Este documento describe la división del trabajo de la Unidad 1 en 3 commits lógicos y progresivos.

---

## Commit 1: Configuración Inicial del Proyecto MVC

### Objetivo
Crear la estructura base del proyecto ASP.NET MVC y establecer la configuración fundamental para comenzar el desarrollo.

### Paso a Paso

#### 1. Creación del Proyecto
- Abrir Visual Studio 2022
- Crear un nuevo proyecto de tipo "ASP.NET Core Web App (Model-View-Controller)"
- Configurar el nombre del proyecto como `SistemaControlAlmuerzos.Web`
- Seleccionar el framework .NET 6.0 o superior
- Habilitar HTTPS para conexiones seguras
- Configurar sin autenticación inicial (se agregará en unidades posteriores)

#### 2. Exploración de la Estructura Generada
- Revisar la carpeta `Controllers/` que contiene el controlador Home por defecto
- Verificar la carpeta `Models/` que estará vacía inicialmente
- Examinar la carpeta `Views/` con las vistas de ejemplo (Home/Index, Shared/_Layout)
- Inspeccionar la carpeta `wwwroot/` para archivos estáticos (CSS, JS, imágenes)
- Revisar el archivo `Program.cs` que contiene la configuración de la aplicación
- Verificar el archivo `appsettings.json` para configuraciones del entorno

#### 3. Instalación de Paquetes NuGet Necesarios
- Instalar `Microsoft.EntityFrameworkCore.SqlServer` para la conexión con SQL Server
- Instalar `Microsoft.EntityFrameworkCore.Tools` para ejecutar comandos de migraciones
- Instalar `Microsoft.EntityFrameworkCore.Design` para el diseño de la base de datos
- Verificar que todas las dependencias se hayan instalado correctamente

#### 4. Configuración de la Cadena de Conexión
- Abrir el archivo `appsettings.json`
- Agregar una sección `ConnectionStrings` con la cadena de conexión a la base de datos
- Configurar el nombre del servidor SQL Server
- Especificar el nombre de la base de datos (ej: `SistemaControlAlmuerzos`)
- Definir el tipo de autenticación (Windows Authentication o SQL Server Authentication)
- Agregar parámetros de seguridad y configuración adicionales

#### 5. Verificación Inicial
- Compilar el proyecto para asegurar que no hay errores
- Ejecutar la aplicación para verificar que el proyecto base funciona correctamente
- Comprobar que la página de inicio (Home/Index) se muestra correctamente
- Verificar que el layout y los estilos base se cargan apropiadamente

### Resultado Esperado
Un proyecto ASP.NET MVC funcional con:
- Estructura de carpetas estándar (Models, Views, Controllers, wwwroot)
- Paquetes NuGet de Entity Framework instalados
- Cadena de conexión configurada en appsettings.json
- Proyecto compilando y ejecutándose sin errores

### Mensaje de Commit
```
feat: inicializar proyecto ASP.NET MVC con configuración base

- Crear proyecto ASP.NET Core MVC con .NET 6.0
- Instalar paquetes Entity Framework Core (SqlServer, Tools, Design)
- Configurar cadena de conexión a SQL Server en appsettings.json
- Verificar estructura inicial del proyecto (MVC, wwwroot, Program.cs)
```

---

## Commit 2: Creación de Modelos de Dominio con Validaciones

### Objetivo
Migrar las clases de dominio desde el proyecto WinForms original, creando modelos limpios con validaciones mediante Data Annotations, preparados para Entity Framework Core.

### Paso a Paso

#### 1. Creación de la Carpeta Models
- Verificar que existe la carpeta `Models/` en la raíz del proyecto
- Si no existe, crearla

#### 2. Migración de la Clase Empresa
- Crear el archivo `Models/Empresa.cs`
- Definir las propiedades básicas:
  - `IdEmpresa` (int) - Clave primaria
  - `Nombre` (string) - Nombre de la empresa
  - `Estado` (bool) - Estado activo/inactivo
- Agregar propiedades de navegación (sin inicializar aún):
  - `ICollection<Empleado> Empleados` - Relación 1:N con Empleados
  - `ICollection<Registro> Registros` - Relación 1:N con Registros
- Aplicar Data Annotations:
  - `[Required(ErrorMessage = "El nombre de la empresa es obligatorio")]` en `Nombre`
  - `[StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]` en `Nombre`
  - `[Display(Name = "Empresa")]` en `Nombre`
  - `[Display(Name = "Estado")]` en `Estado`
- **Nota**: Eliminar la propiedad `CantidadEmpleados` del proyecto original (se calculará con `Empleados.Count()`)

#### 3. Migración de la Clase Empleado
- Crear el archivo `Models/Empleado.cs`
- Definir las propiedades básicas:
  - `IdEmpleado` (int) - Clave primaria
  - `Nombre` (string) - Nombre del empleado
  - `Apellido` (string) - Apellido del empleado
  - `IdCredencial` (string) - Credencial RFID única
  - `IdEmpresa` (int) - Clave foránea a Empresa
  - `Estado` (bool) - Estado activo/inactivo
- Agregar propiedades de navegación:
  - `Empresa Empresa` - Relación N:1 con Empresa
  - `ICollection<Registro> Registros` - Relación 1:N con Registros
- Agregar propiedad calculada:
  - `NombreCompleto` (string, solo lectura) - Retorna `$"{Nombre} {Apellido}"`
- Aplicar Data Annotations:
  - `[Required]` y `[StringLength(100)]` en `Nombre` y `Apellido`
  - `[Required]`, `[StringLength(50)]` en `IdCredencial`
  - `[Display(Name = "Credencial RFID")]` en `IdCredencial`
  - `[Display(Name = "Nombre Completo")]` y `[NotMapped]` en `NombreCompleto`
- **Nota**: Eliminar la propiedad `NombreEmpresa` del proyecto original (se accede vía `Empresa.Nombre`)

#### 4. Migración de la Clase Lugar
- Crear el archivo `Models/Lugar.cs`
- Definir las propiedades básicas:
  - `IdLugar` (int) - Clave primaria
  - `Nombre` (string) - Nombre del lugar (ej: Comedor, Quincho)
  - `Estado` (bool) - Estado activo/inactivo
- Agregar propiedades de navegación:
  - `ICollection<Servicio> Servicios` - Relación 1:N con Servicios
  - `ICollection<Registro> Registros` - Relación 1:N con Registros
- Aplicar Data Annotations:
  - `[Required]` y `[StringLength(50)]` en `Nombre`
  - `[Display(Name = "Lugar")]` en `Nombre`
- **Nota**: Eliminar la propiedad `Descripcion` del proyecto original (no existe en la base de datos)

#### 5. Migración de la Clase Servicio
- Crear el archivo `Models/Servicio.cs`
- Definir las propiedades básicas:
  - `IdServicio` (int) - Clave primaria
  - `IdLugar` (int) - Clave foránea a Lugar
  - `Fecha` (DateTime) - Fecha del servicio
  - `Proyeccion` (int?) - Proyección estimada de comensales (nullable)
  - `DuracionMinutos` (int?) - Duración total del servicio en minutos (nullable)
  - `TotalComensales` (int) - Total de comensales registrados
  - `TotalInvitados` (int) - Total de invitados registrados
- Agregar propiedades de navegación:
  - `Lugar Lugar` - Relación N:1 con Lugar
  - `ICollection<Registro> Registros` - Relación 1:N con Registros
- Agregar propiedades calculadas:
  - `TotalGeneral` (int, solo lectura) - Retorna `TotalComensales + TotalInvitados`
  - `Estado` (string, solo lectura) - Retorna "Activo" si `DuracionMinutos == null`, sino "Finalizado"
- Aplicar Data Annotations:
  - `[Required]` y `[DataType(DataType.Date)]` en `Fecha`
  - `[Display(Name = "Fecha")]` en `Fecha`
  - `[Display(Name = "Proyección")]` en `Proyeccion`
  - `[Display(Name = "Duración (min)")]` en `DuracionMinutos`
  - `[Display(Name = "Total Comensales")]` en `TotalComensales`
  - `[Display(Name = "Total Invitados")]` en `TotalInvitados`
  - `[NotMapped]` en `TotalGeneral` y `Estado`
- **Nota**: Eliminar la propiedad `NombreLugar` del proyecto original (se accede vía `Lugar.Nombre`)

#### 6. Migración de la Clase Registro
- Crear el archivo `Models/Registro.cs`
- Definir las propiedades básicas:
  - `IdRegistro` (int) - Clave primaria
  - `IdEmpleado` (int?) - Clave foránea a Empleado (**NULLABLE** para invitados)
  - `IdEmpresa` (int) - Clave foránea a Empresa
  - `IdServicio` (int) - Clave foránea a Servicio
  - `IdLugar` (int) - Clave foránea a Lugar
  - `Fecha` (DateTime) - Fecha del registro
  - `Hora` (TimeSpan) - Hora del registro
- Agregar propiedades de navegación:
  - `Empleado Empleado` - Relación N:1 con Empleado (nullable)
  - `Empresa Empresa` - Relación N:1 con Empresa
  - `Servicio Servicio` - Relación N:1 con Servicio
  - `Lugar Lugar` - Relación N:1 con Lugar
- Agregar propiedad calculada:
  - `HoraFormateada` (string, solo lectura) - Retorna `Hora.ToString(@"hh\:mm")`
- Aplicar Data Annotations:
  - `[Required]` y `[DataType(DataType.Date)]` en `Fecha`
  - `[Required]` y `[DataType(DataType.Time)]` en `Hora`
  - `[Display(Name = "Fecha")]` en `Fecha`
  - `[Display(Name = "Hora")]` en `Hora`
  - `[NotMapped]` en `HoraFormateada`
- **IMPORTANTE**: `IdEmpleado` debe ser `int?` (nullable) para permitir registros de invitados sin empleado asociado
- **Nota**: Eliminar las propiedades de solo lectura del proyecto original: `NombreEmpleado`, `NombreEmpresa`, `NombreLugar`

#### 7. Agregar Usings Necesarios
- En cada archivo de modelo, agregar los namespaces necesarios:
  ```csharp
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  ```

#### 8. Verificación y Compilación
- Compilar el proyecto con `dotnet build`
- Verificar que no hay errores de compilación
- Revisar que todas las clases estén en el namespace correcto (ej: `SCA_MVC.Models`)
- Verificar que todas las propiedades de navegación estén declaradas (aunque aún no configuradas)

### Resultado Esperado
Un proyecto con:
- **5 modelos de dominio** en la carpeta `Models/`:
  - `Empresa.cs` - Con propiedades básicas y navegación
  - `Empleado.cs` - Con propiedades básicas, navegación y `NombreCompleto`
  - `Lugar.cs` - Con propiedades básicas y navegación
  - `Servicio.cs` - Con propiedades básicas, navegación, `TotalGeneral` y `Estado`
  - `Registro.cs` - Con propiedades básicas, navegación y `HoraFormateada`
- **Data Annotations** aplicadas para validación en todos los modelos
- **Propiedades calculadas** marcadas con `[NotMapped]`
- **Propiedades de navegación** declaradas (sin configuración de relaciones aún)
- **Proyecto compilando sin errores**
- **Modelos listos** para ser usados con Entity Framework Core

### Mensaje de Commit
```
feat: crear modelos de dominio con validaciones

- Crear 5 clases de dominio en Models/ (Empresa, Empleado, Lugar, Servicio, Registro)
- Aplicar Data Annotations para validación (Required, StringLength, Display, DataType)
- Declarar propiedades de navegación para relaciones futuras
- Marcar propiedades calculadas con [NotMapped] (NombreCompleto, TotalGeneral, Estado, HoraFormateada)
- Configurar IdEmpleado como nullable en Registro para permitir invitados
- Eliminar propiedades redundantes del proyecto WinForms original
```

---

## Commit 3: DbContext y Configuración de Relaciones con Fluent API

### Objetivo
Crear el ApplicationDbContext y configurar todas las relaciones entre entidades usando **Fluent API**, estableciendo la base de la infraestructura de acceso a datos.

### ¿Qué es Fluent API?
**Fluent API** es una forma de configurar Entity Framework usando código C# en lugar de atributos (Data Annotations). Es más poderosa y flexible porque permite configuraciones que no son posibles con atributos.

**¿Por qué usarla?**
- ✅ Control total sobre **comportamientos de eliminación** (Restrict, SetNull, Cascade)
- ✅ Permite configurar **índices únicos compuestos** (imposible con Data Annotations)
- ✅ Configuración de **valores por defecto** a nivel de base de datos
- ✅ Documentación clara de todas las relaciones en un solo lugar
- ✅ Separación de responsabilidades (validación vs configuración de BD)

---

### Paso a Paso

#### 1. Creación de la Carpeta Data
- Crear la carpeta `Data/` en la raíz del proyecto (al mismo nivel que `Models/`)
- Esta carpeta contendrá toda la lógica de acceso a datos

#### 2. Creación del ApplicationDbContext
- Crear el archivo `Data/ApplicationDbContext.cs`
- Este será el "puente" entre tu aplicación y la base de datos

**Estructura básica**:
```csharp
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Models;

namespace SCA_MVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor: recibe opciones de configuración
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets: representan las tablas en la BD
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Registro> Registros { get; set; }

        // OnModelCreating: aquí va toda la configuración Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Aquí irán las configuraciones de relaciones
        }
    }
}
```

**Explicación**:
- `DbContext`: Clase base de EF Core que maneja la conexión a la BD
- `DbSet<T>`: Representa una tabla en la BD (permite hacer consultas LINQ)
- `OnModelCreating`: Método donde configuramos las relaciones, índices, etc.

#### 3. Configuración de Relaciones con Fluent API

**📚 Conceptos Básicos de Fluent API**:

Fluent API usa un patrón de "construcción" donde encadenas métodos:
```csharp
modelBuilder.Entity<Entidad>()  // Selecciona la entidad
    .HasOne(x => x.Propiedad)    // Define "tiene uno"
    .WithMany(x => x.Coleccion)  // Define "con muchos"
    .HasForeignKey(x => x.FK)    // Define la clave foránea
    .OnDelete(DeleteBehavior.X); // Define qué pasa al eliminar
```

---

**A. Configuración: Empresa → Empleados (1:N)**

**¿Qué significa?**: Una empresa tiene muchos empleados, un empleado pertenece a una empresa.

```csharp
// Configurar relación Empresa → Empleados
modelBuilder.Entity<Empresa>()
    .HasMany(e => e.Empleados)           // Una Empresa tiene muchos Empleados
    .WithOne(emp => emp.Empresa)         // Cada Empleado tiene una Empresa
    .HasForeignKey(emp => emp.IdEmpresa) // La FK es IdEmpresa
    .OnDelete(DeleteBehavior.Restrict)   // NO permitir eliminar Empresa si tiene Empleados
    .HasConstraintName("FK_Empleados_Empresa"); // Nombre del constraint en BD
```

**Explicación línea por línea**:
1. `Entity<Empresa>()` - Estamos configurando la entidad Empresa
2. `HasMany(e => e.Empleados)` - Empresa tiene una colección de Empleados
3. `WithOne(emp => emp.Empresa)` - Cada Empleado tiene UNA Empresa
4. `HasForeignKey(emp => emp.IdEmpresa)` - La columna FK es IdEmpresa
5. `OnDelete(DeleteBehavior.Restrict)` - Si intentas eliminar una Empresa con empleados, dará error
6. `HasConstraintName(...)` - Nombre personalizado del constraint en la BD

---

**B. Configuración: Empresa → Registros (1:N)**

```csharp
// Configurar relación Empresa → Registros
modelBuilder.Entity<Empresa>()
    .HasMany(e => e.Registros)
    .WithOne(r => r.Empresa)
    .HasForeignKey(r => r.IdEmpresa)
    .OnDelete(DeleteBehavior.Restrict)
    .HasConstraintName("FK_Registros_Empresa");
```

**¿Por qué Restrict?**: No queremos que al eliminar una Empresa se borren todos sus registros históricos.

---

**C. Configuración: Empleado → Registros (1:N, NULLABLE)**

**⚠️ IMPORTANTE**: Esta relación es especial porque `IdEmpleado` es nullable (permite invitados).

```csharp
// Configurar relación Empleado → Registros (NULLABLE)
modelBuilder.Entity<Empleado>()
    .HasMany(e => e.Registros)
    .WithOne(r => r.Empleado)
    .HasForeignKey(r => r.IdEmpleado)
    .OnDelete(DeleteBehavior.SetNull)  // ⚠️ SetNull: si eliminas empleado, IdEmpleado = null
    .IsRequired(false)                  // ⚠️ La relación es opcional
    .HasConstraintName("FK_Registros_Empleado");
```

**Explicación especial**:
- `OnDelete(DeleteBehavior.SetNull)` - Si eliminas un empleado, sus registros NO se borran, solo se pone `IdEmpleado = null`
- `IsRequired(false)` - Indica que la FK puede ser null (permite invitados sin empleado)

---

**D. Configuración: Lugar → Servicios (1:N)**

```csharp
// Configurar relación Lugar → Servicios
modelBuilder.Entity<Lugar>()
    .HasMany(l => l.Servicios)
    .WithOne(s => s.Lugar)
    .HasForeignKey(s => s.IdLugar)
    .OnDelete(DeleteBehavior.Restrict)
    .HasConstraintName("FK_Servicios_Lugar");
```

---

**E. Configuración: Lugar → Registros (1:N)**

```csharp
// Configurar relación Lugar → Registros
modelBuilder.Entity<Lugar>()
    .HasMany(l => l.Registros)
    .WithOne(r => r.Lugar)
    .HasForeignKey(r => r.IdLugar)
    .OnDelete(DeleteBehavior.Restrict)
    .HasConstraintName("FK_Registros_Lugar");
```

---

**F. Configuración: Servicio → Registros (1:N)**

```csharp
// Configurar relación Servicio → Registros
modelBuilder.Entity<Servicio>()
    .HasMany(s => s.Registros)
    .WithOne(r => r.Servicio)
    .HasForeignKey(r => r.IdServicio)
    .OnDelete(DeleteBehavior.Restrict)
    .HasConstraintName("FK_Registros_Servicio");
```

---

#### 4. Registro del DbContext en Program.cs

**📚 ¿Qué es la inyección de dependencias?**
Es un patrón que permite que ASP.NET Core "inyecte" automáticamente el DbContext donde lo necesites.

**Pasos**:
1. Abrir `Program.cs`
2. Agregar los namespaces:
```csharp
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
```

3. Registrar el DbContext ANTES de `var app = builder.Build();`:
```csharp
// Registrar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

**Explicación**:
- `AddDbContext<ApplicationDbContext>` - Registra el DbContext en el contenedor de DI
- `UseSqlServer(...)` - Indica que usaremos SQL Server
- `GetConnectionString("DefaultConnection")` - Lee la cadena de conexión de appsettings.json

---

#### 5. Verificación y Compilación

**Pasos finales**:
1. Compilar el proyecto: `dotnet build`
2. Verificar que no hay errores
3. Revisar que todas las configuraciones estén en `OnModelCreating`

**Checklist de verificación**:
- ✅ ApplicationDbContext creado en `Data/`
- ✅ 5 DbSets declarados
- ✅ 6 relaciones configuradas con Fluent API
- ✅ Nombres de constraints personalizados
- ✅ Comportamientos de eliminación configurados
- ✅ DbContext registrado en Program.cs
- ✅ Proyecto compila sin errores

### Resultado Esperado
Un proyecto con:
- **ApplicationDbContext** creado en `Data/` con:
  - 5 DbSets declarados (Empresas, Empleados, Lugares, Servicios, Registros)
  - Constructor configurado correctamente
  - Método `OnModelCreating` con 6 relaciones configuradas (~80 líneas)
- **Relaciones configuradas** entre todas las entidades:
  - 6 relaciones principales con nombres de constraints personalizados
  - Comportamientos de eliminación apropiados (Restrict, SetNull)
  - Relación nullable configurada correctamente (Empleado → Registros)
- **DbContext registrado** en `Program.cs` con inyección de dependencias
- **Proyecto compilando sin errores**
- **Base lista** para agregar índices y optimizaciones en el siguiente commit

### Mensaje de Commit
```
feat: crear DbContext y configurar relaciones con Fluent API

- Crear ApplicationDbContext en Data/ con 5 DbSets
- Configurar 6 relaciones entre entidades usando Fluent API
- Establecer nombres personalizados para constraints (FK_Empleados_Empresa, etc.)
- Configurar comportamientos de eliminación (Restrict para preservar datos históricos)
- Configurar relación nullable Empleado→Registros con SetNull (permite invitados)
- Registrar DbContext en Program.cs con inyección de dependencias
```

---

## Commit 4: Índices, Constraints y Optimizaciones de Base de Datos

### Objetivo
Agregar índices únicos, valores por defecto, check constraints e índices de performance al DbContext para optimizar y asegurar la integridad de la base de datos.

### Paso a Paso

#### 1. Configuración de Índices Únicos

**📚 ¿Qué es un índice único?**
Un índice único garantiza que no haya valores duplicados en una columna (o combinación de columnas).

**A. Índice Único Simple: Empleado.IdCredencial**

**¿Por qué?**: No puede haber dos empleados con la misma credencial RFID.

Agregar en `OnModelCreating`:
```csharp
// Índice único en IdCredencial
modelBuilder.Entity<Empleado>()
    .HasIndex(e => e.IdCredencial)      // Crear índice en IdCredencial
    .IsUnique()                          // Hacerlo único
    .HasDatabaseName("IX_Empleado_IdCredencial"); // Nombre del índice en BD
```

**Explicación**:
- `HasIndex(e => e.IdCredencial)` - Crea un índice en la columna IdCredencial
- `IsUnique()` - Lo hace único (no permite duplicados)
- `HasDatabaseName(...)` - Nombre personalizado del índice

---

**B. Índice Único Compuesto: Registro (IdEmpleado, IdServicio)**

**¿Por qué?**: Un empleado no puede registrarse dos veces en el mismo servicio.

```csharp
// Índice único compuesto en (IdEmpleado, IdServicio)
modelBuilder.Entity<Registro>()
    .HasIndex(r => new { r.IdEmpleado, r.IdServicio }) // Índice en AMBAS columnas
    .IsUnique()                                         // Hacerlo único
    .HasFilter("[IdEmpleado] IS NOT NULL")             // ⚠️ Solo si IdEmpleado no es null
    .HasDatabaseName("IX_Registro_Empleado_Servicio");
```

**Explicación especial**:
- `new { r.IdEmpleado, r.IdServicio }` - Índice compuesto (dos columnas)
- `HasFilter("[IdEmpleado] IS NOT NULL")` - **MUY IMPORTANTE**: Como IdEmpleado es nullable, solo aplicamos el constraint cuando NO es null (invitados pueden registrarse múltiples veces)

---

#### 2. Configuración de Valores por Defecto

**📚 ¿Qué son valores por defecto?**
Valores que la base de datos asigna automáticamente si no se especifican al insertar.

```csharp
// Valores por defecto para Estado = true
modelBuilder.Entity<Empresa>()
    .Property(e => e.Estado)
    .HasDefaultValue(true);

modelBuilder.Entity<Empleado>()
    .Property(e => e.Estado)
    .HasDefaultValue(true);

modelBuilder.Entity<Lugar>()
    .Property(l => l.Estado)
    .HasDefaultValue(true);

// Valores por defecto para totales = 0
modelBuilder.Entity<Servicio>()
    .Property(s => s.TotalComensales)
    .HasDefaultValue(0);

modelBuilder.Entity<Servicio>()
    .Property(s => s.TotalInvitados)
    .HasDefaultValue(0);
```

**Explicación**:
- `Property(x => x.Propiedad)` - Selecciona una propiedad específica
- `HasDefaultValue(valor)` - Define el valor por defecto en la BD

---

#### 3. Configuración de Check Constraints

**📚 ¿Qué es un Check Constraint?**
Una regla de validación que se aplica directamente en la base de datos.

```csharp
// Check constraint: Fecha no puede ser futura
modelBuilder.Entity<Servicio>()
    .ToTable(t => t.HasCheckConstraint(
        "CK_Servicio_Fecha",           // Nombre del constraint
        "[Fecha] <= CAST(GETDATE() AS DATE)")); // Condición SQL
```

**Explicación**:
- `ToTable(t => ...)` - Configuración a nivel de tabla
- `HasCheckConstraint(nombre, condición)` - Crea un constraint de validación
- `[Fecha] <= GETDATE()` - La fecha no puede ser mayor a hoy

---

#### 4. Configuración de Índices para Performance

**📚 ¿Por qué índices adicionales?**
Mejoran el rendimiento de consultas frecuentes.

```csharp
// Índice compuesto para búsquedas por Fecha y Lugar
modelBuilder.Entity<Servicio>()
    .HasIndex(s => new { s.Fecha, s.IdLugar })
    .HasDatabaseName("IX_Servicio_Fecha_Lugar");

// Índice para búsquedas por Fecha en Registros
modelBuilder.Entity<Registro>()
    .HasIndex(r => r.Fecha)
    .HasDatabaseName("IX_Registro_Fecha");
```

**Explicación**:
Estos índices aceleran consultas como:
- "Mostrar servicios de un lugar en una fecha específica"
- "Mostrar registros de una fecha específica"

---

#### 5. Verificación y Compilación

**Pasos finales**:
1. Compilar el proyecto: `dotnet build`
2. Verificar que no hay errores
3. Revisar que todas las configuraciones estén en `OnModelCreating`

**Checklist de verificación**:
- ✅ 2 índices únicos configurados
- ✅ 5 valores por defecto configurados
- ✅ 1 check constraint configurado
- ✅ 2 índices de performance configurados
- ✅ Proyecto compila sin errores

### Resultado Esperado
Un proyecto con:
- **Índices únicos** configurados:
  - Índice único simple en `Empleado.IdCredencial`
  - Índice único compuesto en `Registro (IdEmpleado, IdServicio)` con filtro para nulls
- **Valores por defecto** configurados:
  - Estado = true (Empresa, Empleado, Lugar)
  - TotalComensales = 0, TotalInvitados = 0 (Servicio)
- **Check Constraints** configurados:
  - Validación de fecha no futura en Servicio
- **Índices de performance** configurados:
  - Índice compuesto (Fecha, IdLugar) en Servicio
  - Índice simple (Fecha) en Registro
- **ApplicationDbContext completo** (~150 líneas) con todas las configuraciones
- **Proyecto compilando sin errores**
- **Infraestructura lista** para crear migraciones

### Mensaje de Commit
```
feat: agregar índices, constraints y optimizaciones a DbContext

- Configurar índice único en Empleado.IdCredencial (evitar credenciales duplicadas)
- Configurar índice único compuesto en Registro (IdEmpleado, IdServicio) con filtro para nulls
- Configurar valores por defecto (Estado=true, TotalComensales=0, TotalInvitados=0)
- Agregar check constraint para validar fecha no futura en Servicio
- Agregar índices de performance para búsquedas por fecha (Servicio, Registro)
```

---

## Commit 5: Creación de Base de Datos con Migraciones

### Objetivo
Crear la primera migración de Entity Framework Core y aplicarla a SQL Server, generando la base de datos BD_Control_Almuerzos con todas las tablas, relaciones y constraints.

### Paso a Paso

#### 1. Verificación de Prerrequisitos
- Verificar que SQL Server esté ejecutándose
- Verificar que la cadena de conexión en `appsettings.json` sea correcta
- Verificar que el nombre de la base de datos sea `BD_Control_Almuerzos`
- Compilar el proyecto para asegurar que no hay errores: `dotnet build`

#### 2. Instalación de Herramientas EF Core (si no están instaladas)
- Verificar si las herramientas están instaladas: `dotnet ef --version`
- Si no están instaladas, ejecutar: `dotnet tool install --global dotnet-ef`
- Si están desactualizadas, ejecutar: `dotnet tool update --global dotnet-ef`

#### 3. Creación de la Primera Migración
- Abrir la terminal en la carpeta del proyecto `SCA-MVC/`
- Ejecutar el comando: `dotnet ef migrations add InitialCreate`
- Esperar a que se genere la migración
- Verificar que se creó la carpeta `Migrations/` en el proyecto

#### 4. Revisión del Archivo de Migración
- Abrir el archivo de migración generado en `Migrations/XXXXXX_InitialCreate.cs`
- Verificar que se hayan creado las 5 tablas:
  - `Empresas` con columnas: IdEmpresa, Nombre, Estado
  - `Empleados` con columnas: IdEmpleado, Nombre, Apellido, IdCredencial, IdEmpresa, Estado
  - `Lugares` con columnas: IdLugar, Nombre, Estado
  - `Servicios` con columnas: IdServicio, IdLugar, Fecha, Proyeccion, DuracionMinutos, TotalComensales, TotalInvitados
  - `Registros` con columnas: IdRegistro, IdEmpleado (nullable), IdEmpresa, IdServicio, IdLugar, Fecha, Hora

#### 5. Verificación de Claves Primarias
- Verificar que todas las tablas tengan claves primarias (IDENTITY):
  - `IdEmpresa` en Empresas
  - `IdEmpleado` en Empleados
  - `IdLugar` en Lugares
  - `IdServicio` en Servicios
  - `IdRegistro` en Registros

#### 6. Verificación de Claves Foráneas
- Verificar que se hayan creado las Foreign Keys:
  - `Empleados.IdEmpresa` → `Empresas.IdEmpresa`
  - `Servicios.IdLugar` → `Lugares.IdLugar`
  - `Registros.IdEmpleado` → `Empleados.IdEmpleado` (nullable)
  - `Registros.IdEmpresa` → `Empresas.IdEmpresa`
  - `Registros.IdServicio` → `Servicios.IdServicio`
  - `Registros.IdLugar` → `Lugares.IdLugar`

#### 7. Verificación de Índices Únicos
- Verificar que se haya creado el índice único en `Empleados.IdCredencial`
- Verificar que se haya creado el constraint único compuesto en `Registros (IdEmpleado, IdServicio)`

#### 8. Verificación de Valores por Defecto
- Verificar que se hayan configurado los valores por defecto:
  - `Estado = true` en Empresas, Empleados, Lugares
  - `TotalComensales = 0` en Servicios
  - `TotalInvitados = 0` en Servicios

#### 9. Corrección de Errores (si los hay)
- Si hay errores en la migración:
  - Eliminar la migración con: `dotnet ef migrations remove`
  - Corregir el DbContext o los modelos según el error
  - Compilar nuevamente: `dotnet build`
  - Volver a crear la migración: `dotnet ef migrations add InitialCreate`

#### 10. Aplicación de la Migración a la Base de Datos
- Ejecutar el comando: `dotnet ef database update`
- Esperar a que se complete el proceso
- Verificar que no haya errores en la consola
- Confirmar el mensaje de éxito: "Done"

#### 11. Verificación en SQL Server
- Abrir SQL Server Management Studio (SSMS) o Azure Data Studio
- Conectarse al servidor configurado en la cadena de conexión
- Verificar que se haya creado la base de datos `BD_Control_Almuerzos`
- Expandir la base de datos y verificar las tablas:
  - dbo.Empresas
  - dbo.Empleados
  - dbo.Lugares
  - dbo.Servicios
  - dbo.Registros

#### 12. Verificación de Estructura de Tablas
- Para cada tabla, verificar:
  - Columnas con tipos de datos correctos
  - Claves primarias configuradas
  - Claves foráneas establecidas
  - Índices únicos creados
  - Valores por defecto aplicados
  - Constraints de integridad referencial

#### 13. Verificación de Relaciones
- En SSMS, expandir cada tabla y revisar la sección "Keys"
- Verificar que las Foreign Keys estén correctamente configuradas
- Verificar los comportamientos de eliminación (Restrict, SetNull)

#### 14. Prueba de Conexión desde la Aplicación
- Ejecutar la aplicación: `dotnet run`
- Verificar que la aplicación inicie sin errores de conexión a la base de datos
- Detener la aplicación (Ctrl+C)

### Resultado Esperado
Un proyecto con:
- **Carpeta `Migrations/`** creada con:
  - Archivo de migración `XXXXXX_InitialCreate.cs`
  - Archivo de snapshot `ApplicationDbContextModelSnapshot.cs`
- **Base de datos `BD_Control_Almuerzos`** creada en SQL Server con:
  - 5 tablas con estructura correcta
  - Todas las columnas con tipos de datos apropiados
  - Claves primarias (IDENTITY) en todas las tablas
  - 6 claves foráneas configuradas correctamente
  - 2 índices únicos (IdCredencial, constraint compuesto)
  - Valores por defecto aplicados
  - Constraints de integridad referencial
- **Migración aplicada exitosamente**
- **Aplicación conectándose correctamente** a la base de datos
- **Infraestructura de datos lista** para implementar funcionalidades

### Mensaje de Commit
```
feat: crear base de datos con migraciones de Entity Framework

- Crear migración InitialCreate con 5 tablas (Empresas, Empleados, Lugares, Servicios, Registros)
- Configurar claves primarias (IDENTITY) en todas las tablas
- Establecer 6 claves foráneas con relaciones correctas
- Aplicar índice único en Empleados.IdCredencial
- Aplicar constraint único compuesto en Registros (IdEmpleado, IdServicio)
- Configurar valores por defecto (Estado, TotalComensales, TotalInvitados)
- Aplicar migración para generar BD_Control_Almuerzos en SQL Server
- Verificar estructura de base de datos y conexión desde la aplicación
```

---

## Commit 6A: Layout Maestro y Navegación Base

### Objetivo
Crear la estructura HTML base de la aplicación con un layout maestro responsive y un menú de navegación funcional.

### Paso a Paso

#### 1. Modificar el Layout Maestro (_Layout.cshtml)

**Ubicación:** `Views/Shared/_Layout.cshtml`

**Cambios a realizar:**

**A. Actualizar el título de la aplicación**
```html
<title>@ViewData["Title"] - Sistema Control Almuerzos</title>
```

**B. Crear Navbar Responsive con Bootstrap**
```html
<header>
    <nav class="navbar navbar-expand-sm navbar-light bg-white border-bottom">
        <div class="container-fluid">
            <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">
                Sistema Control Almuerzos
            </a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" 
                    data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Home" asp-action="Index">Inicio</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Empresas" asp-action="Index">Empresas</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Empleados" asp-action="Index">Empleados</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Lugares" asp-action="Index">Lugares</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Servicios" asp-action="Index">Servicios</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" asp-controller="Registros" asp-action="Index">Registros</a>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</header>
```

**Explicación:**
- `navbar-expand-sm` - Navbar responsive (se colapsa en pantallas pequeñas)
- `navbar-toggler` - Botón "hamburguesa" para móviles
- `asp-controller` y `asp-action` - Tag Helpers que generan URLs automáticamente
- `ms-auto` - Alinea el menú a la derecha

**⚠️ NOTA:** Los controllers (Empresas, Empleados, etc.) no existen todavía. Los crearemos en unidades futuras. Por ahora, los links darán error 404, pero está bien.

**C. Mantener el área de contenido dinámico**
```html
<main role="main" class="pb-3">
    <div class="container">
        @RenderBody()
    </div>
</main>
```

**Explicación:**
- `@RenderBody()` - Aquí se inyecta el contenido de cada vista (Index.cshtml, etc.)

**D. Actualizar el Footer**
```html
<footer class="border-top footer text-muted">
    <div class="container">
        &copy; 2026 - Sistema Control Almuerzos
    </div>
</footer>
```

**E. Verificar Scripts de Bootstrap**
```html
<script src="~/lib/jquery/dist/jquery.min.js"></script>
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
<script src="~/js/site.js" asp-append-version="true"></script>
@await RenderSectionAsync("Scripts", required: false)
```

---

#### 2. Verificar _ViewStart.cshtml

**Ubicación:** `Views/_ViewStart.cshtml`

**Contenido esperado:**
```cshtml
@{
    Layout = "_Layout";
}
```

**Explicación:**
- Este archivo hace que todas las vistas usen `_Layout.cshtml` automáticamente
- No necesitas especificar el layout en cada vista individual

---

#### 3. Compilar y Verificar

**Pasos:**
1. Compilar: `dotnet build`
2. Ejecutar: `dotnet run`
3. Abrir navegador: `http://localhost:5000`

**Verificar:**
- ✅ Navbar aparece en la parte superior
- ✅ Links del navbar son clicables (aunque den 404)
- ✅ Footer aparece en la parte inferior
- ✅ Navbar es responsive (prueba redimensionar la ventana)

---

### Resultado Esperado
Un proyecto con:
- **Layout maestro** funcional con navbar y footer
- **Navegación responsive** que funciona en desktop y móvil
- **Estructura base** lista para agregar contenido
- **Proyecto compilando** sin errores

### Mensaje de Commit
```
feat: implementar layout maestro y navegación base

- Modificar _Layout.cshtml con navbar responsive de Bootstrap
- Crear menú de navegación con links a Empresas, Empleados, Lugares, Servicios, Registros
- Agregar footer básico con información de copyright
- Configurar scripts de Bootstrap y jQuery para funcionalidad responsive
- Verificar _ViewStart.cshtml para aplicar layout a todas las vistas
```

---

## Commit 6B: Página de Inicio, Estilos y Documentación MVC

### Objetivo
Crear una página de inicio profesional, personalizar los estilos de la aplicación y documentar la comprensión del patrón MVC.

### Paso a Paso

#### 1. Actualizar la Página de Inicio (Index.cshtml)

**Ubicación:** `Views/Home/Index.cshtml`

**Reemplazar el contenido con:**
```cshtml
@{
    ViewData["Title"] = "Inicio";
}

<div class="text-center">
    <h1 class="display-4">Bienvenido al Sistema de Control de Almuerzos</h1>
    <p class="lead">Gestión integral de servicios de comedor</p>
</div>

<div class="row mt-5">
    <div class="col-md-4 mb-4">
        <div class="card h-100">
            <div class="card-body">
                <h5 class="card-title">
                    <i class="bi bi-building"></i> Empresas
                </h5>
                <p class="card-text">Gestiona las empresas registradas en el sistema.</p>
                <a asp-controller="Empresas" asp-action="Index" class="btn btn-primary">Ver Empresas</a>
            </div>
        </div>
    </div>
    
    <div class="col-md-4 mb-4">
        <div class="card h-100">
            <div class="card-body">
                <h5 class="card-title">
                    <i class="bi bi-people"></i> Empleados
                </h5>
                <p class="card-text">Administra los empleados y sus credenciales RFID.</p>
                <a asp-controller="Empleados" asp-action="Index" class="btn btn-primary">Ver Empleados</a>
            </div>
        </div>
    </div>
    
    <div class="col-md-4 mb-4">
        <div class="card h-100">
            <div class="card-body">
                <h5 class="card-title">
                    <i class="bi bi-geo-alt"></i> Lugares
                </h5>
                <p class="card-text">Configura los lugares donde se prestan servicios.</p>
                <a asp-controller="Lugares" asp-action="Index" class="btn btn-primary">Ver Lugares</a>
            </div>
        </div>
    </div>
</div>

<div class="row">
    <div class="col-md-6 mb-4">
        <div class="card h-100">
            <div class="card-body">
                <h5 class="card-title">
                    <i class="bi bi-calendar-event"></i> Servicios
                </h5>
                <p class="card-text">Consulta y gestiona los servicios de comedor disponibles.</p>
                <a asp-controller="Servicios" asp-action="Index" class="btn btn-primary">Ver Servicios</a>
            </div>
        </div>
    </div>
    
    <div class="col-md-6 mb-4">
        <div class="card h-100">
            <div class="card-body">
                <h5 class="card-title">
                    <i class="bi bi-clipboard-check"></i> Registros
                </h5>
                <p class="card-text">Revisa los registros de asistencia a los servicios.</p>
                <a asp-controller="Registros" asp-action="Index" class="btn btn-primary">Ver Registros</a>
            </div>
        </div>
    </div>
</div>
```

**Explicación:**
- `@{ ViewData["Title"] = "Inicio"; }` - Define el título de la página
- `display-4` - Clase de Bootstrap para título grande
- `lead` - Clase de Bootstrap para texto destacado
- `row` y `col-md-4` - Sistema de grillas de Bootstrap (3 columnas)
- `card` - Componente de Bootstrap para tarjetas
- `h-100` - Altura 100% (todas las cards tienen la misma altura)
- `mb-4` - Margin bottom 4 (espacio inferior)

---

#### 2. Personalizar Estilos (site.css)

**Ubicación:** `wwwroot/css/site.css`

**Agregar al final del archivo:**
```css
/* ===== ESTILOS PERSONALIZADOS SISTEMA CONTROL ALMUERZOS ===== */

/* Estilos generales */
html {
  font-size: 14px;
  position: relative;
  min-height: 100%;
}

body {
  margin-bottom: 60px;
}

/* Navbar personalizado */
.navbar-brand {
  font-weight: bold;
  color: #0066cc !important;
  font-size: 1.2rem;
}

.nav-link {
  font-weight: 500;
  transition: color 0.3s ease;
}

.nav-link:hover {
  color: #0066cc !important;
}

.nav-link.active {
  color: #0066cc !important;
  font-weight: 600;
}

/* Footer */
.footer {
  position: absolute;
  bottom: 0;
  width: 100%;
  white-space: nowrap;
  line-height: 60px;
  background-color: #f8f9fa;
}

/* Cards en la página de inicio */
.card {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  border: 1px solid #dee2e6;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.card-title {
  color: #0066cc;
  font-weight: 600;
}

.card-title i {
  margin-right: 8px;
}

/* Botones */
.btn-primary {
  background-color: #0066cc;
  border-color: #0066cc;
  transition: all 0.3s ease;
}

.btn-primary:hover {
  background-color: #0052a3;
  border-color: #0052a3;
  transform: translateY(-2px);
  box-shadow: 0 2px 8px rgba(0,102,204,0.3);
}

/* Títulos */
.display-4 {
  color: #333;
  font-weight: 600;
}

.lead {
  color: #666;
}
```

**Explicación:**
- **Navbar:** Color azul corporativo (#0066cc), efectos hover
- **Cards:** Efecto de elevación al pasar el mouse, sombras suaves
- **Botones:** Color personalizado, efecto hover con elevación
- **Footer:** Fijo en la parte inferior, fondo gris claro

---

#### 3. Crear Documento de Comprensión del Patrón MVC

**Ubicación:** `Guias/Comprension_MVC.md`

**Contenido:**
```markdown
# Comprensión del Patrón MVC en Sistema Control Almuerzos

## ¿Qué es MVC?

MVC (Model-View-Controller) es un patrón de arquitectura de software que separa la aplicación en tres componentes principales:

### 1. Model (Modelo)
**Responsabilidad:** Representa los datos y la lógica de negocio de la aplicación.

**En mi proyecto:**
- `Models/Empresa.cs` - Representa una empresa
- `Models/Empleado.cs` - Representa un empleado
- `Models/Lugar.cs` - Representa un lugar de servicio
- `Models/Servicio.cs` - Representa un servicio de comedor
- `Models/Registro.cs` - Representa un registro de asistencia

**Ejemplo:**
```csharp
public class Empleado
{
    [Key]
    public int IdEmpleado { get; set; }
    
    [Required]
    public string Nombre { get; set; }
    
    public int IdEmpresa { get; set; }
    
    // Propiedades de navegación
    public Empresa? Empresa { get; set; }
}
```

### 2. View (Vista)
**Responsabilidad:** Presenta los datos al usuario (interfaz de usuario HTML).

**En mi proyecto:**
- `Views/Shared/_Layout.cshtml` - Layout maestro (navbar, footer)
- `Views/Home/Index.cshtml` - Página de inicio
- `Views/_ViewStart.cshtml` - Configuración global de layout

**Ejemplo:**
```cshtml
@{
    ViewData["Title"] = "Inicio";
}

<h1>@ViewData["Title"]</h1>
<p>Bienvenido al sistema</p>
```

### 3. Controller (Controlador)
**Responsabilidad:** Maneja las peticiones HTTP, interactúa con el modelo y selecciona la vista a mostrar.

**En mi proyecto:**
- `Controllers/HomeController.cs` - Controlador de la página de inicio

**Ejemplo:**
```csharp
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();  // Retorna Views/Home/Index.cshtml
    }
}
```

---

## Flujo de una Petición HTTP en MVC

### Ejemplo: Usuario visita la página de inicio

1. **Usuario** hace clic en "Inicio" en el navbar
2. **Navegador** envía petición GET a `http://localhost:5000/Home/Index`
3. **Routing** (enrutamiento) analiza la URL:
   - Controller: `Home`
   - Action: `Index`
4. **ASP.NET Core** instancia `HomeController`
5. **Controller** ejecuta el método `Index()`
6. **Controller** retorna `View()` (sin parámetros = busca `Views/Home/Index.cshtml`)
7. **View Engine** (Razor):
   - Lee `Views/_ViewStart.cshtml` → Layout = "_Layout"
   - Lee `Views/Shared/_Layout.cshtml`
   - Lee `Views/Home/Index.cshtml`
   - Inyecta `Index.cshtml` en `@RenderBody()` de `_Layout.cshtml`
8. **View Engine** genera HTML final
9. **ASP.NET Core** envía el HTML al navegador
10. **Usuario** ve la página renderizada

---

## Diagrama del Flujo

```
Usuario → Navegador → ASP.NET Core → Routing
                            ↓
                       HomeController.Index()
                            ↓
                       return View();
                            ↓
                       View Engine (Razor)
                            ↓
                    _Layout.cshtml + Index.cshtml
                            ↓
                        HTML Final
                            ↓
                       Navegador → Usuario
```

---

## Ejemplo Concreto en Mi Proyecto

### URL: `http://localhost:5000/Home/Index`

#### 1. Routing (Program.cs)
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

**Mapeo:**
- `{controller=Home}` → `HomeController`
- `{action=Index}` → Método `Index()`
- `{id?}` → Parámetro opcional (no usado en este caso)

#### 2. Controller (HomeController.cs)
```csharp
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

#### 3. View (_ViewStart.cshtml)
```cshtml
@{
    Layout = "_Layout";
}
```

#### 4. Layout (_Layout.cshtml)
```cshtml
<!DOCTYPE html>
<html>
<head>
    <title>@ViewData["Title"] - Sistema Control Almuerzos</title>
</head>
<body>
    <nav>...</nav>
    
    <main>
        @RenderBody()  ← Aquí se inyecta Index.cshtml
    </main>
    
    <footer>...</footer>
</body>
</html>
```

#### 5. Vista Específica (Index.cshtml)
```cshtml
@{
    ViewData["Title"] = "Inicio";
}

<h1>Bienvenido al Sistema de Control de Almuerzos</h1>
```

#### 6. HTML Final Generado
```html
<!DOCTYPE html>
<html>
<head>
    <title>Inicio - Sistema Control Almuerzos</title>
</head>
<body>
    <nav>...</nav>
    
    <main>
        <h1>Bienvenido al Sistema de Control de Almuerzos</h1>
    </main>
    
    <footer>...</footer>
</body>
</html>
```

---

## Ventajas del Patrón MVC

### 1. Separación de Responsabilidades
- **Modelo:** Solo maneja datos y lógica de negocio
- **Vista:** Solo maneja presentación
- **Controller:** Solo maneja flujo de control

### 2. Mantenibilidad
- Cambiar el diseño (Vista) no afecta la lógica (Controller/Model)
- Cambiar la lógica no afecta la presentación
- Código más organizado y fácil de mantener

### 3. Reutilización
- Un modelo puede usarse en múltiples vistas
- Un layout se reutiliza en todas las páginas
- Controllers pueden compartir lógica común

### 4. Testabilidad
- Puedes testear controllers sin la vista
- Puedes testear modelos independientemente
- Separación facilita unit testing

---

## Comparación con WinForms

### WinForms (Proyecto Anterior)
```
┌─────────────────────────┐
│ frmEmpleados.cs         │ ← Todo en un archivo
│ - UI (controles)        │
│ - Lógica de negocio     │
│ - Acceso a datos        │
└─────────────────────────┘
```

### MVC (Proyecto Actual)
```
┌─────────────────────────┐
│ Empleado.cs (Model)     │ ← Solo datos
└─────────────────────────┘
           ↓
┌─────────────────────────┐
│ EmpleadosController.cs  │ ← Solo lógica de control
└─────────────────────────┘
           ↓
┌─────────────────────────┐
│ Index.cshtml (View)     │ ← Solo presentación
└─────────────────────────┘
```

---

## Conceptos Clave Aprendidos

1. ✅ **Layout Maestro:** Plantilla reutilizable para todas las páginas
2. ✅ **@RenderBody():** Inyecta contenido dinámico en el layout
3. ✅ **Tag Helpers:** `asp-controller`, `asp-action` generan URLs
4. ✅ **ViewData:** Pasa datos del controller a la vista
5. ✅ **Routing:** Mapea URLs a controllers y acciones
6. ✅ **Razor:** Motor de vistas que mezcla C# con HTML
7. ✅ **Bootstrap:** Framework CSS para diseño responsive

---

## Próximos Pasos (Unidad 2)

En la siguiente unidad aprenderemos a:
- Crear controllers personalizados (EmpleadosController, EmpresasController)
- Implementar CRUD completo (Create, Read, Update, Delete)
- Pasar datos del controller a la vista usando ViewModels
- Usar Entity Framework para consultar la base de datos
- Crear formularios con validación
```

---

#### 4. Compilar y Verificar

**Pasos:**
1. Compilar: `dotnet build`
2. Ejecutar: `dotnet run`
3. Abrir navegador: `http://localhost:5000`

**Verificar:**
- ✅ Página de inicio muestra tarjetas de módulos
- ✅ Cards tienen efecto hover (elevación)
- ✅ Colores personalizados aplicados
- ✅ Navbar tiene color azul corporativo
- ✅ Botones tienen efecto hover

---

### Resultado Esperado
Un proyecto con:
- **Página de inicio profesional** con tarjetas de presentación
- **Estilos personalizados** aplicados (colores, efectos hover)
- **Documentación completa** del patrón MVC
- **Aplicación funcional** lista para desarrollo futuro

### Mensaje de Commit
```
feat: crear página de inicio y personalizar estilos

- Actualizar Index.cshtml con tarjetas de presentación de módulos principales
- Personalizar site.css con estilos para navbar, cards, botones y footer
- Agregar efectos hover y transiciones suaves
- Configurar colores corporativos (#0066cc) en toda la aplicación
- Crear documento Comprension_MVC.md explicando el patrón MVC aplicado
- Documentar flujo completo de peticiones HTTP en el proyecto
- Incluir comparación con arquitectura WinForms
```
- Documentar cómo se aplica el patrón MVC en este proyecto específico:
  - **Models**: Empleado, Empresa, Servicio, Comensal
  - **Views**: Vistas Razor en carpetas por controlador
  - **Controllers**: Controladores que manejarán las peticiones HTTP
- Explicar el flujo de una petición típica en el sistema
- Comparar la arquitectura con el proyecto WinForms original

#### 8. Configuración de ViewStart
- Verificar el archivo `Views/_ViewStart.cshtml`
- Asegurar que todas las vistas usen el layout por defecto
- Comprender cómo funciona la jerarquía de layouts

#### 9. Pruebas de Navegación
- Ejecutar la aplicación
- Verificar que el layout se muestre correctamente en todas las páginas
- Probar todos los enlaces del menú de navegación
- Verificar que los estilos se apliquen consistentemente
- Comprobar que la navegación entre páginas funcione correctamente
- Validar que el footer y header se muestren en todas las vistas

#### 10. Preparación para Desarrollo Futuro
- Crear carpetas vacías en `Views/` para los futuros controladores:
  - `Views/Empleados/`
  - `Views/Empresas/`
  - `Views/Servicios/`
  - `Views/Reportes/`
- Documentar la estructura de carpetas y convenciones de nombres
- Preparar un checklist de las funcionalidades a implementar en las siguientes unidades

### Resultado Esperado
Un proyecto con:
- Layout maestro personalizado con navegación funcional
- Estilos base configurados y consistentes
- Página de inicio actualizada con información del sistema
- Página About con documentación del proyecto
- Comprensión clara del patrón MVC aplicado al proyecto
- Estructura preparada para el desarrollo de módulos futuros

### Mensaje de Commit
```
feat: implementar layout base y estructura de navegación MVC

- Personalizar layout maestro (_Layout.cshtml) con branding del sistema
- Configurar menú de navegación principal con enlaces a módulos
- Definir estilos base en site.css (colores, tipografía, componentes)
- Actualizar vista Home/Index con página de bienvenida
- Crear vista About con información del sistema y arquitectura
- Verificar configuración de routing y ViewStart
- Preparar estructura de carpetas para futuros módulos (Empleados, Empresas, Servicios, Reportes)
```

---

## Resumen de los 7 Commits

### Commit 1: Configuración Inicial
**Enfoque**: Infraestructura y configuración base del proyecto.
**Entregable**: Proyecto MVC funcional con paquetes instalados y cadena de conexión configurada.
**Tiempo estimado**: ~15 minutos

### Commit 2: Modelos de Dominio
**Enfoque**: Creación de modelos con validaciones.
**Entregable**: 5 modelos (Empresa, Empleado, Lugar, Servicio, Registro) con Data Annotations y propiedades de navegación declaradas.
**Tiempo estimado**: ~20 minutos

### Commit 3: DbContext y Relaciones
**Enfoque**: Configuración de Entity Framework y relaciones.
**Entregable**: ApplicationDbContext con DbSets y 6 relaciones configuradas usando Fluent API.
**Tiempo estimado**: ~30 minutos

### Commit 4: Índices y Optimizaciones
**Enfoque**: Integridad y performance de base de datos.
**Entregable**: Índices únicos, valores por defecto, check constraints e índices de performance configurados.
**Tiempo estimado**: ~20 minutos

### Commit 5: Migraciones y Base de Datos
**Enfoque**: Generación y aplicación de esquema de base de datos.
**Entregable**: Migración InitialCreate aplicada, BD_Control_Almuerzos creada con 5 tablas, relaciones y constraints.
**Tiempo estimado**: ~15 minutos

### Commit 6A: Layout Maestro y Navegación
**Enfoque**: Estructura HTML base y navegación responsive.
**Entregable**: Layout funcional con navbar y footer, navegación implementada.
**Tiempo estimado**: ~20 minutos

### Commit 6B: Página de Inicio y Estilos
**Enfoque**: Contenido visual y documentación del patrón MVC.
**Entregable**: Página de inicio profesional, estilos personalizados, documento de comprensión MVC.
**Tiempo estimado**: ~15 minutos

---

## Notas Importantes

- Cada commit debe compilar sin errores
- Cada commit debe ser funcional y ejecutable
- Los commits siguen una progresión lógica: Configuración → Modelos → DbContext+Relaciones → Índices+Optimizaciones → BD → Layout → UI+Estilos
- Se sigue la convención de commits: `feat:` para nuevas funcionalidades, `fix:` para correcciones
- Los mensajes de commit son descriptivos y siguen el formato Conventional Commits
- **Tiempo total estimado**: ~2 horas 15 minutos
