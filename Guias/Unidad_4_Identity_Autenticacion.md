# Unidad 4: ASP.NET Identity - Autenticación y Autorización

## Introducción a ASP.NET Identity

**ASP.NET Identity** es el sistema de gestión de usuarios y autenticación integrado en ASP.NET Core. Proporciona:

- Registro de usuarios
- Inicio de sesión (Login)
- Cierre de sesión (Logout)
- Gestión de roles
- Autorización basada en roles
- Seguridad de contraseñas
- Bloqueo de cuentas

---

## ¿Por Qué Necesitas Autenticación?

### Tu Sistema Actual (WinForms)

**No tiene autenticación porque:**
- Es una aplicación de escritorio local
- Solo personal autorizado tiene acceso físico a la computadora
- Se ejecuta en un entorno controlado

### Tu Sistema Web Necesita Autenticación Porque:

**Razones de seguridad:**
- Es accesible desde cualquier navegador
- Múltiples usuarios pueden acceder simultáneamente
- Necesitas diferentes niveles de acceso
- Está expuesto a internet o red corporativa

**Beneficios:**
- Control de acceso granular
- Auditoría de acciones por usuario
- Protección de datos sensibles
- Cumplimiento de normativas

---

## Roles en tu Sistema

### Roles Propuestos

**1. Administrador**
- Acceso completo al sistema
- Gestión de empleados y empresas
- Configuración del sistema
- Respaldos de base de datos
- Gestión de usuarios y roles
- Ver todos los reportes

**2. Personal de Cocina**
- Registro de comensales por credencial
- Registro manual de comensales
- Ver servicio activo
- Ver listado en tiempo real
- No puede modificar configuraciones

**3. Supervisor**
- Gestión de servicios (iniciar/finalizar)
- Reportes y estadísticas
- Consulta de históricos
- No puede gestionar usuarios
- No puede modificar empleados/empresas

---

## Configuración de ASP.NET Identity

### 1. Instalación de Paquetes NuGet

**Paquetes necesarios:**
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** - Integración con EF Core
- **Microsoft.AspNetCore.Identity.UI** - UI predefinida (opcional)

### 2. Modelo de Usuario Personalizado

**Clase ApplicationUser:**
- Hereda de `IdentityUser`
- Agrega propiedades personalizadas:
  - Nombre
  - Apellido
  - Activo (bool)
  - FechaCreacion
- Propiedad calculada NombreCompleto

**Características:**
- Atributo `[PersonalData]` para datos personales
- Validaciones con Data Annotations
- Propiedades heredadas de IdentityUser (Email, UserName, etc.)

### 3. Modificar DbContext

**ApplicationDbContext:**
- Cambia de heredar `DbContext` a `IdentityDbContext<ApplicationUser>`
- Llama a `base.OnModelCreating()` en OnModelCreating (¡IMPORTANTE!)
- Agrega seed de roles en OnModelCreating

**Tablas que se crean:**
- AspNetUsers (usuarios)
- AspNetRoles (roles)
- AspNetUserRoles (relación usuarios-roles)
- AspNetUserClaims, AspNetRoleClaims, etc.

### 4. Configurar Identity en Program.cs

**Configuración de Identity:**
- Agregar servicio AddIdentity con ApplicationUser y IdentityRole
- Configurar opciones de contraseñas:
  - Requerir dígito
  - Requerir minúscula/mayúscula
  - Requerir caracteres especiales (opcional)
  - Longitud mínima
- Configurar opciones de usuarios:
  - Email único
- Configurar opciones de bloqueo:
  - Tiempo de bloqueo
  - Intentos máximos fallidos

**Configuración de cookies:**
- Ruta de login
- Ruta de logout
- Ruta de acceso denegado
- Tiempo de expiración
- Sliding expiration

**Middleware:**
- UseAuthentication() - Antes de UseAuthorization()
- UseAuthorization() - Después de UseAuthentication()

### 5. Crear Migración

**Comando:**
- `dotnet ef migrations add AgregarIdentity`
- `dotnet ef database update`

**Resultado:**
- Se crean todas las tablas de Identity
- Se agregan los roles iniciales

---

## Controlador de Cuenta (Account)

### AccountController

