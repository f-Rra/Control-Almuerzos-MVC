using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SCA_MVC.Data;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

// La API que usa un lector RFID de red: POST /api/registros con la clave del lector en la
// cabecera X-Api-Key. No hay ningún lector físico para probarla, así que estos tests lo
// simulan: son un cliente HTTP que manda lo que mandaría el lector.
//
// Cada test levanta su propia app: lo que uno registra o inicia no lo ve el siguiente.
// Lugar 1 = Comedor, lugar 2 = Quincho. El empleado 1 (credencial RF001) es de la empresa 1.
public class ApiRegistrosTests
{
    private const string Ruta = "/api/registros";

    // Un lector nuevo, y un cliente HTTP que ya manda su clave
    private static async Task<(HttpClient Cliente, int IdLector)> ClienteDeLectorAsync(AppFactory app, int idLugar = 1)
    {
        using var alcance = app.Services.CreateScope();
        var (lector, clave) = await alcance.ServiceProvider.GetRequiredService<ILectorNegocio>()
            .CrearAsync("Lector de prueba", idLugar);

        var cliente = app.CreateClient();
        cliente.DefaultRequestHeaders.Add("X-Api-Key", clave);
        return (cliente, lector.IdLector);
    }

    private static async Task<int> IniciarServicioAsync(AppFactory app, int idLugar = 1)
    {
        using var alcance = app.Services.CreateScope();
        return await alcance.ServiceProvider.GetRequiredService<IServicioNegocio>()
            .CrearServicioAsync(idLugar, proyeccion: 50);
    }

    // Los registros que crean estos tests. La base trae miles de registros históricos de
    // ejemplo, pero todos de servicios ya finalizados: los nuevos son los de servicios activos.
    private static async Task<int> ContarRegistrosAsync(AppFactory app)
    {
        using var alcance = app.Services.CreateScope();
        return await alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>()
            .Registros.CountAsync(r => r.Servicio!.DuracionMinutos == null);
    }

    private static Task<HttpResponseMessage> LeerAsync(HttpClient cliente, string credencial) =>
        cliente.PostAsJsonAsync(Ruta, new { credencial });

    // ───────────── el camino feliz ─────────────

    [Fact]
    public async Task ConClaveValida_RegistraYResponde201ConLosDatosDelComensal()
    {
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app);
        var idServicio = await IniciarServicioAsync(app);

