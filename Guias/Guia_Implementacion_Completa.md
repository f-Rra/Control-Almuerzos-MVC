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

**Mensaje:**
```
feat: diseño de vista de servicios

- Panel de configuración (lugar, fecha, proyección, invitados, iniciar/finalizar)
- Panel de estado del servicio activo (cronómetro, cobertura, registrados/faltan)
- Campo de registro por credencial RFID
- Listado de comensales registrados en el servicio
- Vista adaptativa según estado del servicio (activo/inactivo)
```

---

### Commit 2 — Vista de Registro Manual

**Descripción:** Crear la vista de registro manual (`Views/Registro/Index.cshtml`) que replica `ucRegistroManual`. Incluye: filtros superiores (combo de empresa, campo de búsqueda por nombre), tabla de empleados que aún no almorzaron con checkboxes para selección múltiple, botón "Agregar Seleccionados", y un resumen lateral con el conteo de registrados vs pendientes. Datos estáticos de ejemplo.

**Archivos:**
- `Views/Registro/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)

**Mensaje:**
```
feat: diseño de vista de registro manual

- Filtros por empresa y búsqueda por nombre
- Tabla de empleados sin almorzar con selección múltiple
- Botón de registro masivo
- Resumen de registrados vs pendientes
```

---

### Commit 3 — Vista de Reportes

**Descripción:** Crear la vista de reportes (`Views/Reporte/Index.cshtml`) que replica `ucReportes`. Incluye: panel de filtros (DatePicker desde/hasta, combo de lugar, combo de tipo de reporte), tabla de resultados con datos de ejemplo según el tipo seleccionado, y botón de exportar a PDF. Los 4 tipos de reporte son: lista de servicios, asistencias por empresa, cobertura vs proyección, distribución por día de semana. Datos estáticos mostrando el primer tipo.

**Archivos:**
- `Views/Reporte/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)

**Mensaje:**
```
feat: diseño de vista de reportes

- Panel de filtros (fechas, lugar, tipo de reporte)
- Tabla de resultados con datos de ejemplo
- Botón de exportación a PDF
- Soporte visual para 4 tipos de reporte
```

---

### Commit 4 — Vista de Administración: Empresas

**Descripción:** Crear la vista CRUD de empresas (`Views/Empresa/Index.cshtml`) que replica `ucEmpresas`. Incluye: layout dividido con tabla de empresas a la izquierda (con barra de búsqueda y conteo) y formulario de alta/edición a la derecha (nombre, estado activo/inactivo, botones guardar/cancelar/eliminar). Panel inferior con estadísticas de la empresa seleccionada (total empleados, inactivos, asistencias del mes, promedio diario). Datos estáticos.


**Archivos:**
- `Views/Empresa/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)

**Mensaje:**
```
feat: diseño de vista CRUD de empresas

- Tabla de empresas con búsqueda y conteo
- Formulario de alta/edición con validación visual
- Panel de estadísticas por empresa
- Botones de acción (guardar, cancelar, eliminar)
```

---

### Commit 5 — Vista de Administración: Empleados

**Descripción:** Crear la vista CRUD de empleados (`Views/Empleado/Index.cshtml`) que replica `ucEmpleados`. Incluye: tabla de empleados con filtros (búsqueda por nombre/credencial, filtro por empresa), formulario lateral (credencial RFID con botón verificar, nombre, apellido, combo empresa, estado), botones de acción. Datos estáticos de ejemplo.

**Archivos:**
- `Views/Empleado/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)

**Mensaje:**
```
feat: diseño de vista CRUD de empleados

- Tabla de empleados con filtros por nombre y empresa
- Formulario con verificación de credencial RFID
- Combo de empresas y selector de estado
- Botones de acción (nuevo, guardar, eliminar, cancelar)
```

---

### Commit 6 — Vista de Estadísticas

**Descripción:** Crear la vista de estadísticas (`Views/Estadistica/Index.cshtml`) que replica `ucEstadisticas`. Incluye: grid de cards con KPIs organizados en secciones (Empleados: total/activos/inactivos, Empresas: activas/con empleados/promedio, Servicios: mes/año/promedio diario, Asistencias: total/empleados/invitados/promedio/cobertura/duración), y sección de Top 5 Empresas con barras de progreso y porcentajes. Datos estáticos.

