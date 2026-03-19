# Guía del Commit 26: Instalación y Configuración de ASP.NET Identity

## 🎯 Propósito

Integrar el sistema de autenticación y gestión de usuarios de ASP.NET Core Identity al proyecto. Este commit sienta las bases del control de acceso: instala el paquete necesario, extiende la base de datos con las tablas de Identity y configura el pipeline de autenticación, sin romper ninguna funcionalidad existente.

---

## 📝 Concepto Central

### ¿Qué es ASP.NET Identity?

ASP.NET Core Identity es el sistema de membresía incluido en el framework .NET que permite:

- **Autenticación**: verificar quién es el usuario (login con usuario y contraseña)
- **Autorización**: controlar qué puede hacer cada usuario (roles y claims)
- **Gestión de usuarios**: crear, modificar y eliminar cuentas
- **Seguridad**: hashing de contraseñas, lockout automático, tokens de verificación

Identity necesita una base de datos para almacenar su información, por eso se integra con EF Core a través de `IdentityDbContext`.

### Las 7 tablas que crea Identity

| Tabla | Contenido |
|---|---|
| `AspNetUsers` | Usuarios registrados |
| `AspNetRoles` | Roles disponibles (Admin, Empleado, etc.) |
| `AspNetUserRoles` | Asignación usuario ↔ rol |
| `AspNetUserClaims` | Claims adicionales por usuario |
| `AspNetUserLogins` | Proveedores externos (Google, etc.) |
| `AspNetUserTokens` | Tokens de seguridad (reset password, etc.) |
| `AspNetRoleClaims` | Claims asignados a roles |

---

## 📁 Archivos Modificados / Creados

| Archivo | Acción | Descripción |
|---|---|---|
| `SCA-MVC.csproj` | Modificar | Paquete `Identity.EntityFrameworkCore 9.0.0` agregado |
| `Models/ApplicationUser.cs` | **Creado** | Modelo de usuario extendido con Nombre y Apellido |
| `Data/ApplicationDbContext.cs` | Modificar | Hereda de `IdentityDbContext<ApplicationUser>` |
| `Program.cs` | Modificar | Servicios Identity + cookie + middleware |
| `Migrations/20260304221159_AddIdentity.cs` | **Creado** | Migración generada por EF con las 7 tablas |

---

## ⚡ Detalle de Cambios

### 1. Paquete NuGet instalado

```
Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.0
```

Provee la integración entre Identity y Entity Framework Core: `IdentityDbContext`, stores, etc.

---

### 2. `Models/ApplicationUser.cs` — Modelo de usuario

```csharp
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string Nombre { get; set; } = string.Empty;

    [PersonalData]
    public string Apellido { get; set; } = string.Empty;

    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
}
```

**Por qué extender `IdentityUser`?**

`IdentityUser` trae de serie: Id, UserName, Email, PasswordHash, PhoneNumber, etc. Al heredar de él podemos agregar campos propios del negocio (Nombre, Apellido) que quedan en la misma tabla `AspNetUsers`.

El atributo `[PersonalData]` marca los campos como datos personales del usuario, lo que permite exportarlos o eliminarlos si se implementa el GDPR.

---

### 3. `Data/ApplicationDbContext.cs` — Contexto adaptado

**Antes:**
```csharp
public class ApplicationDbContext : DbContext
```

**Después:**
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
```

`IdentityDbContext<TUser>` ya incluye los `DbSet` de las 7 tablas de Identity. También se actualizó `OnModelCreating` para llamar primero a `base.OnModelCreating(modelBuilder)`, lo que permite que Identity registre sus configuraciones antes de las del negocio:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder); // Identity configura sus tablas aquí
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    // Seed data...
}
```

---

### 4. `Program.cs` — Servicios y middleware

**Servicios agregados:**

```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Contraseña
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;

    // Bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Usuario
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
```

**Cookie configurada:**

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});
```

> La cookie expira a las 8 horas de inactividad. Con `SlidingExpiration = true`, cada request reinicia el contador.

**Middleware — orden crítico:**

```csharp
app.UseAuthentication(); // ← ANTES de UseAuthorization
app.UseAuthorization();
```

El orden importa: primero ASP.NET identifica quién es el usuario (Authentication), luego decide qué puede hacer (Authorization).

---

## 🛠️ Pasos a Seguir

1. **Instalar paquete NuGet**
   ```bash
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0
   ```

2. **Crear `Models/ApplicationUser.cs`** con Nombre y Apellido

3. **Actualizar `ApplicationDbContext.cs`**: cambiar herencia a `IdentityDbContext<ApplicationUser>` y ajustar `base.OnModelCreating()`

4. **Actualizar `Program.cs`**: agregar `AddIdentity`, `ConfigureApplicationCookie` y `UseAuthentication()`

5. **Generar migración**
   ```bash
   dotnet ef migrations add AddIdentity
   ```

6. **Aplicar migración a la base de datos**
   ```bash
   dotnet ef database update
   ```
   → Crea las 7 tablas `AspNet*` en `BD_Control_Almuerzos`

---

## ✅ Verificación

Tras aplicar la migración, en SQL Server Management Studio puedes comprobar que existen las nuevas tablas:

```sql
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE 'AspNet%'
ORDER BY TABLE_NAME;
```

Resultado esperado:
```
AspNetRoleClaims
AspNetRoles
AspNetUserClaims
AspNetUserLogins
AspNetUserRoles
AspNetUserTokens
AspNetUsers
```

La aplicación sigue funcionando exactamente igual que antes: Identity está instalado pero aún no protege ninguna ruta (eso viene en los commits 27–29).

---

## 🚀 Cómo hacer tu Commit

```bash
git add .
git commit -m "feat: instalar y configurar ASP.NET Identity

- Paquete Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.0
- Modelo ApplicationUser con Nombre y Apellido (extiende IdentityUser)
- ApplicationDbContext hereda de IdentityDbContext<ApplicationUser>
- Configuracion de Identity: clave>=6 digitos, lockout 5min/5 intentos
- Cookie de autenticacion configurada (8h, sliding, rutas Login/AccessDenied)
- UseAuthentication() agregado al pipeline antes de UseAuthorization()
- Migracion AddIdentity aplicada (7 tablas AspNet* en BD)"
```
