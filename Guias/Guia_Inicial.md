# Guía de Implementación Completa — Sistema Control de Almuerzos MVC

## Estado Actual del Proyecto

### Lo que ya está hecho
- Proyecto ASP.NET MVC (.NET 9) creado y configurado
- 5 Modelos con Data Annotations: `Empleado`, `Empresa`, `Lugar`, `Servicio`, `Registro`
- `ApplicationDbContext` con Fluent API completo (relaciones, índices, restricciones, defaults)
- Migración inicial aplicada (5 tablas con FKs, unique indexes, check constraints)
- Layout (`_Layout.cshtml`) con sidebar glassmorphism, topbar y footer
- Página de inicio (`Index.cshtml`) con diseño de dashboard (datos estáticos)
- `site.css` con sistema de diseño completo (variables, glassmorphism, sidebar, dashboard)
- Imágenes de iconos para navegación y dashboard en `wwwroot/images/`
- Conexión a SQL Server configurada (`BD_Control_Almuerzos`)

### Lo que falta
- Vistas para: Servicios, Registro Manual, Reportes, Configuración (Empresas/Empleados/Estadísticas)
- Controladores CRUD para todas las entidades
- Capa ADO.NET con stored procedures
- Migración a Entity Framework (reemplazo de ADO)
- Fluent API avanzado y Seeding
- Vistas parciales
- ASP.NET Identity (autenticación, roles, permisos)
- Manejo de archivos, envío de email, integración IA
- JavaScript interactivo (`site.js` está vacío)
- Lógica dinámica en Index (datos reales del servidor)

---

## Estructura de Commits por Unidad

---

## FASE 0 — Diseños de Vistas (cshtml)

> Antes de incorporar lógica, se completan todos los diseños de las vistas restantes con datos estáticos, manteniendo el mismo estilo glassmorphism del layout e Index.

---

### Commit 1 — Vista de Servicios

**Descripción:** Crear la vista principal de servicios (`Views/Servicio/Index.cshtml`) que unifica la configuración e inicio de servicio (panel superior de `frmPrincipal`) con el registro en tiempo real (`ucServicio`) del proyecto original. Incluye: panel de configuración para iniciar un servicio (combo de lugar, fecha, proyección, invitados, botón iniciar/finalizar), panel de estado del servicio activo (cronómetro, estado activo/inactivo, barra de progreso de cobertura, indicadores registrados/faltan), campo de entrada para credencial RFID con botón de registro, y tabla/listado de comensales registrados en el servicio actual. Cuando no hay servicio activo se muestra solo el panel de configuración; cuando hay un servicio activo se muestra todo. Datos estáticos de ejemplo mostrando ambos estados.

**Archivos:**
- `Views/Servicio/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos de la vista de servicios)


---

### Commit 2 — Vista de Registro Manual

**Descripción:** Crear la vista de registro manual (`Views/Registro/Index.cshtml`) que replica `ucRegistroManual`. Incluye: filtros superiores (combo de empresa, campo de búsqueda por nombre), tabla de empleados que aún no almorzaron con checkboxes para selección múltiple, botón "Agregar Seleccionados", y un resumen lateral con el conteo de registrados vs pendientes. Datos estáticos de ejemplo.

**Archivos:**
- `Views/Registro/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)


---

### Commit 3 — Vista de Reportes

**Descripción:** Crear la vista de reportes (`Views/Reporte/Index.cshtml`) que replica `ucReportes`. Incluye: panel de filtros (DatePicker desde/hasta, combo de lugar, combo de tipo de reporte), tabla de resultados con datos de ejemplo según el tipo seleccionado, y botón de exportar a PDF. Los 4 tipos de reporte son: lista de servicios, asistencias por empresa, cobertura vs proyección, distribución por día de semana. Datos estáticos mostrando el primer tipo.

**Archivos:**
- `Views/Reporte/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)


---

### Commit 4 — Vista de Administración: Empresas

**Descripción:** Crear la vista CRUD de empresas (`Views/Empresa/Index.cshtml`) que replica `ucEmpresas`. Incluye: layout dividido con tabla de empresas a la izquierda (con barra de búsqueda y conteo) y formulario de alta/edición a la derecha (nombre, estado activo/inactivo, botones guardar/cancelar/eliminar). Panel inferior con estadísticas de la empresa seleccionada (total empleados, inactivos, asistencias del mes, promedio diario). Datos estáticos.


**Archivos:**
- `Views/Empresa/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)


---

### Commit 5 — Vista de Administración: Empleados