**Servicios inyectados:**
- UserManager<ApplicationUser> - Gestión de usuarios
- SignInManager<ApplicationUser> - Gestión de sesiones

**Acciones principales:**

**Login GET:**
- Muestra el formulario de inicio de sesión
- Recibe returnUrl opcional
- Pasa returnUrl a la vista

**Login POST:**
- Recibe LoginViewModel del formulario
- Valida ModelState
- Intenta iniciar sesión con PasswordSignInAsync()
- Si es exitoso, redirige a returnUrl o página principal
- Si falla, muestra error
- Si está bloqueado, muestra mensaje específico

**Logout POST:**
- Cierra la sesión con SignOutAsync()
- Redirige a la página principal

**AccesoDenegado GET:**
- Muestra página de acceso denegado
- Se llama automáticamente cuando un usuario no autorizado intenta acceder

**Método auxiliar RedirectToLocal:**
- Valida que returnUrl sea local (seguridad)
- Previene ataques de redirección abierta

### ViewModels

**LoginViewModel:**
- Email (requerido, formato email)
- Password (requerido, tipo password)
- RememberMe (checkbox)

**RegisterViewModel:**
- Nombre (requerido)
- Apellido (requerido)
- Email (requerido, formato email)
- Password (requerido, longitud mínima)
- ConfirmPassword (debe coincidir con Password)
- Rol (requerido, para asignar rol al crear)

---

## Vistas de Autenticación

### Login.cshtml

**Elementos de la vista:**
- Título del sistema
- Formulario con método POST
- Campo de email con autofocus
- Campo de contraseña (tipo password)
- Checkbox "Recordarme"
- Botón de envío
- Mensajes de validación
- Diseño centrado y atractivo

**Características:**
- Responsive (se adapta a móviles)
- Validación del lado del cliente
- Feedback visual de errores

### AccesoDenegado.cshtml

**Elementos de la vista:**
- Mensaje de error amigable
- Explicación de por qué no tiene acceso
- Enlace para volver a la página principal
- Opción de cerrar sesión

---

## Autorización en Controladores

### Atributo [Authorize]

**Uso básico:**
- Se coloca sobre la clase del controlador o acción específica
- Requiere que el usuario esté autenticado
- Redirige a Login si no está autenticado

**Autorización por rol:**
- `[Authorize(Roles = "Administrador")]` - Solo administradores
- `[Authorize(Roles = "Administrador,Supervisor")]` - Admin o Supervisor
- Se puede aplicar a nivel de controlador o acción

**Ejemplos de aplicación:**

**EmpleadosController:**
- Clase: `[Authorize]` - Requiere autenticación
- Crear/Editar/Eliminar: `[Authorize(Roles = "Administrador")]`

**ServiciosController:**
- Clase: `[Authorize]`
- Crear/Finalizar: `[Authorize(Roles = "Administrador,Supervisor")]`

**RegistrosController:**
- Clase: `[Authorize]` - Todos los roles pueden registrar

**UsuariosController:**
- Clase: `[Authorize(Roles = "Administrador")]` - Solo admin

---

## Autorización en Vistas

### Mostrar/Ocultar según Autenticación

**User.Identity.IsAuthenticated:**
- Verifica si el usuario está autenticado
- Muestra diferentes opciones según el estado

**Casos de uso:**
- Mostrar "Bienvenido, [nombre]" si está autenticado
- Mostrar "Iniciar Sesión" si no está autenticado
- Mostrar botón de "Cerrar Sesión" solo si está autenticado

### Mostrar/Ocultar según Rol

**User.IsInRole("NombreRol"):**
- Verifica si el usuario tiene un rol específico
- Permite mostrar opciones solo a ciertos roles

**Casos de uso:**
- Mostrar "Gestión de Usuarios" solo a Administradores
- Mostrar "Reportes" a Administradores y Supervisores
- Ocultar botones de eliminar a Personal de Cocina

**Ejemplos de aplicación:**
- Menú de navegación con opciones según rol
- Botones de acción en listados
- Enlaces a funcionalidades restringidas

---

## Gestión de Usuarios (Admin)

### UsuariosController

**Solo accesible por Administradores:**
- Atributo `[Authorize(Roles = "Administrador")]` en la clase

**Servicios inyectados:**
- UserManager<ApplicationUser>
- RoleManager<IdentityRole>

