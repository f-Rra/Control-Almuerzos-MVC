# Decisiones técnicas

## Dos maneras de autenticarse

Las pantallas usan **ASP.NET Core Identity**: cookie de 8 horas con renovación, contraseñas con PBKDF2 y dos roles, `Admin` (todo) y `Usuario` (solo lo operativo). Los controladores llevan `[Authorize]` y todos los POST, `[ValidateAntiForgeryToken]`.

Los lectores RFID no tienen sesión de navegador, así que la API usa un **segundo esquema de autenticación**, con una clave por lector en la cabecera `X-Api-Key`. Se suma sin cambiar el esquema por defecto, y la API no acepta cookies. La clave se muestra una sola vez; en la base queda su hash SHA-256 y se revoca al instante. El detalle está en [`api-lectores.md`](api-lectores.md).

## Qué impide un registro duplicado

Un empleado no puede almorzar dos veces en el mismo servicio. Se controla en la capa de negocio, para mostrar un mensaje claro, **y también en la base**: un índice único sobre empleado y servicio. Está filtrado para ignorar los nulos, porque los invitados son registros sin empleado y no deben chocar entre sí. Si dos pedidos llegan a la vez, la base rechaza el segundo.

## Un solo reloj con la hora de Argentina

La aplicación le pregunta la hora a un `TimeProvider` (`RelojArgentina`) y no a `DateTime.Now`. Un servidor en la nube corre en UTC, tres horas adelantado: a las 23:30 de Argentina allá ya es el día siguiente y el servicio se guardaría con la fecha equivocada. Con un solo reloj, los tests también pueden simular horarios sin esperarlos.

## Los datos no se borran

- **Baja lógica.** Empresas, empleados, lugares y lectores se desactivan; no se eliminan. Los usuarios se bloquean con `LockoutEnd`.
- **Claves foráneas con `Restrict`.** La base no deja borrar algo que tiene historial. La excepción es el empleado: si se eliminara, sus registros quedan con el empleado en nulo y no se pierden las asistencias.
- **Lo derivado no se guarda.** El estado de un servicio sale de si tiene duración, y el total, de sumar comensales e invitados.

## Claves y configuración

`appsettings.json` no lleva claves. Se cargan con [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) en desarrollo y con variables de entorno en Azure, con `__` en lugar de `:`.

| Qué | Clave | Cuándo |
|---|---|---|
| Administrador | `DefaultAdmin:Password` | Obligatoria la primera vez; el email se cambia con `DefaultAdmin:Email` |
| Cadena de conexión | `ConnectionStrings:DefaultConnection` | Si SQL Server no es `.\SQLEXPRESS` |
| Envío de reportes | `EmailSettings:SmtpUser`, `SmtpPass`, `FromEmail` | Opcional. Gmail pide una contraseña de aplicación |

Si la base no tiene administrador y falta la clave, **la aplicación no arranca** y dice qué comando correr. Una vez creado, la clave no se vuelve a pedir.

## Migraciones

En un servidor real la base se actualiza a propósito, con `dotnet ef database update`. Con Docker Compose la base nace vacía, así que ahí `Database:MigrarAlArrancar` aplica las migraciones al arrancar. Está apagada por defecto.

La tabla de registros tiene un trigger en SQL Server, y EF Core lo sabe (`HasTrigger`): sin eso genera un `INSERT … OUTPUT` que SQL Server rechaza cuando la tabla tiene triggers.

## Tests

83 tests de xUnit en `SCA-MVC.Tests`:

- **Lógica de negocio** sobre una base SQLite en memoria, nueva para cada test. SQLite cumple los índices únicos y las claves foráneas, así que las reglas se prueban contra una base de verdad y no contra una simulación.
- **Integración**: levantan la aplicación entera y le mandan pedidos HTTP. Cubren la protección CSRF, los permisos por rol, la API de lectores y su documentación Swagger.
- **Un reloj falso** para simular horarios, por ejemplo las 23:30 de Argentina cuando en UTC ya es el día siguiente.

El [CI](../.github/workflows/ci.yml) compila y corre todo en cada push y en cada pull request.

## Estructura del proyecto

```
SCA-MVC/
├─ Controllers/   uno por módulo (servicios, registro, reportes, empresas, empleados, estadísticas,
│                 lectores, usuarios, administración, cuenta) y Api/ para los lectores
├─ Services/      la lógica de negocio: interfaces e implementaciones con EF Core, y el email
├─ Models/        las entidades: Empresa, Empleado, Lugar, Servicio, Registro, Lector
├─ Data/          el contexto y una configuración Fluent API por entidad
├─ Api/           la autenticación por clave y la documentación Swagger
├─ Views/         Razor, una carpeta por controlador
├─ ViewModels/    los modelos de cada pantalla
├─ Helpers/       mensajes, errores de Identity en español, claims
├─ Middleware/    el manejo global de excepciones (404 y 500)
├─ Migrations/    y SQL/, con los procedimientos, vistas y triggers
└─ wwwroot/       CSS, JavaScript y Bootstrap 5 (local)
SCA-MVC.Tests/    los tests de xUnit
Guias/            bitácora del desarrollo, una guía por commit
docker-compose.yml   la aplicación y su base, con un comando
```
