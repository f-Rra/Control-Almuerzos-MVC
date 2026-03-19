# Guía del Commit 27: Vistas de Login, Logout y Registro

## 🎯 Propósito

Implementar el flujo completo de autenticación de usuarios: pantallas de login y registro con estilo glassmorphism, acción de logout, vista de acceso denegado, y actualización del topbar del layout para mostrar el nombre real del usuario autenticado.

---

## 📝 Concepto Central

### El flujo de autenticación en ASP.NET Identity

Con Identity instalado (Commit 26), el sistema ya sabe cómo gestionar usuarios internamente. Este commit cierra el circuito con la UI: el usuario llega a `/Account/Login`, ingresa sus credenciales, Identity las valida contra la BD y emite una **cookie de sesión**. A partir de ese momento, cada request lleva esa cookie y el middleware de autenticación sabe quién es el usuario.

```
[Browser] → GET /Account/Login → [LoginView]
[Browser] → POST /Account/Login → [AccountController]
                                      ↓ PasswordSignInAsync()
                                      ↓ (éxito) → cookie emitida
                                      ↓ → Redirect → /Home/Index
```

### `SignInManager<T>` vs `UserManager<T>`

| Clase | Responsabilidad |
|---|---|
| `UserManager<ApplicationUser>` | Gestión de usuarios: crear, buscar, cambiar contraseña, asignar roles |
| `SignInManager<ApplicationUser>` | Gestión de sesión: login, logout, verificar si está autenticado, lockout |

### `returnUrl` — redirección inteligente

Cuando el middleware de Authorization intercepta una request sin autenticar, redirige a `/Account/Login?returnUrl=/ruta/original`. El controller captura ese parámetro y, tras el login exitoso, redirige al destino original en lugar de siempre ir al Home.

**Precaución de seguridad**: siempre se valida con `Url.IsLocalUrl(returnUrl)` para evitar **open redirect attacks** (que un atacante envíe `returnUrl=https://malicious.com`).

---

## 📁 Archivos Creados / Modificados

| Archivo | Acción | Descripción |
|---|---|---|
| `Controllers/AccountController.cs` | **Creado** | Login, Logout, Register, AccessDenied |
| `ViewModels/AccountViewModels.cs` | **Creado** | `LoginViewModel` y `RegisterViewModel` |
| `Views/Account/Login.cshtml` | **Creado** | Vista de login con layout propio |
| `Views/Account/Register.cshtml` | **Creado** | Vista de registro con layout propio |
| `Views/Account/AccessDenied.cshtml` | **Creado** | Vista de acceso denegado |
| `Views/Shared/_Layout.cshtml` | Modificar | Topbar con usuario real y botón logout |
| `wwwroot/css/site.css` | Modificar | Estilos `auth-*` para las vistas de autenticación |

---

## ⚡ Detalle de Cambios

### 1. `AccountController.cs`

Organizado en 3 regiones según las convenciones del proyecto:

**`Login` (GET):** Si el usuario ya está autenticado, redirige al Home directamente. Si no, muestra el formulario.

**`Login` (POST):** Llama a `PasswordSignInAsync()` con `lockoutOnFailure: true` — si el usuario ingresa 5 contraseñas incorrectas, Identity bloquea la cuenta 5 minutos automáticamente.

```csharp
var result = await _signInManager.PasswordSignInAsync(
    model.Email,
    model.Password,
    model.Recordarme,
    lockoutOnFailure: true);
```

**`Logout` (POST):** Solo accesible vía POST con token anti-CSRF para evitar logout CSRF. Llama a `SignOutAsync()` que invalida la cookie.

**`Register` (POST):** Crea el `ApplicationUser` con `UserManager.CreateAsync()`, que hashea la contraseña automáticamente. Si la creación fue exitosa, hace login inmediato.

**`RedirectToLocal()`:** Método privado de validación de `returnUrl` contra open redirect.

---

### 2. `ViewModels/AccountViewModels.cs`

