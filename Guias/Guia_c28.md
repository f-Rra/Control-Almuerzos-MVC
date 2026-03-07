# Guía del Commit 28: Roles y Autorización por Rol

## 🎯 Propósito

Implementar el sistema de roles del proyecto: crear los roles `Admin` y `Usuario`, poblarlos automáticamente al arrancar la app junto con un usuario administrador por defecto, proteger todos los controladores con `[Authorize]`, restringir los módulos de administración al rol `Admin`, y actualizar el sidebar para que muestre el enlace de Administración solo a los usuarios con ese rol.

---

## 📝 Concepto Central

### Autenticación vs. Autorización

Con el Commit 27 el sistema sabe **quién** es el usuario (autenticación). Este commit resuelve **qué puede hacer** (autorización):

```
Autenticación → "¿Quién sos?"   → Identity verifica credenciales → emite cookie
Autorización  → "¿Qué podés?" → [Authorize] verifica roles/claims → permite o deniega
```

### Roles en ASP.NET Identity

Los roles son simplemente cadenas de texto almacenadas en la tabla `AspNetRoles`. La asignación usuario ↔ rol vive en `AspNetUserRoles`. Identity expone esta información como **claims** dentro de la cookie, por lo que `User.IsInRole("Admin")` funciona sin consultar la BD en cada request.

```
AspNetRoles            AspNetUserRoles
┌──────────────┐       ┌──────────────────────────────┐
│ Id │ Name    │  ←──  │ UserId          │ RoleId      │
│ .. │ Admin   │       │ (guid usuario)  │ (guid rol)  │
│ .. │ Usuario │       └──────────────────────────────┘
└──────────────┘
```

### El atributo `[Authorize]`

| Atributo | Comportamiento |
|---|---|
| `[Authorize]` | Requiere que el usuario esté autenticado (cualquier rol) |
| `[Authorize(Roles = "Admin")]` | Requiere que el usuario tenga el rol `Admin` |
| `[AllowAnonymous]` | Permite acceso sin autenticar (sobreescribe `[Authorize]` superior) |

Cuando se aplica al controlador completo, todas sus acciones heredan la restricción. Las acciones individuales pueden sobreescribirla con `[AllowAnonymous]`.

### Seeding idempotente vs. migración HasData

El seeding de usuarios y roles **no** se hace en `HasData()` porque:

1. Las contraseñas necesitan ser hasheadas en runtime con el `UserManager` (que no está disponible en tiempo de migración).
2. Los IDs de usuarios y roles que genera Identity son GUIDs aleatorios, lo que causaría conflictos con migraciones futuras si se fijan en el código.

La solución correcta es crear un bloque `using (var scope = ...)` después de `app.Build()` que use los servicios ya inyectados:

```csharp
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    // ... crear roles y usuario admin
}
```

Este bloque se ejecuta **una sola vez al arrancar** y es **idempotente**: verifica si los roles/usuarios ya existen antes de crearlos, por lo que es seguro correrlo en cada inicio.

### `User.IsInRole()` en Razor

En las vistas, `User` es el `ClaimsPrincipal` del request actual (el mismo que en los controladores). La comprobación en Razor es directa:

```razor
@if (User.IsInRole("Admin"))
{
    <!-- Solo visible para Admin -->
}
```

Identity inyecta el rol como un **claim de tipo `ClaimTypes.Role`** en la cookie al hacer login, por eso esta verificación no requiere consulta a la BD.

---

## 📁 Archivos Creados / Modificados

| Archivo | Acción | Descripción |
|---|---|---|
| `Program.cs` | Modificado | Seeding idempotente de roles y usuario admin al arranque |
| `Controllers/HomeController.cs` | Modificado | `[Authorize]` — todos los usuarios autenticados |
| `Controllers/ServicioController.cs` | Modificado | `[Authorize]` — todos los usuarios autenticados |
| `Controllers/RegistroController.cs` | Modificado | `[Authorize]` — todos los usuarios autenticados |
| `Controllers/ReporteController.cs` | Modificado | `[Authorize]` — todos los usuarios autenticados |
| `Controllers/EstadisticaController.cs` | Modificado | `[Authorize]` — todos los usuarios autenticados |
| `Controllers/EmpresaController.cs` | Modificado | `[Authorize(Roles = "Admin")]` — solo administradores |
| `Controllers/EmpleadoController.cs` | Modificado | `[Authorize(Roles = "Admin")]` — solo administradores |
| `Controllers/AdminController.cs` | Modificado | `[Authorize(Roles = "Admin")]` — solo administradores |
| `Views/Shared/_Layout.cshtml` | Modificado | Enlace ADMINISTRACIÓN visible solo para el rol Admin |
| `wwwroot/css/site.css` | Modificado | Clase `.side-separator` para divisor visual en sidebar |

---

## ⚡ Detalle de Cambios

### 1. `Program.cs` — Seeding de roles y usuario admin

Se agrega el bloque de seeding entre `app.Build()` y `app.Run()`. El bloque crea un scope temporal para acceder a los servicios de Identity:

```csharp
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Crear roles si no existen
    string[] roles = ["Admin", "Usuario"];
    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
            await roleManager.CreateAsync(new IdentityRole(rol));
    }

    // Crear usuario admin por defecto si no existe
    const string adminEmail    = "admin@sca.com";
    const string adminPassword = "Admin123";

    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var adminUser = new ApplicationUser
        {
            UserName       = adminEmail,
            Email          = adminEmail,
            NombreUsuario  = "Administrador",
            EmailConfirmed = true
        };

        var resultado = await userManager.CreateAsync(adminUser, adminPassword);
        if (resultado.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
```

