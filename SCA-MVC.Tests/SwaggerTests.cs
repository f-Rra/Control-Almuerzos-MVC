using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SCA_MVC.Tests;

// Swagger (OpenAPI) es la documentación de la API que genera la propia aplicación: qué rutas hay,
// qué se manda, qué responde y cómo se autentica. Estos tests piden esa documentación y
// comprueban que describa lo que la API hace de verdad, así no queda desactualizada.
public class SwaggerTests : IClassFixture<AppFactory>
{
    private readonly AppFactory _app;

    public SwaggerTests(AppFactory app) => _app = app;

    private async Task<JsonElement> DocumentoAsync()
    {
        var respuesta = await _app.CreateClient().GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        return await respuesta.Content.ReadFromJsonAsync<JsonElement>();
    }

    [Fact]
    public async Task ElDocumento_DescribeElEndpointDeRegistros()
    {
        var documento = await DocumentoAsync();

        var registrar = documento.GetProperty("paths").GetProperty("/api/registros").GetProperty("post");
        var respuestas = registrar.GetProperty("responses");
        foreach (var codigo in new[] { "201", "400", "401", "404", "409", "422" })
            Assert.True(respuestas.TryGetProperty(codigo, out _), $"El documento no describe la respuesta {codigo}");
    }

    [Fact]
    public async Task ElDocumento_ExplicaCadaRespuestaConElTextoDelCodigo()
    {
        var documento = await DocumentoAsync();

        var respuestas = documento.GetProperty("paths").GetProperty("/api/registros").GetProperty("post").GetProperty("responses");

        Assert.Contains("ya está registrada", respuestas.GetProperty("409").GetProperty("description").GetString());
    }

    [Fact]
    public async Task ElDocumento_PideLaClaveDelLectorEnLaCabecera()
    {
        var documento = await DocumentoAsync();

        var esquema = documento.GetProperty("components").GetProperty("securitySchemes").GetProperty("ApiKey");
        Assert.Equal("apiKey", esquema.GetProperty("type").GetString());
        Assert.Equal("header", esquema.GetProperty("in").GetString());
        Assert.Equal("X-Api-Key", esquema.GetProperty("name").GetString());

        // y el documento declara que toda la API la necesita (Swashbuckle lo pone una sola vez,
        // arriba de todo, y vale para cada endpoint), para que Swagger UI la mande al probar
        Assert.True(documento.TryGetProperty("security", out var seguridad));
        Assert.Contains("ApiKey", seguridad.GetRawText());
    }

    [Fact]
    public async Task ElDocumento_NoMuestraLasPantallasDeLaAplicacion()
    {
        var documento = await DocumentoAsync();

        // Solo la API: las rutas de las pantallas (Servicio, Empresa, Login...) no van acá
        var rutas = documento.GetProperty("paths").EnumerateObject().Select(p => p.Name).ToList();
        Assert.NotEmpty(rutas);
        Assert.All(rutas, ruta => Assert.StartsWith("/api/", ruta));
    }

    [Fact]
    public async Task LaPaginaDeSwagger_SeAbre()
    {
        var respuesta = await _app.CreateClient().GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("API de lectores", await respuesta.Content.ReadAsStringAsync());
    }
}
