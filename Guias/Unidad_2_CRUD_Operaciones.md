# Unidad 2: Operaciones CRUD con ASP.NET MVC

## Introducción a CRUD

**CRUD** es el acrónimo de las cuatro operaciones básicas de persistencia de datos:

- **C**reate (Crear)
- **R**ead (Leer)
- **U**pdate (Actualizar)
- **D**elete (Eliminar)

En esta unidad aprenderás a implementar estas operaciones en ASP.NET MVC para migrar la gestión de **Empresas** y **Empleados** de tu sistema actual.

---

## Estructura de un Controlador CRUD

### Anatomía de un Controlador

Un controlador CRUD típico contiene las siguientes acciones:

**Acciones GET (Mostrar vistas):**
1. **Index** - Lista todos los registros
2. **Detalles** - Muestra un registro específico
3. **Crear** - Muestra el formulario de creación
4. **Editar** - Muestra el formulario de edición
5. **Eliminar** - Muestra confirmación de eliminación

**Acciones POST (Procesar formularios):**
1. **Crear** - Procesa el formulario de creación
2. **Editar** - Procesa el formulario de edición
3. **Eliminar** - Procesa la confirmación de eliminación

### Inyección de Dependencias

El controlador recibe el `ApplicationDbContext` a través del constructor, permitiendo acceso a la base de datos.

---

## Migración: Gestión de Empresas

### 1. Modelo (Model)

**Migración desde WinForms:**
- Tu clase `dominio/Empresa.cs` se migra a `Models/Empresa.cs`
- Se agregan **Data Annotations** para validaciones
- Se agregan atributos **Display** para nombres amigables en vistas
- Se define la relación con Empleados

**Propiedades del modelo:**
- IdEmpresa (clave primaria)
- Nombre (requerido, máximo 100 caracteres)
- Estado (valor por defecto: true)
- Colección de Empleados (relación uno a muchos)

### 2. Controlador (Controller)

**Migración desde WinForms:**
- Los eventos de botones (`btnNuevo_Click`, `btnGuardar_Click`) se convierten en acciones del controlador
- Las llamadas a `EmpresaNegocio.cs` se reemplazan por consultas LINQ con Entity Framework
- La actualización manual del DataGridView se reemplaza por el retorno de vistas

**Acciones del controlador:**

**Index (Listar):**
- Obtiene todas las empresas de la base de datos
- Incluye la relación con empleados
- Retorna la vista con la lista

**Detalles (Ver):**
- Recibe el ID como parámetro
- Busca la empresa en la base de datos
- Retorna la vista con los detalles

**Crear GET:**
- Simplemente retorna la vista con el formulario vacío

**Crear POST:**
- Recibe el objeto Empresa del formulario
- Valida el ModelState
- Si es válido, agrega a la base de datos y guarda
- Redirige al Index con mensaje de éxito

**Editar GET:**
- Recibe el ID como parámetro
- Busca la empresa en la base de datos
- Retorna la vista con el formulario pre-llenado

**Editar POST:**
- Recibe el objeto Empresa modificado
- Valida el ModelState
- Si es válido, actualiza en la base de datos
- Redirige al Index con mensaje de éxito

**Eliminar GET:**
- Muestra la información de la empresa
- Solicita confirmación

**Eliminar POST:**
- Realiza baja lógica (cambia Estado a false)
- Redirige al Index con mensaje de éxito

### 3. Vistas (Views)

#### Index.cshtml (Listado)

**Migración desde WinForms:**
- El DataGridView se convierte en una tabla HTML
- Los botones de acción se convierten en enlaces

**Elementos de la vista:**
- Título de la página
- Botón "Nueva Empresa" que enlaza a la acción Crear
- Tabla con columnas:
  - Nombre
  - Estado (con badge visual)
  - Cantidad de empleados
  - Acciones (Ver, Editar, Eliminar)
- Cada fila representa una empresa
- Los enlaces usan helpers de Razor para generar URLs

#### Crear.cshtml (Formulario de Alta)

**Migración desde WinForms:**
- El panel con TextBox se convierte en un formulario HTML
- Los botones se convierten en botones de submit y enlaces

**Elementos de la vista:**
- Título "Nueva Empresa"
- Formulario con método POST
- Campo de texto para Nombre
- Checkbox para Estado
- Mensajes de validación
- Botón "Guardar" (submit)
- Botón "Cancelar" (enlace al Index)