> **¿Por qué `EmailConfirmed = true`?**
> La configuración de Identity tiene `RequireConfirmedAccount = false`, pero establecerlo en `true` explícitamente evita problemas si ese setting cambia en el futuro.

> **¿Por qué verificar `FindByEmailAsync` antes de crear?**
> El seeding se ejecuta en cada inicio de la app. Sin la verificación, intentaría crear el mismo usuario cada vez y fallaría con un error de email duplicado.

---

### 2. Controladores — atributos de autorización

Se agrega `using Microsoft.AspNetCore.Authorization;` y el atributo correspondiente a nivel de clase en cada controlador:

**Acceso para cualquier usuario autenticado** (`[Authorize]`):
```csharp
[Authorize]
public class HomeController : Controller { ... }

[Authorize]
public class ServicioController : Controller { ... }

[Authorize]
public class RegistroController : Controller { ... }

[Authorize]
public class ReporteController : Controller { ... }

[Authorize]
public class EstadisticaController : Controller { ... }
```

**Acceso exclusivo para Admin** (`[Authorize(Roles = "Admin")]`):
```csharp
[Authorize(Roles = "Admin")]
public class EmpresaController : Controller { ... }

[Authorize(Roles = "Admin")]
public class EmpleadoController : Controller { ... }

[Authorize(Roles = "Admin")]
public class AdminController : Controller { ... }
```

**`AccountController` no cambia** — ya tenía `[AllowAnonymous]` en las acciones públicas (Login, Register, AccessDenied), lo que es suficiente dado que el controlador no tiene `[Authorize]` a nivel de clase.

#### ¿Qué pasa cuando un usuario sin el rol correcto accede?

ASP.NET devuelve un **403 Forbidden** y redirige automáticamente a la ruta configurada en `ConfigureApplicationCookie`:

```csharp
options.AccessDeniedPath = "/Account/AccessDenied";
```

Si el usuario no está autenticado en absoluto, devuelve **401 Unauthorized** y redirige a:
```csharp
options.LoginPath = "/Account/Login";
```

---

### 3. `Views/Shared/_Layout.cshtml` — sidebar adaptativo

Se agrega un bloque condicional al final de la navegación del sidebar, precedido por un separador visual:

```razor
@if (User.IsInRole("Admin"))
{
    <div class="side-separator"></div>
    <a asp-controller="Admin" asp-action="Index"
       class="mi @(controller == "Admin" ? "active" : "")"
       data-tip="Administración">
        <div class="bar"></div>
        <div class="ico"><img src="~/images/usuario.png" /></div>
        <span class="lbl">ADMINISTRACIÓN</span>
    </a>
}
```

El separador divide visualmente la sección operacional (Home, Servicios, etc.) de la sección administrativa. El enlace está activo (`active`) cuando el controller actual es `Admin`.

---

### 4. `wwwroot/css/site.css` — separador de sidebar

```css
/* Separador visual entre secciones del sidebar (ej. operacional / admin) */
.side-separator {
    height: 1px;
    background: rgba(0, 0, 0, 0.1);
    margin: 8px 20px 12px;
    border-radius: 2px;
}
```

---

## 🛠️ Pasos Realizados

1. **Agregar seeding en `Program.cs`** — bloque `using (var scope = ...)` entre `app.Build()` y `app.Run()`, que crea los roles `Admin` y `Usuario` y el usuario `admin@sca.com` si no existen.

2. **Agregar `[Authorize]` en `HomeController`, `ServicioController`, `RegistroController`, `ReporteController` y `EstadisticaController`** — along with `using Microsoft.AspNetCore.Authorization;`.

3. **Agregar `[Authorize(Roles = "Admin")]` en `EmpresaController`, `EmpleadoController` y `AdminController`**.

4. **Actualizar `_Layout.cshtml`** — agregar el bloque `@if (User.IsInRole("Admin"))` con el enlace ADMINISTRACIÓN y el separador.

5. **Agregar `.side-separator`** en `site.css`.

6. **Compilar** (`dotnet build`) para verificar 0 errores.

---

## ✅ Verificación

### Flujo con usuario Admin

1. Iniciar la app — el seeding crea los roles y el usuario admin automáticamente.
2. Ir a `/Account/Login` → ingresar `admin@sca.com` / `Admin123`.
3. Verificar que el sidebar muestra el enlace **ADMINISTRACIÓN** separado del resto.
4. Navegar a `/Empresa` → funciona (Admin tiene acceso).
5. Navegar a `/Empleado` → funciona (Admin tiene acceso).

### Flujo con usuario Usuario

1. Registrar un nuevo usuario desde `/Account/Register` (por defecto no tiene rol asignado — equivalente a "Usuario").
2. Verificar que el sidebar **NO** muestra ADMINISTRACIÓN.
3. Intentar navegar a `/Empresa` → redirección a `/Account/AccessDenied`.
4. Intentar navegar a `/Empleado` → redirección a `/Account/AccessDenied`.
5. Navegar a `/Servicio`, `/Reporte`, `/Estadistica` → acceso normal.

### Flujo sin autenticar

1. Cerrar sesión → intentar navegar a `/Home` → redirección a `/Account/Login`.
2. Intentar navegar directamente a `/Empresa` → redirección a `/Account/Login?returnUrl=%2FEmpresa`.

---

## 🚀 Cómo hacer tu Commit

```bash
git add .
git commit -m "feat: configurar roles y autorización por rol

- Roles Admin y Usuario con seeding automático al arranque
- Usuario admin creado al primer arranque (admin@sca.com / Admin123)
- [Authorize] en todos los controladores operacionales
- [Authorize(Roles = 'Admin')] en Empresa, Empleado y Admin
- Sidebar adaptativo: ADMINISTRACIÓN visible solo para Admin
- Separador visual entre sección operacional y admin en sidebar"
```
