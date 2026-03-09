# Guía del Commit 31: Envío de Reportes por Email

## 🎯 Propósito

Implementar el servicio de email usando MailKit/MimeKit para permitir que el administrador envíe cualquier reporte generado directamente desde la vista de Reportes a un destinatario de correo. El reporte se genera como PDF (reutilizando la lógica existente) y se adjunta al email con un cuerpo HTML con la identidad visual del sistema.

---

## 📝 Concepto Central

### MailKit vs SmtpClient del framework

.NET incluye `System.Net.Mail.SmtpClient`, pero Microsoft lo marca como obsoleto y no recomendado para nuevas aplicaciones. **MailKit** es la alternativa recomendada:

| Aspecto | System.Net.Mail | MailKit |
|---|---|---|
| Estado | Obsoleto | Activo y recomendado |
| STARTTLS | Soporte parcial | Soporte completo |
| OAuth 2.0 | No | Sí |
| Async real | No | Sí (async/await) |
| Adjuntos MIME | Básico | Completo (MimeKit) |

MailKit incluye **MimeKit** como dependencia, que es la librería para construir mensajes MIME (estructura del email: cuerpo HTML, adjuntos, headers).

### Cómo funciona el envío

```
1. Controller recibe POST (email + params del reporte)
2. GenerarPdfBytesAsync() → byte[]       ← reutiliza lógica existente
3. EmailService.EnviarReporteAsync()     ← construye MimeMessage + adjunto
4. SmtpClient: Connect → Auth → Send    ← comunicación SMTP real
5. Redirect con toast de éxito/error
```

### Patrón de extracción del método PDF

El método `ExportarPDF` fue refactorizado: la lógica de generación se movió a `GenerarPdfBytesAsync()` (privado, retorna `byte[]`). El action `ExportarPDF` ahora es un wrapper de una línea, y el nuevo `EnviarPorEmail` reutiliza el mismo método. Sin duplicación de código.

```csharp
// Antes: un action monolítico de ~350 líneas
public async Task<IActionResult> ExportarPDF(...)
{
    // ... toda la lógica de generación ...
    return File(pdfBytes, "application/pdf", fileName);
}

// Después: separación de responsabilidades
public async Task<IActionResult> ExportarPDF(...)
    => File(await GenerarPdfBytesAsync(...), "application/pdf", fileName); // thin wrapper

public async Task<IActionResult> EnviarPorEmail(...)
{
    var pdf = await GenerarPdfBytesAsync(...); // reutiliza sin duplicar
    await _emailService.EnviarReporteAsync(..., pdf, ...);
}

private async Task<byte[]> GenerarPdfBytesAsync(...) { /* lógica PDF */ }
```

### Configuración SMTP — Gmail con App Password

Para Gmail, no se usa la contraseña normal sino una **App Password** (contraseña de aplicación):
1. Activar verificación en dos pasos en la cuenta Google
2. Ir a Seguridad → Contraseñas de aplicación → Generar
3. Pegar el código de 16 caracteres en `appsettings.json → SmtpPass`

Esto permite que MailKit se autentique sin requerir OAuth, usando STARTTLS en el puerto 587.

---

## 📁 Archivos Creados / Modificados

| Archivo | Acción | Descripción |
|---|---|---|
| `Services/IEmailService.cs` | **Creado** | Interfaz con `EnviarReporteAsync()` |
| `Services/EmailService.cs` | **Creado** | Implementación con MailKit (SMTP + adjunto PDF) |
| `SCA-MVC.csproj` | Modificado | `<PackageReference Include="MailKit" Version="4.15.1" />` |
| `appsettings.json` | Modificado | Sección `EmailSettings` con config SMTP |
| `Program.cs` | Modificado | `AddScoped<IEmailService, EmailService>()` |
| `Controllers/ReporteController.cs` | Modificado | Refactor a `GenerarPdfBytesAsync`, nuevo action `EnviarPorEmail`, inyección `IEmailService` |
| `Views/Reporte/Index.cshtml` | Modificado | Botón "Enviar por Email" + modal con input de email |
| `wwwroot/css/site.css` | Modificado | `.btn-email-rpt`, `.modal-field` y su input |

---

## ⚡ Detalle de Cambios

### 1. `Services/IEmailService.cs`

```csharp
public interface IEmailService
{
    Task EnviarReporteAsync(string destinatario, string asunto, string cuerpoHtml,
                            byte[] adjuntoPdf, string nombreArchivo);
}
```

Un único método — sencillo y focalizado en el caso de uso real del sistema.

---

### 2. `Services/EmailService.cs`

Construye un `MimeMessage` con cuerpo HTML y adjunto PDF usando `BodyBuilder` de MimeKit, luego lo envía via `SmtpClient` de MailKit con STARTTLS en el puerto 587.

