using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

// El empleado 1 es de la empresa 1 (viene del HasData). Cada test arma su servicio
// activo con ServicioNegocio, como hace la pantalla de Servicios.
public class RegistroNegocioTests
{
    [Fact]
    public async Task Registrar_GuardaElRegistroDelEmpleadoEnElServicio()
    {
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera()).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);

        var registro = await bd.Contexto.Registros.SingleAsync(r => r.IdServicio == idServicio);
        Assert.Equal(1, registro.IdEmpleado);
        Assert.Equal(1, registro.IdEmpresa);
        Assert.Equal(1, registro.IdLugar);
    }

    [Fact]
    public async Task Registrar_GuardaLaFechaYLaHoraDeArgentina()
    {
        // Son las 23:30 del 10 de marzo en Argentina; en UTC (la hora de Azure) ya es el 11
        var reloj = RelojDePrueba.EnArgentina(2026, 3, 10, 23, 30);
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, reloj).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, reloj);

        await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);

        var registro = await bd.Contexto.Registros.SingleAsync(r => r.IdServicio == idServicio);
        Assert.Equal(new DateTime(2026, 3, 10), registro.Fecha);
        Assert.Equal(new TimeSpan(23, 30, 0), registro.Hora);
    }

    [Fact]
    public async Task Registrar_SiYaEstaRegistrado_DevuelveYaRegistradoYNoLoDuplica()
    {
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera()).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        var primera = await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);
        var segunda = await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);

        Assert.Equal(ResultadoRegistro.Registrado, primera);
        Assert.Equal(ResultadoRegistro.YaRegistrado, segunda);
        Assert.Equal(1, await registros.ContarAsync(idServicio));
    }

    [Fact]
    public async Task Registrar_ConUnEmpleadoQueNoExiste_SigueFallando()
    {
        // Que no se confunda cualquier error de la base con un duplicado: este es otro
        // problema (la clave foránea del empleado) y tiene que seguir saliendo como error
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera()).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        await Assert.ThrowsAnyAsync<DbException>(() =>
            registros.RegistrarAsync(idEmpleado: 99999, idEmpresa: 1, idServicio: idServicio, idLugar: 1));
    }

    [Fact]
    public async Task Registrar_EnUnServicioFinalizado_NoRegistra()
    {
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        var idServicio = await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50);
        await servicios.FinalizarServicioAsync(idServicio, totalComensales: 0, totalInvitados: 0, duracionMinutos: 30);
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        var resultado = await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);

        Assert.Equal(ResultadoRegistro.ServicioFinalizado, resultado);
        Assert.Equal(0, await registros.ContarAsync(idServicio));
    }

    [Fact]
    public async Task Registrar_EnUnServicioQueNoExiste_NoRegistra()
    {
        using var bd = new BaseDeDatos();
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        var resultado = await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: 99999, idLugar: 1);

        Assert.Equal(ResultadoRegistro.ServicioInexistente, resultado);
    }

    [Fact]
    public async Task Registrar_ConUnLugarQueNoEsElDelServicio_NoRegistra()
    {
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera()).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        // el servicio es del Comedor (1) y el pedido dice Quincho (2)
        var resultado = await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 2);

        Assert.Equal(ResultadoRegistro.LugarIncorrecto, resultado);
        Assert.Equal(0, await registros.ContarAsync(idServicio));
    }

    [Fact]
    public async Task YaRegistrado_EsVerdaderoSoloParaQuienPaso()
    {
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera()).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());
        await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);

        Assert.True(await registros.YaRegistradoAsync(idEmpleado: 1, idServicio: idServicio));
        Assert.False(await registros.YaRegistradoAsync(idEmpleado: 2, idServicio: idServicio));
    }

    [Fact]
    public async Task Contar_CuentaLosRegistrosDeEseServicio()
    {
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, RelojDePrueba.Cualquiera()).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, RelojDePrueba.Cualquiera());

        await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);
        await registros.RegistrarAsync(idEmpleado: 2, idEmpresa: 1, idServicio: idServicio, idLugar: 1);

        Assert.Equal(2, await registros.ContarAsync(idServicio));
    }
}
