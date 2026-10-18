using System.Net;

namespace SCA_MVC.Tests;

// En la aplicación hay un solo servicio activo a la vez: la pantalla de servicios, el
// registro manual, el lector y la finalización al cerrar el navegador trabajan siempre
// sobre "el" servicio activo. Por eso iniciar uno nuevo se rechaza si ya hay otro, esté
// en el mismo lugar o en el otro.
//
// Cada test levanta su propia app: lo que uno inicia no lo ve el siguiente.
// El lugar 1 es el Comedor y el 2 el Quincho.
public class IniciarServicioTests
{
    private static async Task<HttpResponseMessage> IniciarAsync(HttpClient cliente, int idLugar)
    {
        var token = await AppFactory.TokenDeLaPantallaAsync(cliente, "/Servicio");
        return await cliente.PostAsync("/Servicio/Iniciar", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["idLugar"] = idLugar.ToString(),
            ["proyeccion"] = "50",
            ["invitados"] = "0",
            ["__RequestVerificationToken"] = token
        }));
    }

    [Fact]
    public async Task Iniciar_SinServicioActivo_LoCrea()
    {
        using var app = new AppFactory();
        var cliente = await app.ClienteConSesionAsync();

        var respuesta = await IniciarAsync(cliente, idLugar: 1);

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Equal(1, app.ServiciosActivos());
    }

    [Fact]
    public async Task Iniciar_ConUnServicioActivoEnOtroLugar_NoCreaOtro()
    {
        using var app = new AppFactory();
        var cliente = await app.ClienteConSesionAsync();
        await IniciarAsync(cliente, idLugar: 1);   // Comedor

        var respuesta = await IniciarAsync(cliente, idLugar: 2);   // Quincho

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Equal(1, app.ServiciosActivos());

        // el motivo le llega al usuario en la pantalla a la que lo redirige
        var pantalla = await cliente.GetStringAsync("/Servicio");
        Assert.Contains("Ya hay un servicio activo en Comedor", pantalla);
    }

    [Fact]
    public async Task Iniciar_ConUnServicioActivoEnElMismoLugar_NoCreaOtro()
    {
        using var app = new AppFactory();
        var cliente = await app.ClienteConSesionAsync();
        await IniciarAsync(cliente, idLugar: 1);

        await IniciarAsync(cliente, idLugar: 1);

        Assert.Equal(1, app.ServiciosActivos());
    }
}