**Descripción:** Crear la vista CRUD de empleados (`Views/Empleado/Index.cshtml`) que replica `ucEmpleados`. Incluye: tabla de empleados con filtros (búsqueda por nombre/credencial, filtro por empresa), formulario lateral (credencial RFID con botón verificar, nombre, apellido, combo empresa, estado), botones de acción. Datos estáticos de ejemplo.

**Archivos:**
- `Views/Empleado/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)


---

### Commit 6 — Vista de Estadísticas

**Descripción:** Crear la vista de estadísticas (`Views/Estadistica/Index.cshtml`) que replica `ucEstadisticas`. Incluye: grid de cards con KPIs organizados en secciones (Empleados: total/activos/inactivos, Empresas: activas/con empleados/promedio, Servicios: mes/año/promedio diario, Asistencias: total/empleados/invitados/promedio/cobertura/duración), y sección de Top 5 Empresas con barras de progreso y porcentajes. Datos estáticos.

**Archivos:**
- `Views/Estadistica/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)


---

### Commit 7 — Vista de Inicio del Administrador

**Descripción:** Crear la vista de inicio exclusiva para el rol Admin (`Views/Admin/Index.cshtml`) que será la página principal al iniciar sesión como administrador. Funciona como hub de acceso rápido a la gestión del sistema. Incluye: encabezado de bienvenida, grid de cards (Empresas, Empleados, Estadísticas) con ícono, título, descripción breve, indicador numérico (ej: "12 empresas") y botón "Ir a...". Cada card enlaza a su respectivo controlador. Datos estáticos.

**Archivos:**
- `Views/Admin/Index.cshtml` (crear)
- `Controllers/AdminController.cs` (crear)
- `wwwroot/css/site.css` (agregar estilos)


---

### Commit 8 — Navegación activa en sidebar y limpieza

**Descripción:** Completar la limpieza de navegación base ya iniciada en commits previos: mantener la lógica de resaltado activo en sidebar con `ViewContext.RouteData` y dejar el menú alineado al dominio actual (`Estadísticas` en lugar de `Configuración`). Eliminar `Privacy.cshtml` y su acción en `HomeController` (contenido scaffold no usado). Limpiar `_Layout.cshtml.css` para remover estilos scaffold sin uso.

**Archivos:**
- `Controllers/HomeController.cs` (eliminar acción `Privacy`)
- `Views/Home/Privacy.cshtml` (eliminar)
- `Views/Shared/_Layout.cshtml.css` (limpiar)


---

## UNIDAD 2 — CRUD con MVC, ADO.NET y SQL

> Se implementa la funcionalidad CRUD completa usando ADO.NET con stored procedures, tal como funciona el proyecto original WinForms.

### 📐 Convenciones de código (basadas en los proyectos de ejemplo del curso)

Todos los controladores CRUD de esta unidad (y las siguientes) deben respetar las siguientes convenciones, extraídas de los ejemplos `maxi-movie-mvc` y `galeria-arte-mvc`:

| Convención | Descripción |
|---|---|
| **`[HttpGet]` implícito** | No se anota `[HttpGet]` explícitamente en acciones de lectura — es el valor por defecto en ASP.NET MVC |
| **Comentarios `// GET:` / `// POST:`** | Cada acción lleva un comentario de una línea al estilo scaffold: `// GET: Empresa`, `// POST: Empresa/Create` |
| **`DeleteConfirmed` + `[ActionName]`** | El método POST de eliminación se llama `DeleteConfirmed` y se decora con `[HttpPost, ActionName("Delete")]` para mantener la ruta `/Delete` y evitar conflictos con el futuro GET de confirmación |
| **`XExiste(int id)`** | Todo controlador tiene un método privado `EntidadExiste(int id)` que verifica la existencia del registro. En la fase ADO.NET llama al servicio; en la fase EF usa `_context.Entidades.Any(...)` |
| **Regiones** | Dividir el controlador en 3 regiones: `Dependencias`, `Acciones Públicas`, `Métodos Privados de Soporte` |
| **Sin `[Bind]` en ViewModel** | Al usar ViewModels dedicados no es necesario `[Bind]`, ya que el ViewModel actúa como filtro de overposting |
| **`try/catch` en escritura** | Envolver siempre los bloques de escritura (Create, Edit, Delete) en `try/catch` con notificación toast via `TempData` |


---

### Commit 9 — Capa de acceso a datos con ADO.NET

