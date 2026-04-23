# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Compilar
dotnet build

# Ejecutar en desarrollo
dotnet run

# Migraciones EF Core
dotnet ef migrations add <NombreMigracion>
dotnet ef database update
dotnet ef migrations remove   # deshacer última migración no aplicada
```

No hay proyecto de tests en el repositorio.

## Arquitectura

**ASP.NET Core 9 MVC** con capas bien separadas: Controllers → Services (interfaz + implementación) → DbContext. No hay repositorio genérico — cada entidad tiene su propio par `IXxxNegocio` / `XxxNegocio` inyectado con `AddScoped`.

### Capa de servicios (`Services/`)

Toda la lógica de negocio y acceso a datos vive en `Services/`. Cada servicio tiene una interfaz (`IXxxNegocio.cs`) y su implementación (`XxxNegocio.cs`). Los controladores nunca acceden a `ApplicationDbContext` directamente. Los servicios se registran en `Program.cs` con `AddScoped`.

### Concepto central: Servicio activo

`Servicio` es la entidad que representa una sesión de almuerzo. El estado se codifica en `DuracionMinutos`:
- `DuracionMinutos == null` → servicio **activo** (en curso)
- `DuracionMinutos != null` → servicio **finalizado**

`ServicioNegocio.ObtenerActivoGlobalAsync()` es el punto de entrada para cualquier operación de registro. `FinalizarPendientesAsync()` se llama al cargar `ServicioController.Index()` para auto-cerrar servicios de días anteriores.

### Workaround con triggers SQL Server

`RegistroNegocio.RegistrarAsync()` usa `ExecuteSqlRawAsync` en lugar de `_context.Registros.Add()` + `SaveChangesAsync()`. Esto es intencional: la tabla `Registros` tiene triggers activos en SQL Server, y EF Core usa la cláusula `OUTPUT` para recuperar el PK generado, lo que es incompatible con triggers. No cambiar este patrón.

### Feedback al usuario

Los controladores usan los métodos de extensión de `Helpers/MensajesUI.cs` sobre `TempData`:
```csharp
TempData.MostrarExito("Mensaje");
TempData.MostrarError("Mensaje");
TempData.MostrarAdvertencia("Mensaje");
TempData.MostrarInfo("Mensaje");
```
El layout `_Layout.cshtml` lee esas claves de TempData y muestra toasts. No usar `ViewBag` para mensajes de feedback.

### Seeding de datos

Hay **dos mecanismos** de seed que no deben confundirse:
1. **`ApplicationDbContext.SeedData()`** (en `OnModelCreating`): seed estático con `HasData()` — lugares, empresas, empleados, servicios y registros de demo (febrero 2026). Se aplica mediante migraciones.
2. **`Program.cs` al arranque**: seed dinámico de roles (`Admin`, `Usuario`) y usuario administrador (`admin@sca.com` / `Admin123`). Es idempotente.

### Identity

`ApplicationUser` extiende `IdentityUser` con `NombreUsuario`, `Nombre` y `Apellido`. `AppUserClaimsPrincipalFactory` agrega `NombreUsuario` como claim para mostrarlo en el topbar sin hacer una query extra. `SpanishIdentityErrorDescriber` localiza los mensajes de error de Identity al español.

### Autorización

- `[Authorize]` en todos los controladores operacionales.
- `[Authorize(Roles = "Admin")]` en `EmpresaController`, `EmpleadoController`, `AdminController` y `ReporteController`.
- Redirecciones configuradas en `Program.cs`: login → `/Account/Login`, acceso denegado → `/Account/AccessDenied`.

### Configuración sensible

`appsettings.json` contiene las credenciales SMTP (`EmailSettings`) y la cadena de conexión. Para desarrollo local, la conexión usa Windows Auth (`Integrated Security=true`). La cadena con SQL Auth (`DefaultConnection_SQLAuth`) está incluida como referencia pero no se usa por defecto.
