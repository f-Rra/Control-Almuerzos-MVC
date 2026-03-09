# Guía del Commit 29: Gestión de Usuarios del Sistema

## 🎯 Propósito

Implementar el módulo completo de gestión de usuarios: una vista al estilo de Empresas/Empleados (dos columnas — lista a la izquierda, formulario a la derecha) que permite al administrador ver todos los usuarios del sistema, crear nuevos usuarios asignándoles un rol, editar sus datos y rol, resetear contraseñas y activar/desactivar cuentas. El módulo es exclusivo del rol Admin.

---

## 📝 Concepto Central

### ¿Por qué no usar `HasData()` para los usuarios?

Como se vio en el Commit 28, el seeding de usuarios se hace en runtime via `UserManager` y no en migraciones. Esta misma razón explica por qué el CRUD de usuarios en este commit **no usa ni `DbContext` ni EF Core directamente**: toda la gestión pasa por los servicios de Identity (`UserManager<ApplicationUser>` y `RoleManager<IdentityRole>`), que a su vez usan EF internamente pero exponen una API de alto nivel.

### `UserManager<T>` — la API de gestión de usuarios

| Método | Qué hace |
|---|---|
| `CreateAsync(user, password)` | Crea el usuario y hashea la contraseña automáticamente |
| `FindByIdAsync(id)` | Busca por Id de Identity (GUID string) |
| `UpdateAsync(user)` | Persiste cambios en datos del usuario |
| `GetRolesAsync(user)` | Devuelve la lista de roles del usuario |
| `AddToRoleAsync(user, rol)` | Asigna un rol |
| `RemoveFromRolesAsync(user, roles)` | Quita roles |
| `GeneratePasswordResetTokenAsync(user)` | Genera token de reseteo |
| `ResetPasswordAsync(user, token, newPassword)` | Cambia la contraseña con token |
| `SetLockoutEndDateAsync(user, date)` | Desactiva/reactiva la cuenta |

### Desactivar sin eliminar — el patrón `LockoutEnd`

En Identity no conviene eliminar usuarios porque pueden tener datos asociados (registros de auditoría, sesiones, etc.). La práctica estándar es **bloquear la cuenta** usando el campo `LockoutEnd`:

```csharp
// Desactivar: bloquear indefinidamente
await _userManager.SetLockoutEnabledAsync(user, true);
await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

// Reactivar: quitar el bloqueo
await _userManager.SetLockoutEndDateAsync(user, null);
```

Un usuario con `LockoutEnd > DateTimeOffset.UtcNow` no puede iniciar sesión aunque ingrese la contraseña correcta.

### ¿Por qué el botón "Desactivar/Reactivar" es un toggle?

En lugar de tener dos acciones separadas (`Activate` / `Deactivate`), el controlador evalúa el estado actual del usuario y hace la operación inversa. Esto simplifica tanto el controller como la vista: un solo botón que cambia de apariencia, texto e ícono según el estado activo del usuario.

### Edición de contraseña opcional

Al editar un usuario, la contraseña es opcional: **si el campo queda vacío, no se cambia**. Esto se logra de dos maneras complementarias:

1. **Server-side**: `ModelState.Remove(nameof(model.NuevaContrasena))` elimina la validación del campo antes de checkear `ModelState.IsValid`.
2. **En la acción**: el código de reseteo solo se ejecuta `if (!string.IsNullOrWhiteSpace(model.NuevaContrasena))`.

---

## 📁 Archivos Creados / Modificados

| Archivo | Acción | Descripción |
|---|---|---|
| `ViewModels/UsuarioViewModel.cs` | **Creado** | `UsuarioListItem`, `UsuarioFormViewModel`, `UsuarioViewModel` |
| `Controllers/UsuarioController.cs` | **Creado** | Index, Create, Edit, Delete (toggle activar/desactivar) |
| `Views/Usuario/Index.cshtml` | **Creado** | Vista 2 columnas: tabla de usuarios + formulario lateral |
| `Views/Shared/_Layout.cshtml` | Modificado | Enlace USUARIOS en la nav del Admin |
| `wwwroot/css/site.css` | Modificado | Badges de rol (`usr-admin`, `usr-usuario`), `.btn-emp-reactivar`, `.emp-field-row` |

---

## ⚡ Detalle de Cambios

### 1. `ViewModels/UsuarioViewModel.cs`

Tres clases en un mismo archivo por cohesión:

**`UsuarioListItem`**: datos de solo lectura para cada fila de la tabla — incluye `Activo` calculado desde `LockoutEnd`.

**`UsuarioFormViewModel`**: datos del formulario de alta/edición. La propiedad `NuevaContrasena` es `string?` (nullable) para que sea opcional en edición. Tiene data annotations con mensajes de validación.

**`UsuarioViewModel`**: agrupa todo — la lista, el formulario del seleccionado, el filtro activo y los roles disponibles (para el `<select>`).

---

### 2. `Controllers/UsuarioController.cs`

Estructura en 3 regiones siguiendo las convenciones del proyecto:

**`Index` (GET)**: Carga todos los usuarios, aplica filtro de texto, construye la lista de ítems con roles (requiere una llamada async por usuario), determina el seleccionado y arma el ViewModel.

**`Create` (POST)**: Valida que la contraseña no esté vacía, crea el `ApplicationUser`, llama a `CreateAsync()` y asigna el rol seleccionado.