**Acciones principales:**

**Index:**
- Lista todos los usuarios
- Para cada usuario, obtiene sus roles
- Crea ViewModel con información completa
- Muestra: nombre, email, roles, estado, fecha de creación

**Crear GET:**
- Carga lista de roles en ViewBag
- Muestra formulario de registro

**Crear POST:**
- Recibe RegisterViewModel
- Crea nuevo ApplicationUser
- Asigna contraseña con CreateAsync()
- Asigna rol con AddToRoleAsync()
- Muestra mensaje de éxito

**CambiarEstado POST:**
- Recibe ID de usuario
- Alterna el estado Activo/Inactivo
- Actualiza con UpdateAsync()
- Muestra mensaje de confirmación

**Características adicionales:**
- No se puede desactivar el propio usuario
- No se puede desactivar el último administrador
- Validación de email único

---

## Información del Usuario en Vistas

### Layout con Usuario Actual

**Elementos en el header:**
- Nombre del usuario autenticado (User.Identity.Name)
- Badge con el rol del usuario
- Botón de cerrar sesión

**Información disponible:**
- User.Identity.Name - Email del usuario
- User.Identity.IsAuthenticated - Si está autenticado
- User.IsInRole("Rol") - Si tiene un rol específico

**Personalización por rol:**
- Badge rojo para Administrador
- Badge amarillo para Supervisor
- Badge azul para Cocina

---

## Crear Usuario Administrador Inicial

### Clase de Inicialización

**DbInitializer.cs:**
- Método estático Initialize()
- Recibe UserManager y RoleManager
- Crea roles si no existen
- Crea usuario administrador por defecto

**Roles a crear:**
- Administrador
- Supervisor
- Cocina

**Usuario administrador por defecto:**
- Email: admin@sistema.com
- Contraseña: Admin123! (cambiar en producción)
- Nombre: Administrador
- Apellido: Sistema
- EmailConfirmed: true

**Proceso:**
1. Verificar si los roles existen
2. Crear roles faltantes
3. Verificar si existe el usuario admin
4. Si no existe, crearlo con contraseña
5. Asignar rol de Administrador

### Llamar en Program.cs

**Después de configurar la app:**
- Crear scope de servicios
- Obtener UserManager y RoleManager
- Llamar a DbInitializer.Initialize()
- Esperar a que complete (await)

**Resultado:**
- Al iniciar la aplicación por primera vez
- Se crean los roles
- Se crea el usuario administrador
- Se puede iniciar sesión con admin@sistema.com

---

## Seguridad de Contraseñas

### Configuración de Requisitos

**Opciones configurables:**
- RequireDigit - Requiere al menos un dígito
- RequireLowercase - Requiere minúscula
- RequireUppercase - Requiere mayúscula
- RequireNonAlphanumeric - Requiere carácter especial
- RequiredLength - Longitud mínima

**Recomendaciones:**
- Mínimo 6-8 caracteres
- Combinar mayúsculas, minúsculas y números
- Caracteres especiales opcionales (pueden complicar)

### Bloqueo de Cuentas

**Configuración:**
- DefaultLockoutTimeSpan - Tiempo de bloqueo (ej: 15 minutos)
- MaxFailedAccessAttempts - Intentos máximos (ej: 5)

**Funcionamiento:**
- Después de X intentos fallidos, la cuenta se bloquea
- El usuario no puede iniciar sesión durante el tiempo configurado
- Previene ataques de fuerza bruta

---

## Ejercicios Prácticos

### Ejercicio 1: Implementar Identity

**Tareas:**
1. Instalar paquetes NuGet necesarios
2. Crear clase ApplicationUser
3. Modificar ApplicationDbContext
4. Configurar Identity en Program.cs
5. Crear y aplicar migración

### Ejercicio 2: Proteger Controladores

**Tareas:**
1. Aplicar [Authorize] a todos los controladores
2. Aplicar [Authorize(Roles = "...")] según corresponda
3. Verificar que funcione correctamente
4. Probar con diferentes roles

### Ejercicio 3: Crear Usuario Admin

**Tareas:**
1. Crear clase DbInitializer
2. Implementar método Initialize
3. Llamar desde Program.cs
4. Verificar que se cree el usuario
5. Iniciar sesión con el usuario admin

---