**Archivos:**
- `Views/Estadistica/Index.cshtml` (crear)
- `wwwroot/css/site.css` (agregar estilos)

**Mensaje:**
```
feat: diseño de vista de estadísticas y KPIs

- Cards de KPIs por sección (empleados, empresas, servicios, asistencias)
- Tendencias y métricas mensuales
- Top 5 empresas con barras de progreso
- Indicadores de cobertura y duración promedio
```

---

### Commit 7 — Vista de Inicio del Administrador

**Descripción:** Crear la vista de inicio exclusiva para el rol Admin (`Views/Admin/Index.cshtml`) que será la página principal al iniciar sesión como administrador. Funciona como hub de acceso rápido a la gestión del sistema. Incluye: encabezado de bienvenida, grid de cards (Empresas, Empleados, Estadísticas) con ícono, título, descripción breve, indicador numérico (ej: "12 empresas") y botón "Ir a...". Cada card enlaza a su respectivo controlador. Datos estáticos.

**Archivos:**
- `Views/Admin/Index.cshtml` (crear)
- `Controllers/AdminController.cs` (crear)
- `wwwroot/css/site.css` (agregar estilos)

**Mensaje:**
```
feat: diseño de vista de inicio del administrador

- Hub de acceso rápido a Empresas, Empleados y Estadísticas
- Grid de cards con íconos, conteos e indicadores
- Controlador AdminController con acción Index
- Estilos adm-* integrados en site.css
```

---

### Commit 8 — Navegación activa en sidebar y limpieza

**Descripción:** Completar la limpieza de navegación base ya iniciada en commits previos: mantener la lógica de resaltado activo en sidebar con `ViewContext.RouteData` y dejar el menú alineado al dominio actual (`Estadísticas` en lugar de `Configuración`). Eliminar `Privacy.cshtml` y su acción en `HomeController` (contenido scaffold no usado). Limpiar `_Layout.cshtml.css` para remover estilos scaffold sin uso.

**Archivos:**
- `Controllers/HomeController.cs` (eliminar acción `Privacy`)
- `Views/Home/Privacy.cshtml` (eliminar)
- `Views/Shared/_Layout.cshtml.css` (limpiar)

**Mensaje:**
```
feat: completar limpieza de navegación base y vistas scaffold

- Sidebar con navegación activa alineada al dominio (Estadísticas)
- Eliminada acción y vista Privacy del Home scaffold
- Limpieza de estilos scaffold no utilizados en _Layout.cshtml.css
```

---

## UNIDAD 2 — CRUD con MVC, ADO.NET y SQL

> Se implementa la funcionalidad CRUD completa usando ADO.NET con stored procedures, tal como funciona el proyecto original WinForms.

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

**Mensaje:**
```
feat: crear capa de acceso a datos con ADO.NET

- Clase AccesoDatos con SqlConnection/SqlCommand/SqlDataReader
- NegocioException con traducción de errores SQL a español
- Mappers estáticos para cada entidad del dominio
- Registro en contenedor de inyección de dependencias
```

---

### Commit 10 — Stored Procedures en SQL Server

**Descripción:** Ejecutar en la base de datos todos los stored procedures del proyecto original. Incluye SPs para: Empresas (7 SPs: listar, agregar, modificar, desactivar, buscar, listar con empleados, filtrar), Empleados (10 SPs: listar, buscar por credencial/id, filtrar, agregar, modificar, desactivar, verificar credencial, sin almorzar), Lugares (2 SPs: listar, buscar por nombre), Servicios (7 SPs: listar, obtener activo/último, iniciar, finalizar, listar por fecha/lugar/rango, finalizar pendientes), Registros (5 SPs: registrar, listar por servicio, verificar, contar, por empresa y fecha). Crear también las vistas (`vw_EmpleadosSinAlmorzar`, `vw_EmpresasConEmpleados`) y el trigger (`TR_ValidarRegistroUnico`). Incluir el script SQL en la carpeta del proyecto.

**Archivos:**
- `SQL/Procedimientos_Vistas_Triggers.sql` (crear — copiar/adaptar del proyecto original)
- `SQL/README.md` (crear — instrucciones de ejecución)

