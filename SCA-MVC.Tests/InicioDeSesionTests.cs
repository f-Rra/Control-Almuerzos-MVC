using System.Net;

namespace SCA_MVC.Tests;

// Levantan la aplicación entera (ver AppFactory) y le hablan por HTTP como un navegador.
public class InicioDeSesionTests : IClassFixture<AppFactory>
{
    private readonly AppFactory _app;

    public InicioDeSesionTests(AppFactory app) => _app = app;

    [Fact]
    public async Task SinSesion_LasPantallasRedirigenAlLogin()
    {
        var cliente = _app.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var respuesta = await cliente.GetAsync("/Servicio");

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Contains("/Account/Login", respuesta.Headers.Location!.ToString());
    }

    [Fact]
    public async Task ConSesion_SeAbreLaPantallaDeServicios()
    {
        var cliente = await _app.ClienteConSesionAsync();

        var respuesta = await cliente.GetAsync("/Servicio");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }
}
