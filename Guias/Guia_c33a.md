# Guía Commit 33a — Manejo Global de Errores

## ¿Qué se implementó?

En este commit se agregó un sistema de manejo de errores consistente con el diseño del resto de la aplicación. Antes, cualquier error inesperado devolvía la página de error genérica de ASP.NET (texto plano o página del navegador), y los errores 404 no tenían página personalizada. Ahora todo error tiene una respuesta visual acorde al sistema.

---

## Archivos creados y modificados

| Archivo | Acción | Descripción |
|---|---|---|
| `Middleware/ExceptionMiddleware.cs` | Creado | Intercepta excepciones y errores HTTP en toda la app |
| `Views/Shared/Error.cshtml` | Reemplazado | Vista 500 con diseño del sistema |
| `Views/Home/NotFound.cshtml` | Creado | Vista 404 con diseño del sistema |
| `Controllers/HomeController.cs` | Modificado | Nueva acción `NotFound()` + `[AllowAnonymous]` en errores |
| `Program.cs` | Modificado | Registro de `ExceptionMiddleware` en el pipeline |

---

## Conceptos implementados

### 1. Middleware personalizado

Un **middleware** es una clase que se inserta en el **pipeline de solicitudes HTTP** de ASP.NET Core. Cada solicitud pasa por todos los middlewares registrados, en orden, antes de llegar al controlador. El middleware de excepciones se registra **primero** para que pueda envolver toda la cadena y atrapar cualquier error que ocurra en los eslabones siguientes.

```
Solicitud → [ExceptionMiddleware] → [Auth] → [Routing] → [Controlador] → Respuesta
                ↑
          Si algo falla acá abajo,
          ExceptionMiddleware lo atrapa
```

La clase `ExceptionMiddleware` implementa el patrón estándar de ASP.NET Core:
- Recibe un `RequestDelegate _next` (el siguiente middleware en la cadena)
- En `InvokeAsync(HttpContext context)` llama a `await _next(context)` dentro de un `try/catch`
- Si ocurre una excepción, la loguea con `ILogger` y redirige a `/Home/Error`
- Si la respuesta ya comenzó a escribirse (`HasStarted`), no puede redirigir: relanza la excepción (`throw`)

El middleware también intercepta **errores HTTP sin excepción** (como un 404 de una ruta que no existe): después de llamar a `_next`, revisa `context.Response.StatusCode` y redirige si es 404 o 403.

### 2. Registro en Program.cs

```csharp
// Se registra ANTES de todo para envolver el pipeline completo
app.UseMiddleware<ExceptionMiddleware>();
```

Se reemplazó el `app.UseExceptionHandler("/Home/Error")` del scaffold, que solo funcionaba en producción, por nuestro middleware que funciona siempre y además maneja 404.

### 3. Acción NotFound en HomeController

```csharp
[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public IActionResult NotFound()
{
    Response.StatusCode = 404;
    return View();
}
```

`[AllowAnonymous]` es necesario porque el middleware puede interceptar una solicitud **antes de que el usuario esté autenticado**, por ejemplo si alguien tipea una URL inválida en la pantalla de login. Sin `[AllowAnonymous]`, la acción de error redirige al login, que redirige al error, creando un loop infinito.

`ResponseCache(..., NoStore = true)` evita que el navegador cachee la página de error, para que se muestre siempre la versión actualizada.

### 4. Vistas de error standalone (sin _Layout)

Ambas vistas (`Error.cshtml` y `NotFound.cshtml`) usan `Layout = null` y tienen todo el HTML embebido. Esto es intencional: si el error ocurre en el layout mismo (por ejemplo, al cargar el menú), una vista que depende del layout volvería a fallar. Las vistas standalone replican el diseño glassmorphism del sistema con CSS inline.

---

## Flujo completo según el tipo de error

| Situación | Qué pasa |
|---|---|
| Usuario tipea una URL que no existe (`/algo/que/no-existe`) | Middleware detecta status 404 → redirige a `/Home/NotFound` |
| Un controlador lanza una excepción no controlada | Middleware la atrapa, la loguea, redirige a `/Home/Error` |
| Usuario intenta acceder a una página sin permiso | Middleware detecta 403 → redirige a `/Account/AccessDenied` |
| Error en el propio middleware | Como `HasStarted = false`, relanza la excepción (ASP.NET Core la maneja) |

---

## Mensaje de commit

```
feat: agregar middleware de excepciones y páginas de error personalizadas

- ExceptionMiddleware global para captura de errores no controlados y HTTP 404/403
- Redirección automática: 404 → /Home/NotFound, 500 → /Home/Error, 403 → /Account/AccessDenied
- ILogger integrado para registro de todas las excepciones no controladas
- Vista Error.cshtml rediseñada con glassmorphism (reemplaza scaffold genérico)
- Vista NotFound.cshtml nueva con diseño consistente y botones "Ir al inicio" / "Volver"
- [AllowAnonymous] en acciones de error para evitar loops de redirección
- Reemplaza UseExceptionHandler del scaffold por middleware propio
```
