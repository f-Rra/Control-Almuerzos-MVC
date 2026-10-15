using SCA_MVC.Models;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

public class EstadisticasNegocioTests
{
    [Fact]
    public async Task ServiciosEsteMes_CuentaElMesDeArgentinaAunqueEnUtcYaSeaElSiguiente()
    {
        // Es el último día de julio a las 23:30 en Argentina, pero en UTC (la hora de
        // Azure) ya son las 02:30 del 1 de agosto. El HasData llega hasta abril, así que
        // julio y agosto arrancan vacíos.
        var reloj = RelojDePrueba.EnArgentina(2026, 7, 31, 23, 30);
        using var bd = new BaseDeDatos();
        bd.Contexto.Servicios.AddRange(
            new Servicio { IdLugar = 1, Fecha = new DateTime(2026, 7, 31), DuracionMinutos = 60 },
            new Servicio { IdLugar = 2, Fecha = new DateTime(2026, 7, 31), DuracionMinutos = 45 },
            new Servicio { IdLugar = 1, Fecha = new DateTime(2026, 8, 1), DuracionMinutos = 60 });
        await bd.Contexto.SaveChangesAsync();
        var estadisticas = new EstadisticasNegocio(bd.Contexto, reloj);

        var vm = await estadisticas.ObtenerTodosAsync();

        // los dos del 31 son de julio; el del 1 de agosto todavía no pasó en Argentina
        Assert.Equal(2, vm.ServiciosEsteMes);
    }
}