**Mensaje:**
```
feat: crear stored procedures, vistas y triggers en SQL Server

- 33 stored procedures para todas las operaciones CRUD y reportes
- 2 vistas (EmpleadosSinAlmorzar, EmpresasConEmpleados)
- Trigger de validación de registro único
- Script SQL documentado con instrucciones de ejecución
```

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

**Mensaje:**
```
feat: crear capa de servicios de negocio con ADO.NET

- Servicios para Empresa, Empleado, Servicio, Registro y Lugar
- Interfaces para inyección de dependencias
- Llamadas a stored procedures via AccesoDatos
- Registro de servicios en el contenedor de DI
```

---

### Commit 12 — CRUD de Empresas funcional

**Descripción:** Implementar el controlador `EmpresaController` con todas las acciones CRUD: `Index` (lista con búsqueda), `Create` (GET/POST), `Edit` (GET/POST), `Delete` (POST, soft-delete), `Detalle` (AJAX para panel lateral). Conectar la vista `Views/Empresa/Index.cshtml` con datos reales del servidor reemplazando los datos estáticos por Razor (`@foreach`, `@model`, `asp-for`, `asp-action`). Agregar validaciones server-side (nombre requerido, duplicados, no desactivar con empleados activos). Crear ViewModels necesarios.

**Archivos:**
- `Controllers/EmpresaController.cs` (implementar CRUD completo)
- `Views/Empresa/Index.cshtml` (conectar con datos reales)
- `ViewModels/EmpresaViewModel.cs` (crear)
- `wwwroot/js/site.js` (agregar JS para búsqueda y panel lateral AJAX)

**Mensaje:**
```
feat: implementar CRUD funcional de empresas con ADO.NET

- Controlador con acciones Index, Create, Edit, Delete
- Vista conectada a datos reales con Razor
- Búsqueda en tiempo real y panel de estadísticas
- Validaciones server-side (duplicados, dependencias)
- ViewModels para la vista
```

---

### Commit 13 — CRUD de Empleados funcional

**Descripción:** Implementar `EmpleadoController` con CRUD completo: `Index` (lista con filtros por empresa y búsqueda), `Create` (GET/POST con verificación de credencial), `Edit` (GET/POST), `Delete` (POST, soft-delete), `VerificarCredencial` (AJAX). Conectar la vista con datos reales. Validaciones: credencial única, regex para nombres (letras/espacios/acentos/guiones), campos requeridos. Cargar combo de empresas activas.

**Archivos:**
- `Controllers/EmpleadoController.cs` (implementar CRUD completo)
- `Views/Empleado/Index.cshtml` (conectar con datos reales)
- `ViewModels/EmpleadoViewModel.cs` (crear)
- `wwwroot/js/site.js` (agregar JS para filtros y verificación AJAX)

**Mensaje:**
```
feat: implementar CRUD funcional de empleados con ADO.NET

- Controlador con acciones CRUD completas
- Filtros por empresa y búsqueda por nombre/credencial
- Verificación de credencial RFID vía AJAX
- Validaciones regex y server-side
```

---

### Commit 14 — Servicio activo y registro por credencial

**Descripción:** Implementar `ServicioController`: `Index` (vista principal con info del servicio activo), `Iniciar` (POST — crear servicio con lugar y proyección), `Finalizar` (POST — guardar totales y duración), `Registrar` (POST — registro por credencial). Conectar la vista con datos reales. Implementar cronómetro con JavaScript (`setInterval`), barra de progreso dinámica, y notificación toast al registrar. El listado de comensales se actualiza via AJAX. Validaciones: no iniciar si ya hay servicio activo, proyección 0-1000, invitados 0-500.

**Archivos:**
- `Controllers/ServicioController.cs` (implementar)
- `Views/Servicio/Index.cshtml` (conectar con datos reales)
- `ViewModels/ServicioActivoViewModel.cs` (crear)
- `wwwroot/js/site.js` (cronómetro JS, AJAX de registro, toast)

**Mensaje:**
```
feat: implementar servicio activo y registro por credencial RFID

- Iniciar/finalizar servicio con validaciones
- Registro de comensales por credencial con AJAX
- Cronómetro JavaScript en tiempo real
- Barra de progreso y notificaciones toast
- Listado dinámico de comensales registrados
```

