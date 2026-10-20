using Microsoft.EntityFrameworkCore;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

// Un lector RFID de red se identifica contra la API con una clave propia. Estos tests cubren
// cómo se crean, se guardan y se validan esas claves; el endpoint que las usa va aparte.
// El lugar 1 es el Comedor y el 2 el Quincho (vienen del HasData).
public class LectorNegocioTests
{
    [Fact]
    public async Task Crear_DevuelveLaClaveEnClaroYGuardaSoloSuHash()
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        var (lector, clave) = await lectores.CrearAsync("Lector del comedor", idLugar: 1);

        Assert.StartsWith("sca_", clave);
        var guardado = await bd.Contexto.Lectores.AsNoTracking().SingleAsync();
        Assert.Equal(lector.IdLector, guardado.IdLector);
        Assert.NotEqual(clave, guardado.ApiKeyHash);
        Assert.Equal(LectorNegocio.HashDe(clave), guardado.ApiKeyHash);
        Assert.Equal(64, guardado.ApiKeyHash.Length);   // SHA-256 en hexadecimal
        Assert.True(guardado.Activo);
    }

    [Fact]
    public async Task Crear_DosLectores_TienenClavesDistintas()
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        var (_, primera) = await lectores.CrearAsync("Lector 1", idLugar: 1);
        var (_, segunda) = await lectores.CrearAsync("Lector 2", idLugar: 2);

        Assert.NotEqual(primera, segunda);
    }

    [Fact]
    public async Task Crear_GuardaLaFechaDeAltaDeArgentina()
    {
        // Son las 23:30 del 10 de marzo en Argentina; en UTC (la hora de Azure) ya es el 11
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.EnArgentina(2026, 3, 10, 23, 30));

        var (lector, _) = await lectores.CrearAsync("Lector del comedor", idLugar: 1);

        Assert.Equal(new DateTime(2026, 3, 10, 23, 30, 0), lector.FechaAlta);
    }

    [Fact]
    public async Task Crear_ConUnLugarQueNoExiste_Falla()
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        await Assert.ThrowsAsync<InvalidOperationException>(() => lectores.CrearAsync("Lector", idLugar: 99));
    }

    [Fact]
    public async Task Autenticar_ConSuClave_DevuelveElLectorConSuLugar()
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        var (creado, clave) = await lectores.CrearAsync("Lector del quincho", idLugar: 2);

        var lector = await lectores.AutenticarAsync(clave);

        Assert.NotNull(lector);
        Assert.Equal(creado.IdLector, lector.IdLector);
        Assert.Equal(2, lector.IdLugar);
        Assert.Equal("Quincho", lector.Lugar!.Nombre);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("sca_una-clave-que-no-existe")]
    public async Task Autenticar_ConUnaClaveQueNoEs_DevuelveNulo(string? clave)
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        await lectores.CrearAsync("Lector del comedor", idLugar: 1);

        Assert.Null(await lectores.AutenticarAsync(clave));
    }

    [Fact]
    public async Task Autenticar_ConUnLectorRevocado_DevuelveNulo()
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        var (lector, clave) = await lectores.CrearAsync("Lector del comedor", idLugar: 1);

        var revocado = await lectores.RevocarAsync(lector.IdLector);

        Assert.True(revocado);
        Assert.Null(await lectores.AutenticarAsync(clave));
    }

    [Fact]
    public async Task Revocar_UnLectorQueNoExiste_DevuelveFalso()
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        Assert.False(await lectores.RevocarAsync(99));
    }

    [Fact]
    public async Task Revocar_UnLector_NoAfectaALosDemas()
    {
        using var bd = new BaseDeDatos();
        var lectores = new LectorNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        var (primero, _) = await lectores.CrearAsync("Lector 1", idLugar: 1);
        var (_, claveDelSegundo) = await lectores.CrearAsync("Lector 2", idLugar: 2);

        await lectores.RevocarAsync(primero.IdLector);

        Assert.NotNull(await lectores.AutenticarAsync(claveDelSegundo));
    }
}
