using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Controllers;
using SCA_MVC.Services;

namespace SCA_MVC.Tests;

// RegistrarMultiples es la acción del registro manual: recibe una lista de empleados.
// Los empleados 1 a 10 son de la empresa 1 (vienen del HasData).
public class RegistroControllerTests
{
    private static RegistroController CrearControlador(BaseDeDatos bd, TimeProvider reloj) => new(
        new EmpleadoNegocio(bd.Contexto),
        new EmpresaNegocio(bd.Contexto),
        new RegistroNegocio(bd.Contexto, reloj),
        new ServicioNegocio(bd.Contexto, reloj),
        reloj);

    [Fact]
    public async Task RegistrarMultiples_SaltaAlQueYaEstabaYRegistraAlResto()
    {
        var reloj = RelojDePrueba.Cualquiera();
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, reloj).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, reloj);
        await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);
        var controlador = CrearControlador(bd, reloj);

        var resultado = Assert.IsType<JsonResult>(await controlador.RegistrarMultiples(new RegistroMultipleRequest
        {
            IdServicio = idServicio,
            IdLugar = 1,
            IdsEmpleados = { 1, 2, 3 }
        }));

        var json = JsonSerializer.SerializeToElement(resultado.Value);
        Assert.True(json.GetProperty("success").GetBoolean());
        Assert.Equal(2, json.GetProperty("count").GetInt32());   // el 1 ya estaba
        Assert.Equal(3, await registros.ContarAsync(idServicio));
    }

    [Fact]
    public async Task RegistrarMultiples_EnUnServicioFinalizado_NoRegistraNada()
    {
        var reloj = RelojDePrueba.Cualquiera();
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, reloj);
        var idServicio = await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50);
        await servicios.FinalizarServicioAsync(idServicio, totalComensales: 0, totalInvitados: 0, duracionMinutos: 30);
        var controlador = CrearControlador(bd, reloj);

        var resultado = Assert.IsType<JsonResult>(await controlador.RegistrarMultiples(new RegistroMultipleRequest
        {
            IdServicio = idServicio,
            IdLugar = 1,
            IdsEmpleados = { 1, 2 }
        }));

        var json = JsonSerializer.SerializeToElement(resultado.Value);
        Assert.False(json.GetProperty("success").GetBoolean());
        Assert.Equal("El servicio ya terminó", json.GetProperty("message").GetString());
        Assert.Equal(0, await new RegistroNegocio(bd.Contexto, reloj).ContarAsync(idServicio));
    }
}