**Descripción:** Crear la capa de acceso a datos replicando el patrón del proyecto original. Clase `AccesoDatos` como wrapper de `SqlConnection`/`SqlCommand`/`SqlDataReader` que lee la cadena de conexión de `appsettings.json` (inyectada via `IConfiguration`). Clase `NegocioException` con traductor de errores SQL a mensajes en español. Mappers estáticos para cada entidad (`EmpleadoMapper`, `EmpresaMapper`, `LugarMapper`, `ServicioMapper`, `RegistroMapper`) que convierten `SqlDataReader` a objetos del dominio. Se registra `AccesoDatos` en el contenedor de DI como Scoped.

**Archivos:**
- `Data/AccesoDatos.cs` (crear)
- `Data/NegocioException.cs` (crear)
- `Data/Mappers/EmpleadoMapper.cs` (crear)
- `Data/Mappers/EmpresaMapper.cs` (crear)
- `Data/Mappers/LugarMapper.cs` (crear)
- `Data/Mappers/ServicioMapper.cs` (crear)
- `Data/Mappers/RegistroMapper.cs` (crear)
- `Program.cs` (registrar AccesoDatos en DI)


---

### Commit 10 — Stored Procedures en SQL Server

**Descripción:** Ejecutar en la base de datos todos los stored procedures del proyecto original. Incluye SPs para: Empresas (7 SPs: listar, agregar, modificar, desactivar, buscar, listar con empleados, filtrar), Empleados (10 SPs: listar, buscar por credencial/id, filtrar, agregar, modificar, desactivar, verificar credencial, sin almorzar), Lugares (2 SPs: listar, buscar por nombre), Servicios (7 SPs: listar, obtener activo/último, iniciar, finalizar, listar por fecha/lugar/rango, finalizar pendientes), Registros (5 SPs: registrar, listar por servicio, verificar, contar, por empresa y fecha). Crear también las vistas (`vw_EmpleadosSinAlmorzar`, `vw_EmpresasConEmpleados`) y el trigger (`TR_ValidarRegistroUnico`). Incluir el script SQL en la carpeta del proyecto.

**Archivos:**
- `SQL/Procedimientos_Vistas_Triggers.sql` (crear — copiar/adaptar del proyecto original)



---

### Commit 11 — Servicios de negocio (Negocio Layer)

**Descripción:** Crear las clases de negocio como servicios inyectables que encapsulan la lógica de acceso a datos via stored procedures. `EmpresaNegocio`: Listar, ListarConEmpleados, BuscarPorId, Agregar, Modificar, Eliminar, Filtrar. `EmpleadoNegocio`: Listar, BuscarPorCredencial, BuscarPorId, Agregar, Modificar, Eliminar, ExisteCredencial, FiltrarEmpleados, EmpleadosSinAlmorzar, FiltrarSinAlmorzar. `ServicioNegocio`: ListarTodos, ObtenerUltimo, CrearServicio, FinalizarServicio, ListarPorFecha, FinalizarPendientes. `RegistroNegocio`: Registrar, ListarPorServicio, YaRegistrado, Contar, PorEmpresaYFecha. `LugarNegocio`: Listar. Registrar todos como Scoped en DI con interfaces.

**Archivos:**
- `Services/IEmpresaNegocio.cs` + `Services/EmpresaNegocio.cs` (crear)
- `Services/IEmpleadoNegocio.cs` + `Services/EmpleadoNegocio.cs` (crear)
- `Services/IServicioNegocio.cs` + `Services/ServicioNegocio.cs` (crear)
- `Services/IRegistroNegocio.cs` + `Services/RegistroNegocio.cs` (crear)
- `Services/ILugarNegocio.cs` + `Services/LugarNegocio.cs` (crear)
- `Program.cs` (registrar servicios en DI)


---

### Commit 12 — CRUD de Empresas funcional

**Descripción:** Implementar el controlador `EmpresaController` con todas las acciones CRUD siguiendo las convenciones del patrón de ejemplo: `Index` (GET implícito, lista con búsqueda y panel lateral), `Create` (POST), `Edit` (POST), `DeleteConfirmed` (`[HttpPost, ActionName("Delete")]`, soft-delete), `Detalle` (GET implícito, endpoint AJAX para panel lateral), `EmpresaExiste()` (método privado de verificación). Conectar la vista `Views/Empresa/Index.cshtml` con datos reales usando Razor (`@foreach`, `@model`, `asp-for`, `asp-action`). Agregar validaciones server-side. Crear ViewModels necesarios.

