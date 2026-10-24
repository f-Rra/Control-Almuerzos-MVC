using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SCA_MVC.Data;
using SCA_MVC.Models;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

// La pantalla donde el administrador da de alta los lectores RFID, ve la lista y los revoca.
// Es la única forma de obtener la clave de un lector: se muestra una sola vez, al crearlo.
//
// Cada test levanta su propia app. Lugar 1 = Comedor, lugar 2 = Quincho.
public class LectoresPantallaTests
{
    // Un usuario común (no administrador), con su sesión iniciada
    private static async Task<HttpClient> ClienteDeUsuarioComunAsync(AppFactory app)
    {
        using (var alcance = app.Services.CreateScope())
        {
            var usuarios = alcance.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var operador = new ApplicationUser
            {
                UserName = "operador@sca.com",
                Email = "operador@sca.com",
                NombreUsuario = "Operador",
                EmailConfirmed = true
            };
            Assert.True((await usuarios.CreateAsync(operador, "Operador1")).Succeeded);
            await usuarios.AddToRoleAsync(operador, "Usuario");
        }

        return await app.ClienteConSesionAsync("operador@sca.com", "Operador1");
    }

    // El token se saca de una pantalla que ya existe, no de la nueva: así un test que falla
    // lo hace por lo que se está probando y no porque la pantalla no se abra
    private static async Task<HttpResponseMessage> PostAsync(HttpClient cliente, string ruta, Dictionary<string, string> campos)
    {
        campos["__RequestVerificationToken"] = await AppFactory.TokenDeLaPantallaAsync(cliente, "/Servicio");
        return await cliente.PostAsync(ruta, new FormUrlEncodedContent(campos));
    }

    private static Task<HttpResponseMessage> CrearAsync(HttpClient cliente, string nombre, string idLugar) =>
        PostAsync(cliente, "/Lector/Crear", new Dictionary<string, string>
        {
            ["Nuevo.Nombre"] = nombre,
            ["Nuevo.IdLugar"] = idLugar
        });

    private static async Task<List<Lector>> LectoresEnLaBaseAsync(AppFactory app)
    {
        using var alcance = app.Services.CreateScope();
        return await alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>().Lectores.AsNoTracking().ToListAsync();
    }

    // ───────────── quién puede entrar ─────────────

    [Fact]
    public async Task SinSesion_RedirigeAlLogin()
    {
        using var app = new AppFactory();
        var cliente = app.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var respuesta = await cliente.GetAsync("/Lector");

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Contains("/Account/Login", respuesta.Headers.Location!.ToString());
    }

    [Fact]
    public async Task UnUsuarioQueNoEsAdmin_NoEntra()
    {
        using var app = new AppFactory();
        var operador = await ClienteDeUsuarioComunAsync(app);

        var respuesta = await operador.GetAsync("/Lector");

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Contains("AccessDenied", respuesta.Headers.Location!.ToString());
    }

    [Fact]
    public async Task UnUsuarioQueNoEsAdmin_NoPuedeCrearLectores()
    {
        using var app = new AppFactory();
        var operador = await ClienteDeUsuarioComunAsync(app);

        var respuesta = await CrearAsync(operador, "Lector del comedor", "1");

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.Empty(await LectoresEnLaBaseAsync(app));
    }

    [Fact]
    public async Task ElAdmin_VeLaPantallaConLosLugaresParaElegir()
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();

