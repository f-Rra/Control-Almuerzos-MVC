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

## Principios de comportamiento (Karpathy)

Guías para reducir errores comunes en codificación con IA. Derivadas de las observaciones de Andrej Karpathy sobre fallas típicas de LLMs en tareas de código.

**Tradeoff:** estas guías priorizan cautela sobre velocidad. Para tareas triviales, usar criterio.

### 1. Pensar antes de codificar

No asumir. No ocultar confusión. Exponer los tradeoffs.

Antes de implementar:
- Enunciar supuestos explícitamente. Si hay incertidumbre, preguntar.
- Si existen múltiples interpretaciones, presentarlas — no elegir en silencio.
- Si existe un enfoque más simple, decirlo. Hacer pushback cuando corresponde.
- Si algo no está claro, detenerse. Nombrar qué confunde. Preguntar.

### 2. Simplicidad primero

Mínimo código que resuelve el problema. Nada especulativo.

- Sin features más allá de lo pedido.
- Sin abstracciones para código de uso único.
- Sin "flexibilidad" o "configurabilidad" que no fue solicitada.
- Sin manejo de errores para escenarios imposibles.
- Si se escriben 200 líneas y podrían ser 50, reescribir.

### 3. Cambios quirúrgicos

Tocar solo lo necesario. Limpiar solo el propio desorden.

Al editar código existente:
- No "mejorar" código adyacente, comentarios ni formato.
- No refactorizar cosas que no están rotas.
- Respetar el estilo existente, aunque se haría diferente.
- Si se detecta código muerto no relacionado, mencionarlo — no eliminarlo.

Al crear cambios que dejan huérfanos:
- Eliminar imports/variables/funciones que LOS PROPIOS cambios dejaron sin uso.
- No eliminar código muerto preexistente salvo que se pida explícitamente.

### 4. Ejecución orientada a metas

Definir criterios de éxito. Iterar hasta verificar.

Transformar tareas en metas verificables:
- "Agregar validación" → "escribir tests para inputs inválidos, luego hacerlos pasar"
- "Corregir el bug" → "escribir un test que lo reproduzca, luego hacerlo pasar"

Para tareas de múltiples pasos, enunciar un plan breve:
```
1. [Paso] → verificar: [check]
2. [Paso] → verificar: [check]
3. [Paso] → verificar: [check]
```

## Plugins y skills instalados

### Plugin: claude-code-setup (Anthropic Verified)

Analiza el proyecto y recomienda automaciones de Claude Code adaptadas al stack: MCPs, skills, hooks, subagents y slash commands. Opera en modo solo-lectura.

Para ejecutarlo: escribir "recommend automations for this project" o "what hooks should I use?" en el chat. Instalación global: `/plugin install claude-code-setup`.

### Skills de proyecto (`.claude/commands/`)

| Comando | Origen | Propósito |
|---------|--------|-----------|
| `/frontend-design` | Propio | Auditoría y mejora de vistas Razor con Bootstrap 5 |
| `/db-review` | Propio | Análisis de queries EF Core, relaciones e índices |
| `/report-generator` | Propio | Guía para reportes PDF con QuestPDF |
| `/seed-data` | Propio | Generación y actualización de datos de prueba |
| `/migrate` | dotnet-claude-kit | Workflow seguro para migraciones EF Core |
| `/verify` | dotnet-claude-kit | Pipeline de 7 fases antes de hacer PR |
| `/security-scan` | dotnet-claude-kit | Audit OWASP completo en 6 dimensiones |
| `/build-fix` | dotnet-claude-kit | Loop autónomo para resolver errores de compilación |
| `/ef-core` | dotnet-claude-kit | Referencia de patrones modernos de EF Core |