**`Edit` (POST)**: Elimina la validación de contraseña del ModelState, actualiza datos del usuario, reemplaza el rol completamente (quita todos, asigna el nuevo) y resetea contraseña si se ingresó una nueva.

**`DeleteConfirmed` (POST con `[ActionName("Delete")]`)**: Toggle de activación. Evalúa `LockoutEnd` para determinar el estado actual y hace la operación inversa.

**`RebuildAndReturn()`**: Método privado que reconstruye el ViewModel completo para volver al Index cuando hay errores de validación, sin perder los datos del formulario.

---

### 3. `Views/Usuario/Index.cshtml`

Diseño idéntico al de Empleados y Empresas:

- **Columna izquierda** (`emp-list-col`): tabla con columnas Usuario, Nombre Completo, Rol (badge de color) y Estado (badge activo/inactivo). Búsqueda en tiempo real con debounce de 300ms. Botón "Nuevo Usuario" que setea `asp-route-nuevo="true"`.

- **Columna derecha** (`emp-detail-col`): formulario con campos NombreUsuario, Nombre + Apellido (en fila de 2 con `.emp-field-row`), Email, Rol (select), Contraseña. Botones Guardar, Cancelar y Desactivar/Reactivar.

- El formulario POST cambia entre `Create` y `Edit` según si `Model.UsuarioActual.Id` está vacío.

- El form oculto `#delete-usuario-form` maneja la desactivación; el botón confirma con `confirm()` antes de submitear.

---

### 4. `Views/Shared/_Layout.cshtml` — sidebar

Se agregó el enlace USUARIOS al final de la nav del Admin, usando el ícono `bi-shield-lock`:

```razor
<a asp-controller="Usuario" asp-action="Index"
   class="mi @(controller == "Usuario" ? "active" : "")"
   data-tip="Usuarios">
    <div class="bar"></div>
    <div class="ico ico-bi"><i class="bi bi-shield-lock"></i></div>
    <span class="lbl">USUARIOS</span>
</a>
```

---

### 5. `wwwroot/css/site.css` — estilos nuevos

```css
/* Badges de rol en tabla de usuarios */
.usr-badge.usr-admin  → fondo dorado/amarillo (alineado con el color primario del sistema)
.usr-badge.usr-usuario → fondo azul suave

/* Botón reactivar (verde, opuesto al peligro) */
.btn-emp-reactivar

/* Dos campos en fila dentro del formulario (Nombre + Apellido) */
.emp-field-row { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
```

---

## 🛠️ Pasos Realizados

1. **Crear `ViewModels/UsuarioViewModel.cs`** — 3 clases: `UsuarioListItem`, `UsuarioFormViewModel`, `UsuarioViewModel`.

2. **Crear `Controllers/UsuarioController.cs`** — `[Authorize(Roles = "Admin")]`, acciones Index/Create/Edit/DeleteConfirmed, método privado `RebuildAndReturn`.

3. **Crear `Views/Usuario/Index.cshtml`** — layout 2 columnas idéntico al de Empleados/Empresas, con búsqueda en tiempo real, filtro por nombre/email, tabla con badges de rol, formulario con rol dropdown y contraseña opcional, botón toggle desactivar/reactivar.

4. **Actualizar `_Layout.cshtml`** — agregar ítem USUARIOS con `bi-shield-lock` al final de la nav del Admin.

5. **Agregar estilos en `site.css`** — `.usr-badge`, `.usr-admin`, `.usr-usuario`, `.btn-emp-reactivar`, `.emp-field-row`.

6. **Compilar** (`dotnet build`) — 0 errores, 0 advertencias.

---

## ✅ Verificación

1. Iniciar sesión como `admin@sca.com` → ver USUARIOS en el sidebar.
2. Ir a `/Usuario` → tabla con el usuario admin, badge dorado "Admin".
3. Click "Nuevo Usuario" → formulario vacío a la derecha.
4. Crear un nuevo usuario con rol "Usuario" → aparece en la tabla con badge azul.
5. Seleccionar el usuario creado → formulario se completa con sus datos.
6. Editar: cambiar el rol a "Admin" → badge cambia en la tabla.
7. Click "Desactivar" → confirmar → badge "Inactivo", botón cambia a "Reactivar".
8. Click "Reactivar" → badge vuelve a "Activo".
9. Intentar iniciar sesión con el usuario desactivado → Login rechaza con "Cuenta bloqueada temporalmente."
10. Búsqueda en tiempo real: escribir nombre → tabla se filtra con debounce de 300ms.

---

## 🚀 Cómo hacer tu Commit

```bash
git add .
git commit -m "feat: crear gestión de usuarios del sistema

- UsuarioController con Index, Create, Edit, Delete (toggle activar/desactivar)
- ViewModel con UsuarioListItem, UsuarioFormViewModel y UsuarioViewModel
- Vista 2 columnas: tabla de usuarios + formulario lateral (mismo diseño que Empleados)
- Badges de rol (Admin en dorado, Usuario en azul) y estado activo/inactivo
- Reseteo de contraseña opcional en edición
- Desactivación via LockoutEnd sin eliminar el registro
- Enlace USUARIOS con icono bi-shield-lock en sidebar del Admin
- Estilos: usr-badge, btn-emp-reactivar, emp-field-row"
```