        var respuesta = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(idServicio, json.GetProperty("idServicio").GetInt32());
        Assert.Equal("Comedor", json.GetProperty("lugar").GetString());
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("empleado").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("empresa").GetString()));
        Assert.Matches(@"^\d{2}:\d{2}$", json.GetProperty("hora").GetString()!);

        // quedó guardado de verdad, en el lugar del lector
        using var alcance = app.Services.CreateScope();
        var registro = await alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>()
            .Registros.SingleAsync(r => r.IdServicio == idServicio);
        Assert.Equal(1, registro.IdEmpleado);
        Assert.Equal(1, registro.IdLugar);
    }

    [Fact]
    public async Task UnLectorDelQuincho_RegistraEnElServicioDelQuincho()
    {
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app, idLugar: 2);
        await IniciarServicioAsync(app, idLugar: 1);                    // el del Comedor, que no es el suyo
        var delQuincho = await IniciarServicioAsync(app, idLugar: 2);

        var respuesta = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        using var alcance = app.Services.CreateScope();
        var registro = await alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>()
            .Registros.SingleAsync(r => r.IdEmpleado == 1 && r.IdServicio == delQuincho);
        Assert.Equal(2, registro.IdLugar);
    }

    // ───────────── 401: quién sos ─────────────

    [Fact]
    public async Task SinClave_Responde401()
    {
        using var app = new AppFactory();
        await IniciarServicioAsync(app);

        var respuesta = await LeerAsync(app.CreateClient(), "RF001");

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
        Assert.Equal("application/problem+json", respuesta.Content.Headers.ContentType?.MediaType);
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }

    [Fact]
    public async Task ConUnaClaveQueNoExiste_Responde401()
    {
        using var app = new AppFactory();
        await IniciarServicioAsync(app);
        var cliente = app.CreateClient();
        cliente.DefaultRequestHeaders.Add("X-Api-Key", "sca_una-clave-inventada");

        var respuesta = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }

    [Fact]
    public async Task ConUnaClaveRevocada_Responde401()
    {
        using var app = new AppFactory();
        var (cliente, idLector) = await ClienteDeLectorAsync(app);
        await IniciarServicioAsync(app);
        using (var alcance = app.Services.CreateScope())
            await alcance.ServiceProvider.GetRequiredService<ILectorNegocio>().RevocarAsync(idLector);

        var respuesta = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }

    [Fact]
    public async Task ConLaSesionDeUnNavegadorPeroSinClave_Responde401()
    {
        // La API no acepta la cookie de un usuario logueado: es solo para lectores con clave
        using var app = new AppFactory();
        var navegador = await app.ClienteConSesionAsync();
        await IniciarServicioAsync(app);

        var respuesta = await LeerAsync(navegador, "RF001");

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }

    // ───────────── 404 y 422: qué pasó con la lectura ─────────────

    [Fact]
    public async Task ConUnaCredencialQueNoExiste_Responde404()
    {
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app);
        await IniciarServicioAsync(app);

        var respuesta = await LeerAsync(cliente, "NO-EXISTE");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
        // el 404 es de esta credencial y no de una ruta inexistente: trae su explicación
        var problema = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Credencial desconocida", problema.GetProperty("title").GetString());
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }

    [Fact]
    public async Task ConUnEmpleadoInactivo_Responde422()
    {
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app);
        await IniciarServicioAsync(app);
        using (var alcance = app.Services.CreateScope())
        {
            var contexto = alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            (await contexto.Empleados.FindAsync(1))!.Estado = false;
            await contexto.SaveChangesAsync();
        }

        var respuesta = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
        var problema = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Empleado inactivo", problema.GetProperty("title").GetString());
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }

    [Fact]
    public async Task SinServicioActivoEnSuLugar_Responde422()
    {
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app, idLugar: 1);
        await IniciarServicioAsync(app, idLugar: 2);   // hay uno, pero en el otro lugar

        var respuesta = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
        var problema = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("No hay servicio activo", problema.GetProperty("title").GetString());
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }

    [Fact]
    public async Task ConUnServicioQueQuedoAbiertoDeAyer_Responde422()
    {
        // Es lo mismo que hace la pantalla web: un servicio de otro día no cuenta como el de hoy
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app);
        using (var alcance = app.Services.CreateScope())
        {
            var contexto = alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            contexto.Servicios.Add(new SCA_MVC.Models.Servicio { IdLugar = 1, Fecha = DateTime.Today.AddDays(-2) });
            await contexto.SaveChangesAsync();
        }

        var respuesta = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }

    // ───────────── 409: ya está ─────────────

    [Fact]
    public async Task PasarLaCredencialDosVeces_LaSegundaResponde409YNoDuplica()
    {
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app);
        await IniciarServicioAsync(app);

        var primera = await LeerAsync(cliente, "RF001");
        var segunda = await LeerAsync(cliente, "RF001");

        Assert.Equal(HttpStatusCode.Created, primera.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
        var problema = await segunda.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Ya registrado", problema.GetProperty("title").GetString());
        Assert.Equal(1, await ContarRegistrosAsync(app));
    }

    // ───────────── 400: el pedido está mal armado ─────────────

    [Theory]
    [InlineData("{}")]
    [InlineData("""{"credencial": ""}""")]
    [InlineData("""{"credencial": "   "}""")]
    public async Task SinCredencial_Responde400(string cuerpo)
    {
        using var app = new AppFactory();
        var (cliente, _) = await ClienteDeLectorAsync(app);
        await IniciarServicioAsync(app);

        var respuesta = await cliente.PostAsync(Ruta, new StringContent(cuerpo, System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.Equal(0, await ContarRegistrosAsync(app));
    }
}