#### Editar.cshtml (Formulario de Modificación)

**Elementos de la vista:**
- Similar a Crear.cshtml
- Incluye campo oculto con el IdEmpresa
- Los campos vienen pre-llenados con los datos actuales
- Título "Editar Empresa"

#### Eliminar.cshtml (Confirmación de Baja)

**Elementos de la vista:**
- Mensaje de advertencia
- Información de la empresa a eliminar
- Formulario con método POST
- Botón "Confirmar Eliminación"
- Botón "Cancelar"

---

## Migración: Gestión de Empleados

### 1. Modelo con Relaciones

**Migración desde WinForms:**
- Tu clase `dominio/Empleado.cs` se migra a `Models/Empleado.cs`
- Se agregan Data Annotations para validaciones
- Se definen relaciones con Empresa y Registros

**Propiedades del modelo:**
- IdEmpleado (clave primaria)
- Nombre (requerido, máximo 50 caracteres)
- Apellido (requerido, máximo 50 caracteres)
- IdEmpresa (clave foránea, requerido)
- IdCredencial (opcional, máximo 20 caracteres)
- Estado (valor por defecto: true)
- Empresa (propiedad de navegación)
- Registros (colección de registros)
- NombreCompleto (propiedad calculada)

### 2. Controlador con Relaciones

**Diferencias con el controlador de Empresas:**

**Index:**
- Incluye la relación con Empresa usando `Include()`
- Ordena por apellido y nombre

**Crear GET:**
- Carga la lista de empresas activas en el ViewBag
- Pasa la lista al DropDownList de la vista

**Crear POST:**
- Valida que la credencial sea única (si se proporciona)
- Si ya existe, agrega error al ModelState
- Si es válida, crea el empleado

**Editar GET:**
- Similar a Crear GET
- Carga las empresas en el ViewBag

**Editar POST:**
- Valida credencial única excluyendo el empleado actual
- Actualiza el empleado si es válido

**Método auxiliar:**
- `CargarEmpresas()` - Método privado que carga las empresas activas en el ViewBag para los DropDownList

### 3. Vista con DropDownList

**Crear.cshtml / Editar.cshtml:**

**Elementos adicionales:**
- Campo de texto para Nombre
- Campo de texto para Apellido
- **DropDownList** para seleccionar Empresa
  - Carga las opciones desde ViewBag
  - Muestra "-- Seleccione una empresa --" como opción por defecto
- Campo de texto para IdCredencial (opcional)
- Checkbox para Estado
- Mensajes de validación para cada campo

---

## Búsqueda y Filtrado

### Implementar Búsqueda en el Listado

**Modificaciones al controlador:**

**Acción Index modificada:**
- Acepta parámetros opcionales: `buscar` (string) y `empresaId` (int?)
- Crea una consulta base con todas las entidades
- Si hay texto de búsqueda, filtra por nombre, apellido o credencial
- Si hay empresa seleccionada, filtra por esa empresa
- Ordena los resultados
- Carga las empresas en ViewBag para el filtro

**Elementos de la vista:**

**Formulario de búsqueda:**
- Campo de texto para buscar
- DropDownList para filtrar por empresa
- Botón "Buscar"
- Botón "Limpiar" que vuelve al Index sin filtros

---

## Mensajes de Confirmación con TempData

### ¿Qué es TempData?

**TempData** es un diccionario que permite pasar datos entre acciones, típicamente después de un redirect. Los datos persisten solo para la siguiente petición.

### Uso en el Controlador

**Después de crear/editar/eliminar:**
- Se agrega un mensaje a TempData con clave "Mensaje"
- Opcionalmente se agrega "TipoMensaje" (success, danger, warning, info)
- Se redirige a la acción Index

### Mostrar en el Layout

**En _Layout.cshtml:**
- Se verifica si existe TempData["Mensaje"]
- Si existe, se muestra un alert de Bootstrap
- El tipo de alert depende de TempData["TipoMensaje"]
- El alert es dismissible (se puede cerrar)

---

## Validaciones

### Validaciones del Lado del Servidor

**Data Annotations en el modelo:**
- `[Required]` - Campo obligatorio
- `[StringLength]` - Longitud máxima/mínima
- `[Range]` - Rango de valores numéricos
- `[EmailAddress]` - Formato de email
- `[Display]` - Nombre amigable para mostrar

