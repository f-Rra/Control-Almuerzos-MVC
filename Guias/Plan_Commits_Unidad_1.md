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

## Commit 3: Configuración de Entity Framework Core

### Objetivo
Crear y configurar el DbContext con todas las relaciones entre entidades usando Fluent API, preparando la infraestructura de acceso a datos.

### Paso a Paso

#### 1. Creación de la Carpeta Data
- Crear la carpeta `Data/` en la raíz del proyecto (al mismo nivel que `Models/`)

#### 2. Creación del ApplicationDbContext
- Crear el archivo `Data/ApplicationDbContext.cs`
- Hacer que la clase herede de `DbContext`
- Agregar el constructor que recibe `DbContextOptions<ApplicationDbContext>`:
  ```csharp
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }
  ```
- Agregar los namespaces necesarios:
  ```csharp
  using Microsoft.EntityFrameworkCore;
  using SCA_MVC.Models;
  ```

#### 3. Declaración de DbSets
- Declarar las propiedades `DbSet` para cada entidad:
  ```csharp
  public DbSet<Empresa> Empresas { get; set; }
  public DbSet<Empleado> Empleados { get; set; }
  public DbSet<Lugar> Lugares { get; set; }
  public DbSet<Servicio> Servicios { get; set; }
  public DbSet<Registro> Registros { get; set; }
  ```

#### 4. Configuración del Método OnModelCreating
- Crear el método `OnModelCreating` que recibe `ModelBuilder`:
  ```csharp
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      base.OnModelCreating(modelBuilder);
      
      // Configuraciones aquí
  }
  ```

#### 5. Configuración de Relaciones con Fluent API

**Relación Empresa → Empleados (1:N)**
- Configurar que una Empresa tiene muchos Empleados
- Un Empleado pertenece a una Empresa
- Clave foránea: `IdEmpresa`
- Comportamiento de eliminación: `Restrict` (no permitir eliminar empresa con empleados)

**Relación Empresa → Registros (1:N)**
- Configurar que una Empresa tiene muchos Registros
- Un Registro pertenece a una Empresa
- Clave foránea: `IdEmpresa`
- Comportamiento de eliminación: `Restrict`

**Relación Empleado → Registros (1:N, opcional)**
- Configurar que un Empleado puede tener muchos Registros
- Un Registro puede pertenecer a un Empleado (nullable)
- Clave foránea: `IdEmpleado` (nullable)
- Comportamiento de eliminación: `SetNull` (si se elimina empleado, IdEmpleado queda null)

**Relación Lugar → Servicios (1:N)**
- Configurar que un Lugar tiene muchos Servicios
- Un Servicio pertenece a un Lugar
- Clave foránea: `IdLugar`
- Comportamiento de eliminación: `Restrict`

**Relación Lugar → Registros (1:N)**
- Configurar que un Lugar tiene muchos Registros
- Un Registro pertenece a un Lugar
- Clave foránea: `IdLugar`
- Comportamiento de eliminación: `Restrict`

**Relación Servicio → Registros (1:N)**
- Configurar que un Servicio tiene muchos Registros
- Un Registro pertenece a un Servicio
- Clave foránea: `IdServicio`
- Comportamiento de eliminación: `Restrict`

#### 6. Configuración de Índices Únicos

**Índice único en Empleado.IdCredencial**
- Configurar índice único para evitar credenciales duplicadas
- Usar `HasIndex(e => e.IdCredencial).IsUnique()`

**Constraint único compuesto en Registro**
- Configurar índice único compuesto en `(IdEmpleado, IdServicio)`
- Evitar que un empleado se registre dos veces en el mismo servicio
- Usar `HasIndex(r => new { r.IdEmpleado, r.IdServicio }).IsUnique()`

#### 7. Configuración de Valores por Defecto
- Configurar valor por defecto para `Empresa.Estado = true`
- Configurar valor por defecto para `Empleado.Estado = true`
- Configurar valor por defecto para `Lugar.Estado = true`
- Configurar valor por defecto para `Servicio.TotalComensales = 0`
- Configurar valor por defecto para `Servicio.TotalInvitados = 0`

#### 8. Configuración de Nombres de Tablas (Opcional)
- Verificar que los nombres de las tablas coincidan con la base de datos existente
- Si es necesario, configurar nombres explícitos con `ToTable("NombreTabla")`
- Por convención, EF Core usa el nombre del DbSet como nombre de tabla

#### 9. Registro del DbContext en Program.cs
- Abrir el archivo `Program.cs`
- Agregar los namespaces necesarios:
  ```csharp
  using Microsoft.EntityFrameworkCore;
  using SCA_MVC.Data;
  ```
- Registrar el DbContext antes de `builder.Build()`:
  ```csharp
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  ```
- Verificar que la cadena de conexión "DefaultConnection" existe en `appsettings.json`

#### 10. Verificación y Compilación
- Compilar el proyecto con `dotnet build`
- Verificar que no hay errores de compilación
- Verificar que el DbContext está correctamente registrado
- Revisar que todas las relaciones estén configuradas

### Resultado Esperado
Un proyecto con:
- **ApplicationDbContext** creado en `Data/` con:
  - 5 DbSets declarados (Empresas, Empleados, Lugares, Servicios, Registros)
  - Constructor configurado correctamente
  - Método `OnModelCreating` con configuración Fluent API
- **Relaciones configuradas** entre todas las entidades:
  - 6 relaciones principales (Empresa-Empleados, Empresa-Registros, Empleado-Registros, Lugar-Servicios, Lugar-Registros, Servicio-Registros)
  - Comportamientos de eliminación apropiados (Restrict, SetNull)