---

### Commit 15 — Registro manual de empleados

**Descripción:** Implementar en `RegistroController`: `Index` (vista de registro manual), `ObtenerSinAlmorzar` (AJAX — lista filtrada), `RegistrarMultiples` (POST — registro masivo). Conectar la vista con datos reales: filtro por empresa (combo), búsqueda por nombre (input), tabla de empleados sin almorzar con checkboxes, botón de registro masivo. Actualización dinámica del conteo.

**Archivos:**
- `Controllers/RegistroController.cs` (implementar)
- `Views/Registro/Index.cshtml` (conectar con datos reales)
- `wwwroot/js/site.js` (agregar JS para filtros y registro masivo AJAX)

**Mensaje:**
```
feat: implementar registro manual de empleados
- Registro masivo vía AJAX
- Actualización dinámica del listado y conteos
```

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

**Mensaje:**
```
feat: implementar reportes con filtros y exportación PDF

- 4 tipos de reporte (servicios, empresas, cobertura, distribución)
- Filtros por fecha, lugar y tipo de reporte
- Generación de PDF con QuestPDF (header, tabla formateada)
- Modelos de datos para reportes
```

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

**Mensaje:**
```
feat: conectar estadísticas y dashboard con datos reales

- KPIs de empleados, empresas, servicios y asistencias
- Top 5 empresas con barras de progreso
- Dashboard Index conectado a datos del servidor
- Selección dinámica de servicio en listado
```

---

### Commit 18 — Seed de datos iniciales

**Descripción:** Ejecutar datos iniciales en la base de datos: 2 lugares, 12 empresas, 60 empleados con credenciales RF001-RF060, 10 servicios finalizados y ~490 registros de asistencia. Incluir el script SQL en la carpeta del proyecto. Esto permite probar toda la funcionalidad CRUD y reportes con datos realistas.

**Archivos:**
- `SQL/Datos_Iniciales.sql` (crear — copiar/adaptar del proyecto original)

**Mensaje:**
```
feat: agregar seed de datos iniciales en SQL Server

- 2 lugares (Comedor, Quincho)
- 12 empresas del complejo industrial
- 60 empleados distribuidos con credenciales RFID
- 10 servicios finalizados con registros de asistencia
```

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

**Mensaje:**
```
feat: agregar validaciones completas y sistema de mensajes

- Validaciones client-side con jQuery Validation
- Constantes de mensajes en español
- Helper de TempData para notificaciones
- Toast notifications globales en layout
```

---

## UNIDAD 3 — Entity Framework

> Se reemplaza la capa ADO.NET por Entity Framework Core, aprovechando el DbContext y modelos ya existentes.

---

### Commit 20 — Refactorización: de ADO.NET a Entity Framework

**Descripción:** Reemplazar todas las llamadas a stored procedures en los servicios de negocio por consultas LINQ a través de `ApplicationDbContext`. Eliminar `AccesoDatos.cs` y los Mappers (EF los hace automáticamente). Actualizar cada servicio: `EmpresaNegocio` usa `_context.Empresas.Where/Include/...`, `EmpleadoNegocio` usa `_context.Empleados.Include(e => e.Empresa)...`, etc. Los servicios ahora reciben `ApplicationDbContext` via DI en lugar de `AccesoDatos`. Mantener las mismas interfaces para que los controladores no cambien.

**Archivos:**
- `Services/EmpresaNegocio.cs` (reescribir con EF)
- `Services/EmpleadoNegocio.cs` (reescribir con EF)
- `Services/ServicioNegocio.cs` (reescribir con EF)
- `Services/RegistroNegocio.cs` (reescribir con EF)
- `Services/LugarNegocio.cs` (reescribir con EF)
- `Services/ReporteNegocio.cs` (reescribir con EF)
- `Services/EstadisticasNegocio.cs` (reescribir con EF)
- `Data/AccesoDatos.cs` (eliminar)
- `Data/Mappers/` (eliminar carpeta completa)
- `Program.cs` (actualizar registros de DI)

**Mensaje:**
```
refactor: migrar de ADO.NET a Entity Framework Core

- Reemplazo de stored procedures por consultas LINQ
- Eliminación de AccesoDatos y Mappers (EF los reemplaza)
- Servicios refactorizados con ApplicationDbContext
- Mismas interfaces, mismos controladores, nueva implementación
```