```csharp
var builder = new BodyBuilder { HtmlBody = cuerpoHtml };
builder.Attachments.Add(nombreArchivo, adjuntoPdf, ContentType.Parse("application/pdf"));
message.Body = builder.ToMessageBody();

using var client = new SmtpClient();
await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
await client.AuthenticateAsync(user, pass);
await client.SendAsync(message);
```

La configuración se lee desde `IConfiguration` (sección `EmailSettings`) — nunca hardcodeada en el código.

---

### 3. `appsettings.json`

```json
"EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "tu-correo@gmail.com",
    "SmtpPass": "xxxx xxxx xxxx xxxx",
    "FromEmail": "tu-correo@gmail.com",
    "FromName": "Sistema Control de Almuerzos"
}
```

Completar `SmtpUser`, `SmtpPass` y `FromEmail` con las credenciales reales antes de usar.

---

### 4. `Controllers/ReporteController.cs`

Tres cambios:

**a)** Inyección de `IEmailService` en el constructor.

**b)** Refactor: `ExportarPDF` delegó su lógica a `GenerarPdfBytesAsync()` (privado).

**c)** Nuevo action `EnviarPorEmail` (POST, con `[ValidateAntiForgeryToken]`):
- Recibe: `email`, `desde`, `hasta`, `idLugar`, `tipo`
- Genera el PDF llamando a `GenerarPdfBytesAsync`
- Llama a `_emailService.EnviarReporteAsync`
- Redirige con toast de éxito o error

---

### 5. `Views/Reporte/Index.cshtml`

Dos adiciones:

**Botón** en el panel de resultados, al lado de "Exportar PDF":
```html
<button type="button" class="btn-email-rpt" id="btn-enviar-email">
    <i class="bi bi-envelope"></i> Enviar por Email
</button>
```

**Modal** con el mismo estilo glassmorphism que el modal de confirmaciones:
- Ícono azul de email (`bi-envelope-fill`)
- Input `type="email"` con validación HTML5 (`required`)
- Form POST a `EnviarPorEmail` con los parámetros del reporte actual como hidden inputs
- Botones Cancelar y Enviar con los estilos estándar del sistema
- Cierre al hacer click fuera del modal

---

### 6. `wwwroot/css/site.css`

**`.btn-email-rpt`** — botón tono azul (similar al `btn-exportar` en dorado, pero en azul suave) con borde y hover elevado.

**`.modal-field input[type="email"]`** — input dentro del modal con las mismas propiedades que `.emp-field input` (border-radius 12px, fondo semitransparente, focus con `--primary-glow`).

---

## 🛠️ Pasos Realizados

1. `dotnet add package MailKit` → instala MailKit 4.15.1 + MimeKit 4.15.1.
2. Crear `IEmailService.cs` y `EmailService.cs`.
3. Agregar `EmailSettings` en `appsettings.json`.
4. Registrar `builder.Services.AddScoped<IEmailService, EmailService>()` en `Program.cs`.
5. Refactorizar `ReporteController`: extraer PDF a `GenerarPdfBytesAsync`, agregar `IEmailService`, agregar `EnviarPorEmail`.
6. Actualizar `Views/Reporte/Index.cshtml`: botón + modal + section Scripts.
7. Agregar estilos en `site.css`.
8. `dotnet build` → 0 errores, 0 advertencias nuevas.

---

## ✅ Verificación

### Configuración previa
1. Activar verificación en dos pasos en la cuenta Google.
2. Ir a [myaccount.google.com → Seguridad → Contraseñas de aplicación](https://myaccount.google.com/apppasswords).
3. Crear una App Password para "Correo" y copiar el código.
4. Completar `appsettings.json` con el email y la App Password.

### Prueba funcional
1. Ir a `/Reporte`.
2. Configurar un período y tipo de reporte, hacer click en "Generar Reporte".
3. Hacer click en **"Enviar por Email"** → aparece el modal.
4. Ingresar una dirección de email válida → click "Enviar".
5. Verificar que aparece el toast "¡Enviado! Reporte enviado correctamente a...".
6. Revisar la bandeja de entrada del destinatario: debe llegar un email con el PDF adjunto.
7. Probar con email inválido (sin `@`) → el modal no cierra (validación HTML5 `required`).
8. Cerrar el modal haciendo click fuera de él → modal se cierra sin enviar.

---

## 🚀 Cómo hacer tu Commit

```bash
git add .
git commit -m "feat: implementar envío de reportes por email

- IEmailService + EmailService con MailKit/MimeKit (SMTP STARTTLS)
- Configuración SMTP en appsettings.json (EmailSettings)
- GenerarPdfBytesAsync extraído para reutilización (ExportarPDF + EnviarPorEmail)
- Action POST EnviarPorEmail con generación y adjunto de PDF
- Modal de email con estilo glassmorphism en vista de Reportes
- Botón 'Enviar por Email' junto a 'Exportar PDF'
- Estilos: btn-email-rpt, modal-field"
```
