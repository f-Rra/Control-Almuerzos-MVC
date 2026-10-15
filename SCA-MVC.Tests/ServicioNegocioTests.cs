using Microsoft.EntityFrameworkCore;
using SCA_MVC.Helpers;
using SCA_MVC.Models;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

// Cada test crea su propia base (ver BaseDeDatos) y la usa a través de ServicioNegocio,
// igual que lo hace la aplicación. Los lugares 1 (Comedor) y 2 (Quincho) vienen del
// HasData; los servicios históricos que trae ya están finalizados.
public class ServicioNegocioTests
{
    [Fact]
    public async Task CrearServicio_LoDejaActivoEnEseLugar()
    {
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        var id = await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50, invitados: 2);

        var activo = await servicios.ObtenerActivoAsync(1);
        Assert.NotNull(activo);
        Assert.Equal(id, activo.IdServicio);
        Assert.Equal(50, activo.Proyeccion);
        Assert.Equal(2, activo.TotalInvitados);
        Assert.Equal("Activo", activo.Estado);
    }

    [Fact]
    public async Task ObtenerActivo_NoTraeElServicioDeOtroLugar()
    {
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50);

        var delQuincho = await servicios.ObtenerActivoAsync(2);

        Assert.Null(delQuincho);
    }

    [Fact]
    public async Task FinalizarServicio_GuardaLosTotalesYLoPasaAFinalizado()
    {
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        var id = await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50);

        await servicios.FinalizarServicioAsync(id, totalComensales: 48, totalInvitados: 3, duracionMinutos: 55);

        Assert.Null(await servicios.ObtenerActivoAsync(1));
        var servicio = await bd.Contexto.Servicios.SingleAsync(s => s.IdServicio == id);
        Assert.Equal("Finalizado", servicio.Estado);
        Assert.Equal(48, servicio.TotalComensales);
        Assert.Equal(3, servicio.TotalInvitados);
        Assert.Equal(55, servicio.DuracionMinutos);
    }

    [Fact]
    public async Task CrearServicio_GuardaLaFechaDeArgentinaYElInicioEnUtc()
    {
        // Son las 23:30 del 10 de marzo en Argentina, pero en UTC (la hora de Azure)
        // ya son las 02:30 del 11
        var reloj = RelojDePrueba.EnArgentina(2026, 3, 10, 23, 30);
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, reloj);

        var id = await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50);

        var servicio = await bd.Contexto.Servicios.SingleAsync(s => s.IdServicio == id);
        Assert.Equal(new DateTime(2026, 3, 10), servicio.Fecha);
        Assert.Equal(new DateTime(2026, 3, 11, 2, 30, 0), servicio.HoraInicio);
    }

    [Fact]
    public async Task FinalizarPendientes_NoCierraElDeHoyAunqueEnUtcYaSeaOtroDia()
    {
        // En Argentina todavía es 10 de marzo: el servicio de esa fecha sigue siendo el de hoy
        var reloj = RelojDePrueba.EnArgentina(2026, 3, 10, 23, 30);
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, reloj);
        var deHoy = new Servicio { IdLugar = 1, Fecha = new DateTime(2026, 3, 10) };
        bd.Contexto.Servicios.Add(deHoy);
        await bd.Contexto.SaveChangesAsync();

        var cerrados = await servicios.FinalizarPendientesAsync();

        Assert.Equal(0, cerrados);
        Assert.Null((await bd.Contexto.Servicios.FindAsync(deHoy.IdServicio))!.DuracionMinutos);
    }

    [Fact]
    public async Task FinalizarPendientes_CierraSoloLosDeDiasAnteriores()
    {
        using var bd = new BaseDeDatos();
        var reloj = RelojDePrueba.EnArgentina(2026, 3, 10, 12, 0);
        var servicios = new ServicioNegocio(bd.Contexto, reloj);
        var deAyer = new Servicio { IdLugar = 1, Fecha = reloj.Hoy().AddDays(-1) };
        var deHoy = new Servicio { IdLugar = 2, Fecha = reloj.Hoy() };
        bd.Contexto.Servicios.AddRange(deAyer, deHoy);
        await bd.Contexto.SaveChangesAsync();

        var cerrados = await servicios.FinalizarPendientesAsync();

        Assert.Equal(1, cerrados);
        Assert.Equal(120, (await bd.Contexto.Servicios.FindAsync(deAyer.IdServicio))!.DuracionMinutos);
        Assert.Null((await bd.Contexto.Servicios.FindAsync(deHoy.IdServicio))!.DuracionMinutos);
    }
}
