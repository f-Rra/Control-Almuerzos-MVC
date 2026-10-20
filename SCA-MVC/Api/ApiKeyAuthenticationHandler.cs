using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SCA_MVC.Services;

namespace SCA_MVC.Api
{
    // Identifica a un lector RFID por la clave que manda en la cabecera X-Api-Key.
    //
    // Es un "esquema de autenticación" más, al lado del de la cookie de los usuarios que
    // entran con el navegador. Cada endpoint elige cuál acepta: los de la API aceptan solo
    // este, así que una cookie de sesión no sirve para llamarlos (ni al revés).
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Esquema = "ApiKey";
        public const string Cabecera = "X-Api-Key";

        // El lugar donde está instalado el lector, que el endpoint lee de acá
        public const string ClaimLugar = "idLugar";

        private readonly ILectorNegocio _lectores;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> opciones,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ILectorNegocio lectores)
            : base(opciones, logger, encoder)
        {
            _lectores = lectores;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Sin cabecera no hay nada que validar: el endpoint decide qué hacer (responde 401)
            if (!Request.Headers.TryGetValue(Cabecera, out var valores))
                return AuthenticateResult.NoResult();

            var lector = await _lectores.AutenticarAsync(valores.ToString());
            if (lector == null)
                return AuthenticateResult.Fail("Clave inválida o revocada.");

            var identidad = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, lector.IdLector.ToString()),
                    new Claim(ClaimTypes.Name, lector.Nombre),
                    new Claim(ClaimLugar, lector.IdLugar.ToString())
                ],
                Scheme.Name);

            return AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(identidad), Scheme.Name));
        }

        // Lo que se responde cuando falta la clave o no sirve: un 401 con el formato estándar de
        // errores de las APIs (ProblemDetails), igual que el resto de las respuestas de error
        protected override async Task HandleChallengeAsync(AuthenticationProperties propiedades)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers.WWWAuthenticate = Esquema;

            await Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "No autorizado",
                    Detail = $"Falta la cabecera {Cabecera} o la clave no es válida."
                },
                options: null,
                contentType: "application/problem+json");
        }
    }
}