**`LoginViewModel`**: Email + Password + Recordarme (bool para cookie persistente)

**`RegisterViewModel`**: Nombre + Apellido + Email + Password + ConfirmPassword
- `[Compare(nameof(Password))]` valida que ambas contraseñas coincidan en client-side y server-side
- `[StringLength(100, MinimumLength = 6)]` alinea la validación con la config de Identity

---

### 3. Vistas de autenticación — diseño

Las vistas de Login, Register y AccessDenied usan `Layout = null` — tienen su propio HTML completo. Esto es intencional: las páginas de autenticación no deben mostrar el sidebar.

El diseño mantiene el estilo glassmorphism del sistema:
- Fondo con orbs difuminados (`shader > orb`)
- Card central con `backdrop-filter: blur(20px)` y borde dorado
- Inputs con focus dorado y sombra
- Botón primario con gradiente amarillo

---

### 4. `_Layout.cshtml` — topbar dinámica

**Antes:** `Administrador` hardcodeado con un `<a href="#">` sin funcionalidad.

**Después:**

```razor
@if (User.Identity?.IsAuthenticated == true)
{
    var iniciales = /* toma primera letra de cada palabra del nombre */
    <div class="pill">
        <div class="avatar">@iniciales</div>
        <span class="uname">@User.Identity.Name</span>
    </div>
    <form asp-controller="Account" asp-action="Logout" method="post">
        @Html.AntiForgeryToken()
        <button type="submit" class="logout">...</button>
    </form>
}
else
{
    /* link a Login */
}
```

> **Por qué el logout es un `<form>` POST y no un `<a>` GET?**
> Para protegerse de CSRF. Si fuera un link GET, cualquier página maliciosa podría cargar `<img src="/Account/Logout">` y cerrar la sesión del usuario sin su consentimiento.

---

## 🛠️ Pasos a Seguir

1. **Crear `AccountController.cs`** con Login (GET/POST), Logout (POST), Register (GET/POST), AccessDenied (GET)

2. **Crear `ViewModels/AccountViewModels.cs`** con `LoginViewModel` y `RegisterViewModel`

3. **Crear `Views/Account/Login.cshtml`** — formulario con toggle de contraseña, Layout = null

4. **Crear `Views/Account/Register.cshtml`** — formulario con campos nombre/apellido, Layout = null

5. **Crear `Views/Account/AccessDenied.cshtml`** — pantalla de acceso denegado con botones de volver e ir al login

6. **Actualizar `Views/Shared/_Layout.cshtml`** — topbar condicional con usuario real e iniciales, logout vía form POST

7. **Agregar estilos en `wwwroot/css/site.css`** — sección `auth-*` con layout centrado, card glassmorphism, inputs y botones estilizados

---

## ✅ Verificación

1. Navegar a cualquier URL de la app → debe redirigir a `/Account/Login` (cuando se agregue `[Authorize]` en commits siguientes)
2. Por ahora: ir directamente a `/Account/Login` → ver la vista de login con estilo glassmorphism
3. Ir a `/Account/Register` → ver el formulario de registro de dos columnas
4. Registrar un usuario → login automático → topbar muestra el nombre real con iniciales
5. Hacer logout → regresa a `/Account/Login`
6. Verificar topbar: muestra iniciales del nombre del usuario (ej: "JP" para Juan Pérez)

---

## 🚀 Cómo hacer tu Commit

```bash
git add .
git commit -m "feat: crear vistas de autenticación (Login, Logout, Registro)

- AccountController con Login/Logout/Register/AccessDenied
- ViewModels con validaciones, campo NombreUsuario
- Vistas Login, Register y AccessDenied con layout propio y glassmorphism
- Topbar muestra NombreUsuario desde claims, logout via form POST
- SpanishIdentityErrorDescriber y AppUserClaimsPrincipalFactory
- Migracion AddNombreUsuario aplicada
- Login card rediseñada: logo flotante sobre card (diseño 3)"
```
