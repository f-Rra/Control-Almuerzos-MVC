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

## Commit 2: Migración de Modelos de Dominio y Configuración de Entity Framework

### Objetivo
Migrar las clases de dominio desde el proyecto WinForms y configurar Entity Framework para el acceso a datos.

### Paso a Paso

#### 1. Migración de Modelos de Dominio
- Crear las clases de modelo en la carpeta `Models/` basándose en las clases existentes en `dominio/`
- Migrar la clase `Empleado.cs` con todas sus propiedades (Id, Nombre, Apellido, Credencial, etc.)
- Migrar la clase `Empresa.cs` con sus propiedades (Id, Nombre, RazonSocial, etc.)
- Migrar la clase `Servicio.cs` con sus propiedades (Id, FechaInicio, FechaFin, Estado, etc.)
- Migrar la clase `Comensal.cs` con sus propiedades (Id, IdServicio, IdEmpleado, FechaHora, etc.)
- Asegurar que todas las propiedades mantienen sus tipos de datos originales

#### 2. Configuración de Relaciones entre Entidades
- Definir las propiedades de navegación entre entidades relacionadas
- Configurar la relación entre `Empleado` y `Empresa` (muchos a uno)
- Configurar la relación entre `Comensal` y `Empleado` (muchos a uno)
- Configurar la relación entre `Comensal` y `Servicio` (muchos a uno)
- Configurar la relación entre `Servicio` y sus comensales (uno a muchos)

#### 3. Agregar Anotaciones de Datos (Data Annotations)
- Agregar atributos de validación a las propiedades de los modelos
- Marcar campos obligatorios con `[Required]`
- Definir longitudes máximas con `[MaxLength]` o `[StringLength]`
- Configurar claves primarias con `[Key]` si no siguen la convención
- Agregar atributos de visualización con `[Display]` para nombres amigables
- Configurar formatos de fecha con `[DataType]`

#### 4. Creación del DbContext
- Crear una nueva clase `ApplicationDbContext` en la carpeta `Data/`
- Hacer que la clase herede de `DbContext`
- Agregar un constructor que reciba `DbContextOptions<ApplicationDbContext>`
- Declarar propiedades `DbSet` para cada entidad (Empleados, Empresas, Servicios, Comensales)
- Configurar el método `OnModelCreating` para definir relaciones y restricciones adicionales

#### 5. Configuración de Relaciones Fluent API
- En el método `OnModelCreating`, configurar las relaciones entre entidades usando Fluent API
- Definir comportamientos de eliminación en cascada donde sea apropiado
- Configurar índices únicos para campos como credenciales de empleados
- Establecer restricciones de integridad referencial
- Configurar nombres de tablas si difieren de las convenciones

#### 6. Registro del DbContext en Program.cs
- Abrir el archivo `Program.cs`
- Agregar el servicio de DbContext al contenedor de dependencias
- Configurar el DbContext para usar SQL Server
- Pasar la cadena de conexión desde `appsettings.json`
- Verificar que el servicio esté correctamente registrado

#### 7. Creación de la Primera Migración
- Abrir la Consola del Administrador de Paquetes en Visual Studio
- Ejecutar el comando para crear la migración inicial
- Revisar el archivo de migración generado para verificar que las tablas y relaciones son correctas
- Verificar que se hayan creado las claves primarias, foráneas e índices esperados

#### 8. Aplicación de la Migración a la Base de Datos
- Ejecutar el comando para aplicar la migración a la base de datos
- Verificar que las tablas se hayan creado correctamente en SQL Server
- Comprobar que las relaciones entre tablas estén establecidas
- Validar que las restricciones y índices se hayan aplicado

### Resultado Esperado
Un proyecto con:
- Modelos de dominio migrados desde WinForms a la carpeta Models/
- Relaciones entre entidades correctamente configuradas
- DbContext configurado con DbSets para todas las entidades
- Base de datos creada con todas las tablas y relaciones
- Primera migración aplicada exitosamente

### Mensaje de Commit
```
feat: migrar modelos de dominio y configurar Entity Framework

- Migrar clases de dominio (Empleado, Empresa, Servicio, Comensal) a Models/
- Agregar Data Annotations para validación y configuración de entidades
- Crear ApplicationDbContext con DbSets para todas las entidades
- Configurar relaciones entre entidades usando Fluent API
- Registrar DbContext en Program.cs con conexión a SQL Server
- Crear y aplicar migración inicial para generar esquema de base de datos
```

---

## Commit 3: Implementación de Layout Base y Comprensión del Patrón MVC

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

## Resumen de los 3 Commits

### Commit 1: Configuración Inicial
**Enfoque**: Infraestructura y configuración base del proyecto.
**Entregable**: Proyecto MVC funcional con paquetes instalados y cadena de conexión configurada.

### Commit 2: Modelos y Base de Datos
**Enfoque**: Capa de datos y persistencia.
**Entregable**: Modelos migrados, DbContext configurado y base de datos creada con migraciones.

### Commit 3: UI Base y Arquitectura
**Enfoque**: Interfaz de usuario base y comprensión del patrón MVC.
**Entregable**: Layout funcional, navegación implementada y estructura preparada para desarrollo futuro.

---

## Notas Importantes

- Cada commit debe compilar sin errores
- Cada commit debe ser funcional y ejecutable
- Los commits siguen una progresión lógica: Configuración → Datos → UI
- Se sigue la convención de commits: `feat:` para nuevas funcionalidades
- Los mensajes de commit son descriptivos y siguen el formato Conventional Commits