---

### Commit 21 — Revisión del modelado de clases y Fluent API

**Descripción:** Revisar y documentar el modelado existente en `OnModelCreating`. Verificar que todas las relaciones, índices, restricciones y defaults coincidan con el schema original. Agregar comentarios explicativos en el `ApplicationDbContext` sobre cada configuración de Fluent API: `HasOne/WithMany`, `HasForeignKey`, `OnDelete`, `HasIndex`, `HasCheckConstraint`, `HasDefaultValue`. Esto es la explicación didáctica del modelado para la unidad.

**Archivos:**
- `Data/ApplicationDbContext.cs` (agregar comentarios explicativos de Fluent API)

**Mensaje:**
```
docs: documentar Fluent API en ApplicationDbContext

- Comentarios explicativos en cada configuración
- Detalle de relaciones, índices y restricciones
- Referencia didáctica del modelado de clases con EF Core
```

---

### Commit 22 — Nueva migración con ajustes de modelado

**Descripción:** Realizar ajustes finos al modelo si se detectaron diferencias con el esquema original (por ejemplo, tipos de datos `Date` vs `DateTime`, longitudes de campos). Generar una nueva migración que aplique estos cambios. Documentar el proceso de crear y aplicar migraciones (`Add-Migration`, `Update-Database`).

**Archivos:**
- `Models/` (ajustes menores si son necesarios)
- `Data/ApplicationDbContext.cs` (ajustes de Fluent API si son necesarios)
- `Migrations/[timestamp]_AjustesModelo.cs` (nueva migración)

**Mensaje:**
```
feat: aplicar ajustes de modelado y nueva migración

- Revisión de tipos de datos contra esquema original
- Migración con correcciones de modelado
- Verificación de coherencia BD ↔ Modelos
```

---

## UNIDAD 4 — Desarrollo con EF: Fluent API, Seeding y Vistas Parciales

---

### Commit 23 — Seeding con Entity Framework

**Descripción:** Implementar seeding de datos iniciales usando `HasData()` en `OnModelCreating`. Incluir: 2 Lugares (Comedor, Quincho), 12 Empresas, 60 Empleados con credenciales RF001-RF060. Generar migración que incluya los datos semilla. Los servicios y registros NO se seedean (se crean en runtime). Esto reemplaza el script SQL de datos iniciales.

**Archivos:**
- `Data/ApplicationDbContext.cs` (agregar HasData en OnModelCreating)
- `Migrations/[timestamp]_SeedDatos.cs` (nueva migración con seed)

**Mensaje:**
```
feat: agregar seeding de datos iniciales con EF Core HasData

- 2 lugares (Comedor, Quincho)
- 12 empresas del complejo industrial
- 60 empleados con credenciales RFID
- Migración generada con datos semilla
```

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

**Mensaje:**
```
refactor: separar configuraciones Fluent API por entidad

- IEntityTypeConfiguration<T> para cada modelo
- ApplyConfigurationsFromAssembly en OnModelCreating
- Mejor organización y mantenibilidad del contexto
```

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

**Mensaje:**
```
feat: crear vistas parciales reutilizables

- _ServicioCard para cards de servicio
- _EmpleadoRow para filas de empleado
- _KpiCard para indicadores estadísticos
- _FiltroFechas y _Paginacion como componentes comunes
- Vistas principales refactorizadas con partials
```

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

**Mensaje:**
```
feat: instalar y configurar ASP.NET Identity

- Modelo ApplicationUser con propiedades extendidas
- DbContext hereda de IdentityDbContext
- Configuración de Identity (contraseña, lockout, cookie)
- Migración para tablas de Identity
```

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

**Mensaje:**
```
feat: crear vistas de autenticación (Login, Logout, Registro)

- AccountController con autenticación completa
- Vistas de Login y Registro con estilo glassmorphism
- Topbar actualizada con nombre de usuario real
- Vista de acceso denegado
```

---

### Commit 28 — Roles y autorización por rol