> ⚠️ **Nota de diseño:** la vista unifica Create y Edit en un panel lateral dentro de `Index` (sin vistas separadas), por eso no existen acciones `Edit(int id)` GET ni `Create()` GET — el formulario ya está en la vista. Esto es una variación válida respecto al CRUD clásico de los ejemplos, que sí tienen vistas separadas.

**Archivos:**
- `Controllers/EmpresaController.cs` (implementar CRUD completo)
- `Views/Empresa/Index.cshtml` (conectar con datos reales)
- `ViewModels/EmpresaViewModel.cs` (crear)
- `wwwroot/js/site.js` (agregar JS para búsqueda y panel lateral AJAX)


---

### Commit 13 — CRUD de Empleados funcional

**Descripción:** Implementar `EmpleadoController` con CRUD completo: `Index` (lista con filtros por empresa y búsqueda), `Create` (GET/POST con verificación de credencial), `Edit` (GET/POST), `Delete` (POST, soft-delete), `VerificarCredencial` (AJAX). Conectar la vista con datos reales. Validaciones: credencial única, regex para nombres (letras/espacios/acentos/guiones), campos requeridos. Cargar combo de empresas activas.

**Archivos:**
- `Controllers/EmpleadoController.cs` (implementar CRUD completo)
- `Views/Empleado/Index.cshtml` (conectar con datos reales)
- `ViewModels/EmpleadoViewModel.cs` (crear)
- `wwwroot/js/site.js` (agregar JS para filtros y verificación AJAX)


---

### Commit 14 — Servicio activo y registro por credencial

**Descripción:** Implementar `ServicioController`: `Index` (vista principal con info del servicio activo), `Iniciar` (POST — crear servicio con lugar y proyección), `Finalizar` (POST — guardar totales y duración), `Registrar` (POST — registro por credencial). Conectar la vista con datos reales. Implementar cronómetro con JavaScript (`setInterval`), barra de progreso dinámica, y notificación toast al registrar. El listado de comensales se actualiza via AJAX. Validaciones: no iniciar si ya hay servicio activo, proyección 0-1000, invitados 0-500.

**Archivos:**
- `Controllers/ServicioController.cs` (implementar)
- `Views/Servicio/Index.cshtml` (conectar con datos reales)
- `ViewModels/ServicioActivoViewModel.cs` (crear)
- `wwwroot/js/site.js` (cronómetro JS, AJAX de registro, toast)


---

### Commit 15 — Registro manual de empleados

**Descripción:** Implementar en `RegistroController`: `Index` (vista de registro manual), `ObtenerSinAlmorzar` (AJAX — lista filtrada), `RegistrarMultiples` (POST — registro masivo). Conectar la vista con datos reales: filtro por empresa (combo), búsqueda por nombre (input), tabla de empleados sin almorzar con checkboxes, botón de registro masivo. Actualización dinámica del conteo.

**Archivos:**
- `Controllers/RegistroController.cs` (implementar)
- `Views/Registro/Index.cshtml` (conectar con datos reales)
- `wwwroot/js/site.js` (agregar JS para filtros y registro masivo AJAX)


---

### Commit 16 — Reportes y exportación PDF

**Descripción:** Implementar `ReporteController`: `Index` (vista con filtros), `Generar` (POST — genera reporte según tipo), `ExportarPdf` (GET — descarga PDF). Conectar la vista con datos reales. 4 tipos de reporte: lista de servicios, asistencias por empresa, cobertura vs proyección, distribución por día de semana. Instalar paquete `QuestPDF` para generación de PDFs con estilo (header, filtros aplicados, tabla formateada). Agregar modelos de reporte (`AsistenciaPorEmpresa`, `CoberturaVsProyeccion`, `DistribucionDiaSemana`).

**Archivos:**
- `Controllers/ReporteController.cs` (implementar)
- `Views/Reporte/Index.cshtml` (conectar con datos reales)
- `Services/IReporteNegocio.cs` + `Services/ReporteNegocio.cs` (crear)
- `Services/PdfService.cs` (crear — generación PDF con QuestPDF)
- `Models/Reportes.cs` (crear — modelos de reporte)
- `SCA-MVC.csproj` (agregar QuestPDF)
- `wwwroot/js/site.js` (agregar JS para cambio de tipo de reporte)


---

### Commit 17 — Estadísticas y dashboard con datos reales

**Descripción:** Implementar `EstadisticaController` con acción `Index` que obtiene todos los KPIs. Conectar la vista con datos reales. Implementar `EstadisticasNegocio` con 5 métodos que llaman a los SPs de estadísticas. Agregar modelos de estadísticas (`Estadisticas.Empleados`, `.Empresas`, `.Servicios`, `.Asistencias`, `.TopEmpresa`). Crear ViewModel que agrupe todas las estadísticas. Conectar también el `Index` del `HomeController` con datos reales (últimos servicios, detalle, comparativa semanal).

