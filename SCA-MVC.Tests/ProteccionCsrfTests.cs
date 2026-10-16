using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace SCA_MVC.Tests;

// Los tres endpoints que llama JavaScript (Registrar, FinalizarDirecto y RegistrarMultiples)
// cambian datos, así que tienen que exigir el token antiforgery como los formularios.
//
// Los tests "SinToken" hacen lo que haría una página ajena: mandar el pedido con la
// cookie de sesión del usuario pero sin el token, que esa página no puede conocer.
// Los tests "ConToken" hacen lo que hace el JavaScript de la aplicación: sacar el token
// de la propia pantalla y mandarlo de vuelta.
public class ProteccionCsrfTests : IClassFixture<AppFactory>
{
    private readonly AppFactory _app;

    public ProteccionCsrfTests(AppFactory app) => _app = app;

    // El token que la pantalla deja en <meta name="csrf-token"> para que lo lea el JavaScript
    private static async Task<string> TokenDeLaPantallaAsync(HttpClient cliente, string ruta)
    {
        var html = await cliente.GetStringAsync(ruta);
        var meta = Regex.Match(html, "<meta name=\"csrf-token\" content=\"([^\"]+)\"");
        Assert.True(meta.Success, $"La pantalla {ruta} no trae <meta name=\"csrf-token\">: el JavaScript no tiene de dónde sacar el token");
        return meta.Groups[1].Value;
    }

    private static FormUrlEncodedContent FormularioDeRegistro() => new(new Dictionary<string, string>
    {
        ["credencial"] = "RF001",
        ["idServicio"] = "0",
        ["idLugar"] = "1"
    });

    // ───────────── sin token: tiene que rechazarlo ─────────────

    [Fact]
    public async Task Registrar_SinToken_EsRechazado()
    {
        var cliente = await _app.ClienteConSesionAsync();

        var respuesta = await cliente.PostAsync("/Servicio/Registrar", FormularioDeRegistro());

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task FinalizarDirecto_SinToken_EsRechazado()
    {
        var cliente = await _app.ClienteConSesionAsync();

        var respuesta = await cliente.PostAsync("/Servicio/FinalizarDirecto", content: null);

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task RegistrarMultiples_SinToken_EsRechazado()
    {
        var cliente = await _app.ClienteConSesionAsync();

        var respuesta = await cliente.PostAsJsonAsync("/Registro/RegistrarMultiples",
            new { idServicio = 0, idLugar = 1, idsEmpleados = new[] { 1 } });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    // ───────────── con token: tiene que seguir funcionando ─────────────

    [Fact]
    public async Task Registrar_ConElTokenEnLaCabecera_SeAcepta()
    {
        var cliente = await _app.ClienteConSesionAsync();
        var token = await TokenDeLaPantallaAsync(cliente, "/Servicio");
        var pedido = new HttpRequestMessage(HttpMethod.Post, "/Servicio/Registrar") { Content = FormularioDeRegistro() };
        pedido.Headers.Add("RequestVerificationToken", token);

        var respuesta = await cliente.SendAsync(pedido);

        // El servicio 0 no existe, así que el registro se rechaza, pero por su lógica y no
        // por el token: llegó hasta la acción y devolvió su JSON
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.False((await respuesta.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>()).GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task FinalizarDirecto_ConElTokenEnElCuerpo_SeAcepta()
    {
        // Es lo que hace navigator.sendBeacon al cerrar el navegador: un beacon no puede
        // mandar cabeceras, así que el token va como un campo más del cuerpo
        var cliente = await _app.ClienteConSesionAsync();
        var token = await TokenDeLaPantallaAsync(cliente, "/Servicio");
        var cuerpo = new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token });

        var respuesta = await cliente.PostAsync("/Servicio/FinalizarDirecto", cuerpo);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }

    [Fact]
    public async Task RegistrarMultiples_ConElTokenEnLaCabecera_SeAcepta()
    {
        var cliente = await _app.ClienteConSesionAsync();
        var token = await TokenDeLaPantallaAsync(cliente, "/Registro");
        var pedido = new HttpRequestMessage(HttpMethod.Post, "/Registro/RegistrarMultiples")
        {
            Content = JsonContent.Create(new { idServicio = 0, idLugar = 1, idsEmpleados = Array.Empty<int>() })
        };
        pedido.Headers.Add("RequestVerificationToken", token);

        var respuesta = await cliente.SendAsync(pedido);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }
}