**Descripción:** Crear roles del sistema: `Admin` y `Usuario`. Implementar seeding de roles y usuario admin inicial en `Program.cs` (al arrancar la app, crea roles si no existen y crea usuario admin por defecto). Agregar `[Authorize]` a todos los controladores. Agregar `[Authorize(Roles = "Admin")]` a los controladores de: Empresa, Empleado, Configuración. El rol `Usuario` solo puede acceder a: Home, Servicio, Registro, Reporte, Estadística. Actualizar sidebar para ocultar ítems según rol.

**Archivos:**
- `Program.cs` (seeding de roles y admin user al inicio)
- `Controllers/*.cs` (agregar atributos [Authorize] con roles)
- `Views/Shared/_Layout.cshtml` (mostrar/ocultar ítems por rol)

**Mensaje:**
```
feat: configurar roles y autorización por rol

- Roles Admin y Usuario con seeding automático
- Usuario admin creado al primer arranque
- [Authorize] en todos los controladores
- Restricción de módulos por rol
- Sidebar adaptativo según permisos del usuario
```

---

### Commit 29 — Gestión de usuarios

**Descripción:** Crear `UsuarioController` (solo Admin) para gestión de usuarios del sistema: listar usuarios con sus roles, crear nuevo usuario asignando rol, editar usuario (cambiar rol, resetear contraseña), desactivar usuario. Crear vista `Views/Usuario/Index.cshtml` con tabla de usuarios y formulario de edición. Agregar enlace en el sidebar (solo visible para Admin). Agregar entrada de navegación en el layout con icono.

**Archivos:**
- `Controllers/UsuarioController.cs` (crear)
- `Views/Usuario/Index.cshtml` (crear)
- `ViewModels/UsuarioViewModel.cs` (crear)
- `Views/Shared/_Layout.cshtml` (agregar ítem en sidebar para Admin)
- `wwwroot/css/site.css` (estilos de la vista)

**Mensaje:**
```
feat: crear gestión de usuarios del sistema

- CRUD de usuarios (crear, editar rol, resetear contraseña, desactivar)
- Vista con tabla de usuarios y formulario
- Solo accesible para rol Admin
- Nuevo ítem en sidebar para administración de usuarios
```

---

## UNIDAD 6 — Avanzado: Archivos, Email e IA

---

### Commit 30 — Manejo de archivos (foto de empleado)

**Descripción:** Agregar campo `FotoUrl` al modelo `Empleado`. Implementar subida de foto en el formulario de empleados (input file con preview, validación de tipo y tamaño). Guardar fotos en `wwwroot/uploads/empleados/`. Mostrar foto en la tabla de empleados, en el detalle del servicio y en las notificaciones de registro. Implementar servicio `IFileService` para subida, eliminación y validación de archivos. Generar migración para el nuevo campo.

**Archivos:**
- `Models/Empleado.cs` (agregar FotoUrl)
- `Services/IFileService.cs` + `Services/FileService.cs` (crear)
- `Controllers/EmpleadoController.cs` (actualizar Create/Edit para manejar archivo)
- `Views/Empleado/Index.cshtml` (agregar input file con preview)
- `Migrations/[timestamp]_AddFotoEmpleado.cs` (nueva migración)
- `Program.cs` (registrar FileService)

**Mensaje:**
```
feat: agregar manejo de archivos — foto de empleado

- Campo FotoUrl en modelo Empleado
- Servicio de archivos (subida, validación, eliminación)
- Preview de imagen en formulario
- Foto visible en tabla y detalle de servicio
```

---

### Commit 31 — Envío de email (reportes por correo)

**Descripción:** Implementar servicio de email usando `MailKit`/`MimeKit`. Configurar SMTP en `appsettings.json` (servidor, puerto, credenciales). Crear `IEmailService` con método `EnviarReporteAsync(destinatario, asunto, cuerpoHtml, archivoPdfAdjunto)`. Agregar botón "Enviar por Email" en la vista de reportes junto al botón de exportar PDF. El reporte se genera como PDF, se adjunta al email y se envía. Implementar modal para ingresar dirección de email destino.

**Archivos:**
- `Services/IEmailService.cs` + `Services/EmailService.cs` (crear)
- `SCA-MVC.csproj` (agregar MailKit)
- `appsettings.json` (agregar sección EmailSettings)
- `Controllers/ReporteController.cs` (agregar acción EnviarPorEmail)
- `Views/Reporte/Index.cshtml` (agregar botón y modal de email)
- `Program.cs` (registrar EmailService)