**Archivos:**
- `Controllers/EstadisticaController.cs` (implementar)
- `Controllers/HomeController.cs` (conectar Index con datos reales)
- `Views/Estadistica/Index.cshtml` (conectar con datos reales)
- `Views/Home/Index.cshtml` (reemplazar datos estáticos por Razor)
- `Services/IEstadisticasNegocio.cs` + `Services/EstadisticasNegocio.cs` (crear)
- `Models/Estadisticas.cs` (crear)
- `ViewModels/DashboardViewModel.cs` (crear)
- `ViewModels/EstadisticasViewModel.cs` (crear)
- `wwwroot/js/site.js` (agregar JS para selección de servicio en Index)


---

### Commit 18 — Seed de datos iniciales (Data Seeding)

**Descripción:** Implementar el seeding de datos iniciales utilizando `modelBuilder.HasData()` en el método `OnModelCreating` de `ApplicationDbContext`. Esto permite que la base de datos se pueble automáticamente al aplicar las migraciones. Se incluyen: 2 lugares principales, las 6 empresas clave del complejo, una lista de 16 empleados iniciales con credenciales RFxxxx, y una muestra de servicios y registros para febrero de 2026. Se ajusta también el check constraint de fecha para permitir datos de prueba.

**Archivos:**
- `Data/ApplicationDbContext.cs` (implementar método `SeedData` y llamar desde `OnModelCreating`)


---

### Commit 19 — Validaciones completas y mensajes de usuario

**Descripción:** Implementar validaciones client-side con jQuery Validation + Unobtrusive en todos los formularios. Crear clase `MensajesConstantes` con todas las constantes de mensajes en español (validación, confirmación, éxito, error). Crear helper `MensajesUI` para manejo centralizado de TempData (éxito/error/advertencia). Agregar `_ValidationScriptsPartial` en todas las vistas con formularios. Implementar notificaciones toast globales en el layout que leen TempData.

**Archivos:**
- `Helpers/MensajesConstantes.cs` (crear)
- `Helpers/MensajesUI.cs` (crear — extensiones de TempData)
- `Views/Shared/_Notificaciones.cshtml` (crear — partial de toasts)
- `Views/Shared/_Layout.cshtml` (agregar render de notificaciones)
- `Views/Empresa/Index.cshtml` (agregar validaciones client-side)
- `Views/Empleado/Index.cshtml` (agregar validaciones client-side)
- `wwwroot/js/site.js` (lógica de toast notifications)


---

## UNIDAD 3 — Entity Framework

> Se reemplaza la capa ADO.NET por Entity Framework Core, aprovechando el DbContext y modelos ya existentes.

---

### Commit 20 — Refactorización: de ADO.NET a Entity Framework

**Descripción:** Reemplazar todas las llamadas a stored procedures en los servicios de negocio por consultas LINQ a través de `ApplicationDbContext`. Eliminar `AccesoDatos.cs` y los Mappers (EF los hace automáticamente — mapea los resultados directamente a los modelos). Actualizar cada servicio: `EmpresaNegocio` usa `_context.Empresas.Where/Include/...`, `EmpleadoNegocio` usa `_context.Empleados.Include(e => e.Empresa)...`, etc. Los servicios ahora reciben `ApplicationDbContext` via DI en lugar de `AccesoDatos`. Mantener las mismas interfaces para que los controladores **no cambien**.

> 🎯 **Punto de convergencia con los ejemplos del curso:** al finalizar este commit, el patrón interno de los servicios será idéntico al de `PeliculaController`/`GeneroController` de `maxi-movie-mvc` — LINQ con `.Include()`, `.FirstOrDefaultAsync()`, `.ToListAsync()`, y `catch (DbUpdateConcurrencyException)` en las ediciones. Los controladores permanecen intactos porque las interfaces actúan como contrato estable.

> 🗑️ **Archivos a eliminar completamente:** `Data/AccesoDatos.cs`, `Data/NegocioException.cs`, `Data/Mappers/` (toda la carpeta). Estos son el equivalente al `DbContext` wrapper de ADO, que EF reemplaza de forma nativa.