**Validaciones personalizadas en el controlador:**
- Verificar unicidad de credenciales
- Validar que la empresa esté activa
- Verificar reglas de negocio específicas

**ModelState:**
- Se valida automáticamente con `ModelState.IsValid`
- Se pueden agregar errores manualmente con `ModelState.AddModelError()`

### Validaciones del Lado del Cliente

**Scripts necesarios:**
- jQuery Validation
- jQuery Validation Unobtrusive

**Inclusión en las vistas:**
- Se incluyen en la sección Scripts
- Se usa una vista parcial `_ValidationScriptsPartial`
- Las validaciones se ejecutan antes de enviar el formulario

**Ventajas:**
- Feedback inmediato al usuario
- Reduce peticiones al servidor
- Mejora la experiencia de usuario

---

## Comparación Final: WinForms vs MVC

### Flujo de Creación de Empleado

**WinForms:**
1. Usuario hace clic en "Nuevo"
2. Se limpian los TextBox
3. Usuario llena los campos
4. Usuario hace clic en "Guardar"
5. Evento `btnGuardar_Click` se dispara
6. Se validan los datos manualmente
7. Se llama a `EmpleadoNegocio.Agregar()`
8. Se actualiza el DataGridView
9. Se muestra MessageBox de confirmación

**MVC:**
1. Usuario navega a `/Empleados/Crear`
2. Se muestra el formulario HTML
3. Usuario llena los campos
4. Usuario hace clic en "Guardar"
5. POST a `/Empleados/Crear`
6. ModelState valida automáticamente
7. Se guarda con Entity Framework
8. Redirect a `/Empleados/Index`
9. Se muestra alerta con TempData

---

## Buenas Prácticas

### Organización del Código

**Controladores:**
- Mantener las acciones simples y enfocadas
- Extraer lógica compleja a métodos privados
- Usar métodos auxiliares para código repetitivo

**Vistas:**
- Mantener la lógica mínima en las vistas
- Usar vistas parciales para componentes reutilizables
- Seguir las convenciones de nombres

**Modelos:**
- Usar Data Annotations para validaciones simples
- Documentar propiedades complejas
- Mantener las propiedades calculadas simples

### Convenciones de Nombres

**Controladores:**
- Nombre en plural (EmpleadosController, EmpresasController)
- Sufijo "Controller"

**Vistas:**
- Carpeta con nombre del controlador sin "Controller"
- Archivos con nombre de la acción
- Vistas parciales con prefijo "_"

**Modelos:**
- Nombre en singular (Empleado, Empresa)
- Propiedades en PascalCase

---

## Ejercicios Prácticos

### Ejercicio 1: Implementar CRUD de Lugares

Crea el CRUD completo para la entidad `Lugar` (Comedor/Quincho):

**Tareas:**
1. Crear el modelo Lugar con validaciones
2. Crear el controlador LugaresController
3. Crear las vistas (Index, Crear, Editar, Eliminar, Detalles)
4. Implementar búsqueda por nombre
5. Agregar mensajes de confirmación

### Ejercicio 2: Mejorar el Listado de Empleados

**Tareas:**
1. Agregar filtro por estado (Activos/Inactivos/Todos)
2. Agregar ordenamiento por columnas
3. Mostrar la cantidad total de empleados
4. Agregar botón para exportar a Excel (opcional)

### Ejercicio 3: Validaciones Adicionales

**Tareas:**
1. Validar que el nombre de la empresa sea único
2. Validar que no se pueda desactivar una empresa con empleados activos
3. Agregar validación de formato para la credencial (solo números)

---

## Resumen de la Unidad

### Lo que Aprendiste

✅ Estructura de un controlador CRUD completo
✅ Migración de la gestión de Empresas de WinForms a MVC
✅ Migración de la gestión de Empleados con relaciones
✅ Creación de vistas con formularios
✅ Implementación de búsqueda y filtrado
✅ Uso de TempData para mensajes
✅ Validaciones del lado del servidor y cliente

### Archivos Creados

**Modelos:**
- `Models/Empresa.cs`
- `Models/Empleado.cs`

**Controladores:**
- `Controllers/EmpresasController.cs`
- `Controllers/EmpleadosController.cs`

**Vistas:**
- `Views/Empresas/` (Index, Crear, Editar, Eliminar, Detalles)
- `Views/Empleados/` (Index, Crear, Editar, Eliminar, Detalles)

---