**Mensaje:**
```
feat: implementar envío de reportes por email

- Servicio de email con MailKit/MimeKit
- Configuración SMTP en appsettings.json
- Generación y adjunto de PDF en email
- Modal para ingresar destinatario
- Botón de envío en vista de reportes
```

---

### Commit 32 — Integración de LLM/IA (análisis inteligente)

**Descripción:** Integrar un servicio de IA (OpenAI API o similar) para análisis inteligente de datos. Crear `IIAService` con método `AnalizarDatosAsync(contexto)` que envía estadísticas del sistema a un LLM y recibe un análisis en texto. Agregar sección "Análisis IA" en la vista de Estadísticas con un botón "Generar Análisis" que solicita al LLM interpretar las tendencias, sugerir mejoras y predecir demanda. Configurar API key en `appsettings.json` (o user secrets). Mostrar respuesta en un panel con formato Markdown.

**Archivos:**
- `Services/IIAService.cs` + `Services/IAService.cs` (crear)
- `SCA-MVC.csproj` (agregar paquete HTTP client o SDK de OpenAI)
- `appsettings.json` (agregar sección IASettings con API key placeholder)
- `Controllers/EstadisticaController.cs` (agregar acción AnalizarConIA)
- `Views/Estadistica/Index.cshtml` (agregar sección de análisis IA)
- `wwwroot/js/site.js` (AJAX para solicitar análisis)
- `Program.cs` (registrar IAService)

**Mensaje:**
```
feat: integrar IA para análisis de datos

- Servicio de IA con conexión a LLM (OpenAI API)
- Análisis automático de tendencias y estadísticas
- Sección de análisis inteligente en vista de Estadísticas
- Configuración de API key en appsettings
- Respuesta formateada en Markdown
```

---

### Commit 33 — Refinamientos finales y documentación

**Descripción:** Realizar pruebas integrales de toda la funcionalidad. Corregir bugs encontrados. Agregar manejo global de excepciones con middleware personalizado. Agregar página de error personalizada (404, 500). Actualizar `README.md` del proyecto con: descripción, tecnologías, instrucciones de instalación, configuración, estructura del proyecto y capturas de pantalla. Limpiar código no utilizado.

**Archivos:**
- `Middleware/ExceptionMiddleware.cs` (crear)
- `Views/Shared/Error.cshtml` (mejorar)
- `Views/Shared/NotFound.cshtml` (crear)
- `Program.cs` (agregar middleware de excepciones)
- `README.md` (actualizar documentación completa)
- Varios archivos (corrección de bugs menores)

**Mensaje:**
```
feat: aplicar refinamientos finales y documentación

- Middleware global de manejo de excepciones
- Páginas de error personalizadas (404/500)
- README completo con instrucciones y capturas
- Corrección de bugs y limpieza de código
```

---

## Resumen por Unidad

| Unidad | Commits | Tema Principal |
|--------|---------|---------------|
| **Fase 0** | 1–8 | Diseños de vistas cshtml (estáticas) |
| **Unidad 2** | 9–19 | CRUD con ADO.NET, Stored Procedures, Validaciones |
| **Unidad 3** | 20–22 | Migración a Entity Framework, Modelado, Migraciones |
| **Unidad 4** | 23–25 | Seeding, Fluent API avanzado, Vistas parciales |
| **Unidad 5** | 26–29 | Identity, Login, Roles, Gestión de usuarios |
| **Unidad 6** | 30–33 | Archivos, Email, IA, Documentación final |

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
| Entity Framework | 20–22 |
| Modelado de clases | 21 |
| Configuración de contexto | 20, 21 |
| Migraciones | 22, 23 |
| Desarrollo con EF | 20–25 |
| Fluent API | 21, 24 |
| Seeding | 23 |
| Vistas parciales | 25 |
| Identity | 26 |
| Configuración de usuarios | 27 |
| Roles y permisos | 28 |
| Gestión de usuarios | 29 |
| Desarrollo con EF e Identity | 26–29 |
| Manejo de archivos | 30 |
| Envíos de email | 31 |
| Integración de LLM IA | 32 |
