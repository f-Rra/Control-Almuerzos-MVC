using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SCA_MVC.Data;

namespace SCA_MVC.Tests;

// La aplicación entera, levantada en memoria, para los tests que necesitan el pipeline
// de ASP.NET de verdad (filtros, antiforgery, autenticación, rutas). Se le pueden mandar
// pedidos HTTP como si fuera un navegador, sin abrir ningún puerto.
//
// La única diferencia con la aplicación real es la base: en lugar de SQL Server usa una
// SQLite en memoria, igual que BaseDeDatos. Y el administrador se crea con una clave
// conocida, porque Program.cs lo siembra al arrancar y se niega si no hay clave.
public sealed class AppFactory : WebApplicationFactory<Program>
{
    public const string AdminEmail = "admin@sca.com";
    public const string AdminClave = "ClaveDePrueba1";

    // La base vive mientras esta conexión esté abierta
    private readonly SqliteConnection _conexion = new("DataSource=:memory:");

    public AppFactory()
    {
        _conexion.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("DefaultAdmin:Email", AdminEmail);
        builder.UseSetting("DefaultAdmin:Password", AdminClave);

        builder.ConfigureServices(services =>
        {
            // Se saca la configuración de SQL Server que registró Program.cs. Desde EF Core 9
            // son dos registros por contexto: las opciones y la configuración que las arma.
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlite(_conexion));

            // Las tablas tienen que existir antes de que arranque la aplicación, porque
            // Program.cs siembra los roles y el administrador apenas arranca
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_conexion).Options;
            using var contexto = new ApplicationDbContext(opciones);
            contexto.Database.EnsureCreated();
        });
    }

    // Un cliente HTTP con la sesión del administrador iniciada. Hace lo mismo que un
    // navegador: pide la página de login, saca el token antiforgery del formulario y lo
    // manda junto con el email y la clave. Las cookies las guarda el propio cliente.
    public async Task<HttpClient> ClienteConSesionAsync()
    {
        var cliente = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var pagina = await cliente.GetStringAsync("/Account/Login");
        var respuesta = await cliente.PostAsync("/Account/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Email"] = AdminEmail,
            ["Password"] = AdminClave,
            ["__RequestVerificationToken"] = TokenDeFormulario(pagina)
        }));

        // Entrar bien redirige (302) a la pantalla del administrador; una clave mala
        // volvería a mostrar el formulario con un 200
        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        return cliente;
    }

    // El token que la pantalla deja en <meta name="csrf-token"> para que lo lea el JavaScript
    public static async Task<string> TokenDeLaPantallaAsync(HttpClient cliente, string ruta)
    {
        var html = await cliente.GetStringAsync(ruta);
        var meta = Regex.Match(html, "<meta name=\"csrf-token\" content=\"([^\"]+)\"");
        Assert.True(meta.Success, $"La pantalla {ruta} no trae <meta name=\"csrf-token\">: el JavaScript no tiene de dónde sacar el token");
        return meta.Groups[1].Value;
    }

    // Cuántos servicios hay activos en la base de la app (los que todavía no tienen duración)
    public int ServiciosActivos()
    {
        using var alcance = Services.CreateScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return contexto.Servicios.Count(s => s.DuracionMinutos == null);
    }

    // El token que Html.AntiForgeryToken() deja en un campo oculto de los formularios
    public static string TokenDeFormulario(string html)
    {
        var coincidencia = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(coincidencia.Success, "La página no trae el campo __RequestVerificationToken");
        return coincidencia.Groups[1].Value;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _conexion.Dispose();
    }
}
