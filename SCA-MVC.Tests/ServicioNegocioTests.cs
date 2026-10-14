using Microsoft.EntityFrameworkCore;
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
        var servicios = new ServicioNegocio(bd.Contexto);

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
        var servicios = new ServicioNegocio(bd.Contexto);
        await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50);

        var delQuincho = await servicios.ObtenerActivoAsync(2);

        Assert.Null(delQuincho);
    }

    [Fact]
    public async Task FinalizarServicio_GuardaLosTotalesYLoPasaAFinalizado()
    {
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto);
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
    public async Task FinalizarPendientes_CierraSoloLosDeDiasAnteriores()
    {
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto);
        var deAyer = new Servicio { IdLugar = 1, Fecha = DateTime.Today.AddDays(-1) };
        var deHoy = new Servicio { IdLugar = 2, Fecha = DateTime.Today };
        bd.Contexto.Servicios.AddRange(deAyer, deHoy);
        await bd.Contexto.SaveChangesAsync();

        var cerrados = await servicios.FinalizarPendientesAsync();

        Assert.Equal(1, cerrados);
        Assert.Equal(120, (await bd.Contexto.Servicios.FindAsync(deAyer.IdServicio))!.DuracionMinutos);
        Assert.Null((await bd.Contexto.Servicios.FindAsync(deHoy.IdServicio))!.DuracionMinutos);
    }
}
