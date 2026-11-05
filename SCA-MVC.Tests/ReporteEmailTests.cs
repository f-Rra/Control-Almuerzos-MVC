using System.Net;

namespace SCA_MVC.Tests;

// Mandar un reporte por email usa la cuenta de correo de la aplicación y acepta cualquier
// destinatario. Por eso es solo del administrador: un usuario común, como el de prueba que
// queda a la vista en el README, no puede mandar correos a nombre del sistema.
//
// Cada test levanta su propia app.
public class ReporteEmailTests
{
    // El token se saca de una pantalla que ya existe, así un test que falla lo hace por lo
    // que se está probando y no por el token
    private static async Task<HttpResponseMessage> EnviarAsync(HttpClient cliente, string destinatario)
    {
        var campos = new Dictionary<string, string>
        {
            ["email"] = destinatario,
            ["desde"] = "2026-01-01",
            ["hasta"] = "2026-01-31",
            ["tipo"] = "cobertura",
            ["__RequestVerificationToken"] = await AppFactory.TokenDeLaPantallaAsync(cliente, "/Servicio")
        };
        return await cliente.PostAsync("/Reporte/EnviarPorEmail", new FormUrlEncodedContent(campos));
    }

    [Fact]
    public async Task UnUsuarioQueNoEsAdmin_NoPuedeEnviarReportesPorEmail()
    {
        using var app = new AppFactory();
        var operador = await app.ClienteDeUsuarioComunAsync();

        var respuesta = await EnviarAsync(operador, "otra-persona@ejemplo.com");

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Contains("AccessDenied", respuesta.Headers.Location!.ToString());
    }

    [Fact]
    public async Task ElAdmin_VeElBotonParaEnviarPorEmail()
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();

        var pantalla = await admin.GetStringAsync("/Reporte");

        Assert.Contains("id=\"btn-enviar-email\"", pantalla);
        Assert.Contains("id=\"email-modal-overlay\"", pantalla);
    }

    [Fact]
    public async Task UnUsuarioQueNoEsAdmin_NoVeElBotonParaEnviarPorEmail()
    {
        using var app = new AppFactory();
        var operador = await app.ClienteDeUsuarioComunAsync();

        var pantalla = await operador.GetStringAsync("/Reporte");

        Assert.DoesNotContain("id=\"btn-enviar-email\"", pantalla);
        Assert.DoesNotContain("id=\"email-modal-overlay\"", pantalla);
    }
}
