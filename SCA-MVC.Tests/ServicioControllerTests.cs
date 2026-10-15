using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Controllers;
using SCA_MVC.Models;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

// Prueban las acciones del controlador llamándolas como un método más, con los
// servicios de verdad conectados a la base de prueba. Las dos acciones que se
// llaman desde JavaScript (Registrar y FinalizarDirecto) no usan TempData ni la
// sesión, así que no hace falta armar un pedido HTTP.
public class ServicioControllerTests
{
    private static ServicioController CrearControlador(BaseDeDatos bd, TimeProvider reloj) => new(
        new ServicioNegocio(bd.Contexto, reloj),
        new LugarNegocio(bd.Contexto),
        new RegistroNegocio(bd.Contexto, reloj),
        new EmpleadoNegocio(bd.Contexto),
        reloj);

    [Fact]
    public async Task Registrar_DevuelveLaHoraDeArgentina()
    {
        var reloj = RelojDePrueba.EnArgentina(2026, 3, 10, 23, 30);
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, reloj).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var controlador = CrearControlador(bd, reloj);

        // RF001 es la credencial del empleado 1 (viene del HasData)
        var resultado = Assert.IsType<JsonResult>(await controlador.Registrar("RF001", idServicio, 1));

        var json = JsonSerializer.SerializeToElement(resultado.Value);
        Assert.True(json.GetProperty("success").GetBoolean());
        Assert.Equal("23:30", json.GetProperty("hora").GetString());
    }

    [Fact]
    public async Task FinalizarDirecto_CalculaLaDuracionDesdeElInicioEnUtc()
    {
        // Son las 12:00 en Argentina, o sea las 15:00 UTC. El servicio empezó a las
        // 14:15 UTC (HoraInicio se guarda en UTC), así que lleva 45 minutos.
        var reloj = RelojDePrueba.EnArgentina(2026, 3, 10, 12, 0);
        using var bd = new BaseDeDatos();
        var servicio = new Servicio
        {
            IdLugar = 1,
            Fecha = new DateTime(2026, 3, 10),
            HoraInicio = new DateTime(2026, 3, 10, 14, 15, 0)
        };
        bd.Contexto.Servicios.Add(servicio);
        await bd.Contexto.SaveChangesAsync();
        var controlador = CrearControlador(bd, reloj);

        await controlador.FinalizarDirecto();

        var guardado = await bd.Contexto.Servicios.AsNoTracking().SingleAsync(s => s.IdServicio == servicio.IdServicio);
        Assert.Equal(45, guardado.DuracionMinutos);
    }
}
