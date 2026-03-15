using System.Net;

namespace SCA_MVC.Middleware
{
    /// <summary>
    /// Middleware global de manejo de excepciones.
    /// Captura cualquier excepción no controlada que ocurra en el pipeline de ASP.NET Core
    /// y redirige al usuario a la página de error correspondiente, en lugar de devolver
    /// un stack trace crudo o un error 500 sin formato.
    ///
    /// Registro en Program.cs:
    ///   app.UseMiddleware<ExceptionMiddleware>();
    ///
    /// Flujo:
    ///   Solicitud → [ExceptionMiddleware] → siguiente middleware → respuesta
    ///   Si hay excepción → catch → log → redirigir a /Home/Error
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next   = next;
            _logger = logger;
            _env    = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pasar la solicitud al siguiente middleware de la cadena
                await _next(context);

                // ── Manejo de errores HTTP (404, 403, etc.) ──────────────────
                // Si la respuesta ya fue iniciada, no se puede redirigir
                if (!context.Response.HasStarted)
                {
                    if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                    {
                        context.Response.Redirect("/Home/NotFound");
                    }
                    else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                    {
                        context.Response.Redirect("/Account/AccessDenied");
                    }
                }
            }
            catch (Exception ex)
            {
                // ── Excepción no controlada ───────────────────────────────────
                // Registrar siempre en el log del servidor
                _logger.LogError(ex,
                    "Excepción no controlada en {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                // No redirigir si la respuesta ya empezó a escribirse
                if (context.Response.HasStarted)
                {
                    throw;
                }

                // Redirigir a la página de error genérica
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