**Archivos:**
- `Services/EmpresaNegocio.cs` (reescribir con EF — LINQ en lugar de stored procedures)
- `Services/EmpleadoNegocio.cs` (reescribir con EF)
- `Services/ServicioNegocio.cs` (reescribir con EF)
- `Services/RegistroNegocio.cs` (reescribir con EF)
- `Services/LugarNegocio.cs` (reescribir con EF)
- `Services/ReporteNegocio.cs` (reescribir con EF)
- `Services/EstadisticasNegocio.cs` (reescribir con EF)
- `Data/AccesoDatos.cs` (**eliminar**)
- `Data/NegocioException.cs` (**eliminar**)
- `Data/Mappers/` (**eliminar carpeta completa**)
- `Program.cs` (reemplazar registro de `AccesoDatos` por `ApplicationDbContext`)


---

### Commit 21 — Revisión del modelado de clases y Fluent API

**Descripción:** Revisar y documentar el modelado existente en `OnModelCreating`. Verificar que todas las relaciones, índices, restricciones y defaults coincidan con el schema original. Agregar comentarios explicativos en el `ApplicationDbContext` sobre cada configuración de Fluent API: `HasOne/WithMany`, `HasForeignKey`, `OnDelete`, `HasIndex`, `HasCheckConstraint`, `HasDefaultValue`. Esto es la explicación didáctica del modelado para la unidad.

**Archivos:**
- `Data/ApplicationDbContext.cs` (agregar comentarios explicativos de Fluent API)


---

## UNIDAD 4 — Desarrollo con EF: Fluent API, Seeding y Vistas Parciales

---

### Commit 23 — Seeding con Entity Framework

**Descripción:** Implementar seeding de datos iniciales usando `HasData()` en `OnModelCreating`. Incluir: 2 Lugares (Comedor, Quincho), 12 Empresas, 60 Empleados con credenciales RF001-RF060. Generar migración que incluya los datos semilla. Los servicios y registros NO se seedean (se crean en runtime). Esto reemplaza el script SQL de datos iniciales.

**Archivos:**
- `Data/ApplicationDbContext.cs` (agregar HasData en OnModelCreating)
- `Migrations/[timestamp]_SeedDatos.cs` (nueva migración con seed)


---

### Commit 24 — Configuraciones Fluent API separadas

**Descripción:** Refactorizar el `OnModelCreating` moviendo cada configuración de entidad a su propia clase usando `IEntityTypeConfiguration<T>`. Crear: `EmpresaConfiguration`, `EmpleadoConfiguration`, `LugarConfiguration`, `ServicioConfiguration`, `RegistroConfiguration`. En `OnModelCreating` usar `modelBuilder.ApplyConfigurationsFromAssembly()`. Esto mejora la organización y es una práctica avanzada de Fluent API.

**Archivos:**
- `Data/Configurations/EmpresaConfiguration.cs` (crear)
- `Data/Configurations/EmpleadoConfiguration.cs` (crear)
- `Data/Configurations/LugarConfiguration.cs` (crear)
- `Data/Configurations/ServicioConfiguration.cs` (crear)
- `Data/Configurations/RegistroConfiguration.cs` (crear)
- `Data/ApplicationDbContext.cs` (simplificar OnModelCreating)


---

### Commit 25 — Vistas parciales

**Descripción:** Extraer componentes reutilizables a vistas parciales: `_ServicioCard.cshtml` (card de servicio usada en Index y Reportes), `_EmpleadoRow.cshtml` (fila de empleado usada en Empleados y Registro Manual), `_KpiCard.cshtml` (card de KPI usada en Estadísticas e Index), `_FiltroFechas.cshtml` (filtro de rango de fechas usado en Reportes), `_Paginacion.cshtml` (controles de paginación). Actualizar las vistas principales para usar `@await Html.PartialAsync()` o `<partial>` tag helper.

**Archivos:**
- `Views/Shared/_ServicioCard.cshtml` (crear)
- `Views/Shared/_EmpleadoRow.cshtml` (crear)
- `Views/Shared/_KpiCard.cshtml` (crear)
- `Views/Shared/_FiltroFechas.cshtml` (crear)
- `Views/Shared/_Paginacion.cshtml` (crear)
- `Views/Home/Index.cshtml` (usar partials)
- `Views/Estadistica/Index.cshtml` (usar partials)
- `Views/Reporte/Index.cshtml` (usar partials)
- `Views/Empleado/Index.cshtml` (usar partials)


---

## UNIDAD 5 — ASP.NET Identity: Autenticación y Autorización

---

### Commit 26 — Instalación y configuración de Identity