- **Índices únicos** configurados:
  - Índice único en `Empleado.IdCredencial`
  - Constraint único compuesto en `Registro (IdEmpleado, IdServicio)`
- **Valores por defecto** configurados para propiedades Estado y totales
- **DbContext registrado** en `Program.cs` con conexión a SQL Server
- **Proyecto compilando sin errores**
- **Infraestructura lista** para crear migraciones

### Mensaje de Commit
```
feat: configurar Entity Framework Core con DbContext

- Crear ApplicationDbContext en Data/ con 5 DbSets
- Configurar 6 relaciones entre entidades usando Fluent API
- Establecer comportamientos de eliminación (Restrict, SetNull)
- Configurar índice único en Empleado.IdCredencial
- Configurar constraint único compuesto en Registro (IdEmpleado, IdServicio)
- Configurar valores por defecto (Estado=true, TotalComensales=0, TotalInvitados=0)
- Registrar DbContext en Program.cs con conexión a SQL Server
```

---

## Commit 4: Creación de Base de Datos con Migraciones

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

## Commit 5: Implementación de Layout Base y Comprensión del Patrón MVC

### Objetivo
Configurar el layout maestro de la aplicación, implementar la navegación básica y documentar la comprensión del patrón MVC aplicado al proyecto.

### Paso a Paso

#### 1. Personalización del Layout Maestro
- Abrir el archivo `Views/Shared/_Layout.cshtml`
- Modificar el título de la aplicación a "Sistema de Control de Almuerzos"
- Actualizar el logo o nombre de la aplicación en el header
- Configurar el menú de navegación principal con enlaces a los módulos futuros
- Agregar un footer con información de copyright y versión
- Asegurar que el layout sea responsive y se vea bien en diferentes dispositivos

#### 2. Configuración de Estilos Base
- Revisar el archivo `wwwroot/css/site.css`
- Definir variables CSS para colores corporativos del sistema
- Establecer estilos base para tipografía (fuentes, tamaños, pesos)
- Configurar estilos para el header y footer
- Definir estilos para la navegación principal
- Agregar estilos para mensajes de alerta y notificaciones
- Establecer un esquema de colores consistente

#### 3. Estructura del Menú de Navegación
- Crear enlaces en el navbar para los módulos principales:
  - Inicio (Home)
  - Servicios (placeholder para futuro desarrollo)
  - Empleados (placeholder para futuro desarrollo)
  - Empresas (placeholder para futuro desarrollo)
  - Reportes (placeholder para futuro desarrollo)
  - Configuración (placeholder para futuro desarrollo)
- Configurar los enlaces usando HTML Helpers para generar URLs correctas
- Asegurar que el menú indique visualmente la página activa

#### 4. Configuración de Routing
- Revisar la configuración de rutas en `Program.cs`
- Verificar que la ruta por defecto apunte a `Home/Index`
- Comprender el patrón de URL: `/{controller}/{action}/{id?}`
- Documentar cómo las URLs se mapean a controladores y acciones
- Preparar el routing para los futuros controladores

#### 5. Actualización de la Vista Home/Index
- Modificar la vista `Views/Home/Index.cshtml`
- Crear una página de bienvenida al sistema
- Agregar una descripción breve del propósito del sistema
- Incluir tarjetas o secciones que presenten los módulos principales
- Agregar iconos o imágenes representativas (pueden ser placeholders)
- Asegurar que la vista use el layout maestro correctamente

#### 6. Creación de Vista About
- Crear una nueva acción `About()` en `HomeController.cs`
- Crear la vista correspondiente `Views/Home/About.cshtml`
- Agregar información sobre el sistema:
  - Propósito del sistema
  - Módulos principales
  - Tecnologías utilizadas (ASP.NET MVC, Entity Framework, SQL Server)
  - Comparación conceptual con la versión WinForms
- Agregar un enlace a esta vista en el footer

#### 7. Documentación del Patrón MVC en el Proyecto
- Crear un archivo de documentación `Docs/ArquitecturaMVC.md` (opcional)
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

## Resumen de los 5 Commits

### Commit 1: Configuración Inicial
**Enfoque**: Infraestructura y configuración base del proyecto.
**Entregable**: Proyecto MVC funcional con paquetes instalados y cadena de conexión configurada.

### Commit 2: Modelos de Dominio
**Enfoque**: Creación de modelos con validaciones.
**Entregable**: 5 modelos (Empresa, Empleado, Lugar, Servicio, Registro) con Data Annotations y propiedades de navegación declaradas.

### Commit 3: Configuración de Entity Framework
**Enfoque**: Infraestructura de acceso a datos.
**Entregable**: ApplicationDbContext con DbSets, relaciones Fluent API, índices únicos y valores por defecto configurados.

### Commit 4: Migraciones y Base de Datos
**Enfoque**: Generación y aplicación de esquema de base de datos.
**Entregable**: Migración InitialCreate aplicada, BD_Control_Almuerzos creada con 5 tablas, relaciones y constraints.

### Commit 5: UI Base y Arquitectura
**Enfoque**: Interfaz de usuario base y comprensión del patrón MVC.
**Entregable**: Layout funcional, navegación implementada y estructura preparada para desarrollo futuro.

---

## Notas Importantes

- Cada commit debe compilar sin errores
- Cada commit debe ser funcional y ejecutable
- Los commits siguen una progresión lógica: Configuración → Modelos → EF Core → BD → UI
- Se sigue la convención de commits: `feat:` para nuevas funcionalidades
- Los mensajes de commit son descriptivos y siguen el formato Conventional Commits