        var respuesta = await admin.GetAsync("/Lector");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var html = await respuesta.Content.ReadAsStringAsync();
        Assert.Contains("Comedor", html);
        Assert.Contains("Quincho", html);
    }

    // ───────────── crear un lector ─────────────

    [Fact]
    public async Task Crear_SinToken_EsRechazado()
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();

        var respuesta = await admin.PostAsync("/Lector/Crear", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Nuevo.Nombre"] = "Lector del comedor",
            ["Nuevo.IdLugar"] = "1"
        }));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.Empty(await LectoresEnLaBaseAsync(app));
    }

    [Fact]
    public async Task Crear_MuestraLaClaveUnaSolaVez_YElLectorFuncionaEnLaApi()
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();

        var respuesta = await CrearAsync(admin, "Lector del comedor", "1");

        // la página que responde al alta trae la clave, y no se puede guardar en caché
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("no-store", respuesta.Headers.CacheControl?.ToString());
        var clave = Regex.Match(await respuesta.Content.ReadAsStringAsync(), @"sca_[A-Za-z0-9_-]{43}");
        Assert.True(clave.Success, "La página del alta no muestra la clave del lector");

        // en la base quedó el lector, pero no la clave
        var lector = Assert.Single(await LectoresEnLaBaseAsync(app));
        Assert.Equal("Lector del comedor", lector.Nombre);
        Assert.Equal(1, lector.IdLugar);
        Assert.NotEqual(clave.Value, lector.ApiKeyHash);

        // y la lista que se ve después ya no la muestra: no hay forma de recuperarla
        var lista = await (await admin.GetAsync("/Lector")).Content.ReadAsStringAsync();
        Assert.Contains("Lector del comedor", lista);
        Assert.DoesNotContain(clave.Value, lista);

        // de punta a punta: la clave que mostró la pantalla sirve para registrar por la API
        using (var alcance = app.Services.CreateScope())
            await alcance.ServiceProvider.GetRequiredService<IServicioNegocio>().CrearServicioAsync(1, proyeccion: 50);
        var lectorHttp = app.CreateClient();
        lectorHttp.DefaultRequestHeaders.Add("X-Api-Key", clave.Value);
        var registro = await lectorHttp.PostAsJsonAsync("/api/registros", new { credencial = "RF001" });
        Assert.Equal(HttpStatusCode.Created, registro.StatusCode);
    }

    [Theory]
    [InlineData("", "1")]            // sin nombre
    [InlineData("   ", "1")]         // nombre en blanco
    [InlineData("Lector", "")]       // sin lugar
    [InlineData("Lector", "99")]     // un lugar que no existe
    public async Task Crear_ConDatosInvalidos_NoCreaNadaYVuelveAlFormulario(string nombre, string idLugar)
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();

        var respuesta = await CrearAsync(admin, nombre, idLugar);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.DoesNotMatch(@"sca_[A-Za-z0-9_-]{43}", await respuesta.Content.ReadAsStringAsync());
        Assert.Empty(await LectoresEnLaBaseAsync(app));
    }

    // ───────────── revocar un lector ─────────────

    [Fact]
    public async Task Revocar_DejaSinEfectoLaClaveDelLector()
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();
        string clave;
        int idLector;
        using (var alcance = app.Services.CreateScope())
        {
            var (lector, claveNueva) = await alcance.ServiceProvider.GetRequiredService<ILectorNegocio>().CrearAsync("Lector del comedor", 1);
            (idLector, clave) = (lector.IdLector, claveNueva);
            await alcance.ServiceProvider.GetRequiredService<IServicioNegocio>().CrearServicioAsync(1, proyeccion: 50);
        }

        var respuesta = await PostAsync(admin, "/Lector/Revocar", new Dictionary<string, string> { ["idLector"] = idLector.ToString() });

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        Assert.False((await LectoresEnLaBaseAsync(app)).Single().Activo);
        var lectorHttp = app.CreateClient();
        lectorHttp.DefaultRequestHeaders.Add("X-Api-Key", clave);
        var registro = await lectorHttp.PostAsJsonAsync("/api/registros", new { credencial = "RF001" });
        Assert.Equal(HttpStatusCode.Unauthorized, registro.StatusCode);
    }

    [Fact]
    public async Task Revocar_SinToken_EsRechazado()
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();
        int idLector;
        using (var alcance = app.Services.CreateScope())
            idLector = (await alcance.ServiceProvider.GetRequiredService<ILectorNegocio>().CrearAsync("Lector del comedor", 1)).Lector.IdLector;

        var respuesta = await admin.PostAsync("/Lector/Revocar", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["idLector"] = idLector.ToString()
        }));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.True((await LectoresEnLaBaseAsync(app)).Single().Activo);
    }

    [Fact]
    public async Task Revocar_UnLectorQueNoExiste_NoRompe()
    {
        using var app = new AppFactory();
        var admin = await app.ClienteConSesionAsync();

        var respuesta = await PostAsync(admin, "/Lector/Revocar", new Dictionary<string, string> { ["idLector"] = "99" });

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
    }
}