**Descripción:** Instalar `Microsoft.AspNetCore.Identity.EntityFrameworkCore`. Crear modelo `ApplicationUser` que extiende `IdentityUser` con propiedades adicionales (`Nombre`, `Apellido`). Cambiar `ApplicationDbContext` para heredar de `IdentityDbContext<ApplicationUser>`. Configurar Identity en `Program.cs` con opciones de contraseña, lockout y cookie. Generar migración para las tablas de Identity.

**Archivos:**
- `Models/ApplicationUser.cs` (crear)
- `Data/ApplicationDbContext.cs` (cambiar herencia a IdentityDbContext)
- `Program.cs` (configurar Identity services + middleware)
- `SCA-MVC.csproj` (agregar paquete Identity)
- `Migrations/[timestamp]_AddIdentity.cs` (nueva migración)


---

### Commit 27 — Vistas de Login, Logout y Registro

**Descripción:** Crear `AccountController` con acciones: `Login` (GET/POST), `Logout` (POST), `Register` (GET/POST), `AccessDenied` (GET). Diseñar las vistas de login y registro manteniendo el estilo glassmorphism (centradas, con logo, campos dorados). Actualizar el layout: reemplazar el pill "Administrador" hardcodeado por el nombre real del usuario autenticado con `@User.Identity.Name`. Agregar botón de logout funcional.

**Archivos:**
- `Controllers/AccountController.cs` (crear)
- `Views/Account/Login.cshtml` (crear)
- `Views/Account/Register.cshtml` (crear)
- `Views/Account/AccessDenied.cshtml` (crear)
- `Views/Shared/_Layout.cshtml` (actualizar topbar con usuario real)
- `wwwroot/css/site.css` (estilos de login/register)


---

### Commit 28 — Roles y autorización por rol

**Descripción:** Crear roles del sistema: `Admin` y `Usuario`. Implementar seeding de roles y usuario admin inicial en `Program.cs` (al arrancar la app, crea roles si no existen y crea usuario admin por defecto). Agregar `[Authorize]` a todos los controladores. Agregar `[Authorize(Roles = "Admin")]` a los controladores de: Empresa, Empleado, Configuración. El rol `Usuario` solo puede acceder a: Home, Servicio, Registro, Reporte, Estadística. Actualizar sidebar para ocultar ítems según rol.

**Archivos:**
- `Program.cs` (seeding de roles y admin user al inicio)
- `Controllers/*.cs` (agregar atributos [Authorize] con roles)
- `Views/Shared/_Layout.cshtml` (mostrar/ocultar ítems por rol)


---

### ✅ Commit 29 — Gestión de usuarios

**Descripción:** Crear `UsuarioController` (solo Admin) para gestión de usuarios del sistema: listar usuarios con sus roles, crear nuevo usuario asignando rol, editar usuario (cambiar rol, resetear contraseña), desactivar/reactivar usuario via `LockoutEnd`. Vista `Views/Usuario/Index.cshtml` con el mismo diseño de dos columnas que Empleados y Empresas. Badges de rol en dorado (Admin) y azul (Usuario). Enlace USUARIOS con icono `bi-shield-lock` en el sidebar del Admin.

**Archivos:**
- ✅ `Controllers/UsuarioController.cs` (creado) — Index, Create, Edit, DeleteConfirmed (toggle), RebuildAndReturn
- ✅ `Views/Usuario/Index.cshtml` (creado) — 2 columnas: tabla + formulario, búsqueda en tiempo real
- ✅ `ViewModels/UsuarioViewModel.cs` (creado) — UsuarioListItem, UsuarioFormViewModel, UsuarioViewModel
- ✅ `Views/Shared/_Layout.cshtml` — USUARIOS con bi-shield-lock en nav Admin
- ✅ `wwwroot/css/site.css` — .usr-badge, .usr-admin, .usr-usuario, .btn-emp-reactivar, .emp-field-row


---

## UNIDAD 6 — Avanzado: Archivos, Email e IA

---

### ✅ Commit 31 — Envío de email (reportes por correo)

**Descripción:** Implementar servicio de email usando MailKit/MimeKit. Configurar SMTP en `appsettings.json`. Crear `IEmailService` con `EnviarReporteAsync()`. Refactorizar `ExportarPDF` extrayendo la generación de PDF a `GenerarPdfBytesAsync()` para reutilización. Botón "Enviar por Email" en la vista de reportes con modal glassmorphism para ingresar el destinatario. El PDF se genera y adjunta al email en un solo flujo.

**Archivos:**
- ✅ `Services/IEmailService.cs` (creado) — interfaz con `EnviarReporteAsync`
- ✅ `Services/EmailService.cs` (creado) — MailKit SMTP + MimeKit adjunto PDF
- ✅ `SCA-MVC.csproj` — MailKit 4.15.1 agregado
- ✅ `appsettings.json` — sección EmailSettings (SmtpHost/Port/User/Pass/From)
- ✅ `Program.cs` — `AddScoped<IEmailService, EmailService>()`
- ✅ `Controllers/ReporteController.cs` — GenerarPdfBytesAsync, EnviarPorEmail, IEmailService inyectado
- ✅ `Views/Reporte/Index.cshtml` — botón + modal de email + section Scripts
- ✅ `wwwroot/css/site.css` — .btn-email-rpt, .modal-field


---

### Commit 32 — Responsividad para Tablet Samsung Galaxy A9

**Descripción:** Corregir problemas de responsividad detectados al usar la aplicación en una tablet táctil. Ajustar el meta viewport, bloquear el expand por hover del sidebar y reorganizar los layouts de grillas. Solucionar error `OUTPUT clause` del SQL Server al guardar registros con triggers.

**Archivos:**
- `Views/Shared/_Layout.cshtml` (viewport meta)
- `wwwroot/css/site.css` (media queries)
- `Data/Configurations/RegistroConfiguration.cs` (HasTrigger)


---

### Commit 33 — Manejo Global de Errores

**Descripción:** Implementar un middleware de excepciones global (`ExceptionMiddleware`) para atrapar errores no controlados y redireccionar a páginas de error estilo glassmorphism. Crear vistas exclusivas `Error.cshtml` y `NotFound.cshtml` para manejar códigos 500 y 404 independientes del Layout principal.

**Archivos:**
- `Middleware/ExceptionMiddleware.cs` (crear)
- `Controllers/HomeController.cs` (NotFound)
- `Views/Shared/Error.cshtml` (rediseño)
- `Views/Home/NotFound.cshtml` (creado)


---

### Commit 34 — Corrección de Bugs y Limpieza de Código

**Descripción:** Estabilizar la aplicación. Reemplazar media queries por un viewport fijo de 1280px para tablet y forzar zoom del navegador nativo. Alinear cards del Admin en 4 columnas. Unificar los layouts `Index` de ABM. Eliminar código html/css redundante. 

**Archivos:**
- `Services/RegistroNegocio.cs` (ExecuteSqlRawAsync)
- `wwwroot/css/site.css` (limpieza media queries)
- `Views/Shared/_Layout.cshtml` (viewport 1280px)
- `Views/Admin/Index.cshtml` (arreglar grid 4 col)
- `Views/.../Index.cshtml` (alineación gaps)


---

## Resumen por Unidad

| Unidad | Commits | Tema Principal |
|--------|---------|---------------|
| **Fase 0** | 1–8 | Diseños de vistas cshtml (estáticas) |
| **Unidad 2** | 9–19 | CRUD con ADO.NET, Stored Procedures, Validaciones |
| **Unidad 3** | 20–21 | Migración a Entity Framework, Modelado, Migraciones |
| **Unidad 4** | 23–25 | Seeding, Fluent API avanzado, Vistas parciales |
| **Unidad 5** | 26–29 | Identity, Login, Roles, Gestión de usuarios |
| **Unidad 6** | 31–34 | Envío Email, Responsive Tablet, Errores y Limpieza |

---

## Conceptos Cubiertos por Commit

| Concepto del Plan de Estudio | Commits |
|------------------------------|---------|
| Introducción a MVC | Todo el proyecto |
| Introducción a Razor | 1–8, 12–17 |
| HTML, CSS y JS en MVC | 1–8, 12–17 |
| Primer proyecto MVC | 1–8 |
| CRUD con MVC, ADO y SQL | 9–18 |
| Controladores y Vistas Razor | 8, 12–17 |
| Validaciones | 13, 14, 19 |
| Entity Framework | 20–21 |
| Modelado de clases | 21 |
| Configuración de contexto | 20, 21 |
| Migraciones | 21, 23 |
| Desarrollo con EF | 20–25 |
| Fluent API | 21, 24 |
| Seeding | 23 |
| Vistas parciales | 25 |
| Identity | 26 |
| Configuración de usuarios | 27 |
| Roles y permisos | 28 |
| Gestión de usuarios | 29 |
| Desarrollo con EF e Identity | 26–29 |
| Envíos de email | 31 |
| Layout Responsive Móvil/Tablet | 32 |
| Manejo Global de Excepciones | 33 |

