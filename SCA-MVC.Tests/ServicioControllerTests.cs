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
    // registros y empleados: para los tests que necesitan reemplazar alguno de esos servicios
    private static ServicioController CrearControlador(BaseDeDatos bd, TimeProvider reloj,
        IRegistroNegocio? registros = null, IEmpleadoNegocio? empleados = null) => new(
        new ServicioNegocio(bd.Contexto, reloj),
        new LugarNegocio(bd.Contexto),
        registros ?? new RegistroNegocio(bd.Contexto, reloj),
        empleados ?? new EmpleadoNegocio(bd.Contexto),
        reloj);

    // Un servicio de empleados que explota si alguien le pide la lista completa. Sirve para
    // comprobar que una pantalla NO la carga: si la pidiera, el test fallaría con esta excepción.
    private sealed class EmpleadoNegocioSinListado : IEmpleadoNegocio
    {
        public Task<List<Empleado>> ListarAsync() => throw new InvalidOperationException("No se debe cargar a todos los empleados");

        public Task<Empleado?> BuscarPorCredencialAsync(string idCredencial) => throw new NotImplementedException();
        public Task<Empleado?> BuscarPorIdAsync(int idEmpleado) => throw new NotImplementedException();
        public Task<int> AgregarAsync(Empleado empleado) => throw new NotImplementedException();
        public Task ModificarAsync(Empleado empleado) => throw new NotImplementedException();
        public Task EliminarAsync(int idEmpleado) => throw new NotImplementedException();
        public Task<bool> ExisteCredencialAsync(string idCredencial) => throw new NotImplementedException();
        public Task<List<Empleado>> FiltrarEmpleadosAsync(string? filtro, int? idEmpresa) => throw new NotImplementedException();
        public Task<(List<Empleado> Items, int Total)> FiltrarPaginadoAsync(string? filtro, int? idEmpresa, int pagina, int pageSize) => throw new NotImplementedException();
        public Task<List<Empleado>> EmpleadosSinAlmorzarAsync(int idServicio) => throw new NotImplementedException();
        public Task<List<Empleado>> FiltrarSinAlmorzarAsync(int idServicio, int? idEmpresa, string? nombre) => throw new NotImplementedException();
    }

    [Fact]
    public async Task Index_MuestraLosUltimosRegistrosConSuEmpleadoYEmpresa_SinCargarATodosLosEmpleados()
    {
        // Empleado 1 pasa a las 12:00 y el 2 a las 12:05
        var reloj = RelojDePrueba.EnArgentina(2026, 3, 10, 12, 0);
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, reloj).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var registros = new RegistroNegocio(bd.Contexto, reloj);
        await registros.RegistrarAsync(idEmpleado: 1, idEmpresa: 1, idServicio: idServicio, idLugar: 1);
        reloj.Advance(TimeSpan.FromMinutes(5));
        await registros.RegistrarAsync(idEmpleado: 2, idEmpresa: 1, idServicio: idServicio, idLugar: 1);
        var segundo = await bd.Contexto.Empleados.AsNoTracking().Include(e => e.Empresa).SingleAsync(e => e.IdEmpleado == 2);
        var controlador = CrearControlador(bd, reloj, empleados: new EmpleadoNegocioSinListado());

        var vista = Assert.IsType<ViewResult>(await controlador.Index());

        var modelo = Assert.IsType<ViewModels.ServicioActivoViewModel>(vista.Model);
        Assert.Equal(2, modelo.RegistradosHoy);
        Assert.Equal(2, modelo.UltimosRegistros.Count);
        // el más nuevo primero, con el nombre del empleado y de su empresa que usa la pantalla
        Assert.Equal($"{segundo.Nombre} {segundo.Apellido}", modelo.UltimosRegistros[0].NombreEmpleado);
        Assert.Equal(segundo.Empresa!.Nombre, modelo.UltimosRegistros[0].NombreEmpresa);
        Assert.Equal(new TimeSpan(12, 5, 0), modelo.UltimosRegistros[0].Hora);
    }

    // Un servicio de registros de mentira que reproduce la carrera de dos lecturas juntas:
    // cuando el controlador pregunta si el empleado ya está registrado la respuesta es "no"
    // (la otra lectura todavía no había insertado), pero al intentar registrar ya se le
    // adelantó y la base lo rechaza por duplicado. Un test así no depende de la velocidad
    // de la máquina: la carrera real es imposible de repetir a pedido.
    private sealed class RegistroNegocioAdelantado : IRegistroNegocio
    {
        public Task<bool> YaRegistradoAsync(int idEmpleado, int idServicio) => Task.FromResult(false);
        public Task<ResultadoRegistro> RegistrarAsync(int idEmpleado, int idEmpresa, int idServicio, int idLugar) => Task.FromResult(ResultadoRegistro.YaRegistrado);

        public Task<List<Registro>> ListarPorServicioAsync(int idServicio) => throw new NotImplementedException();
        public Task<int> ContarAsync(int idServicio) => throw new NotImplementedException();
        public Task<List<Registro>> PorEmpresaYFechaAsync(int idEmpresa, DateTime fechaInicio, DateTime fechaFin) => throw new NotImplementedException();
    }

    [Fact]
    public async Task Registrar_SiOtraLecturaSeAdelanto_RespondeYaRegistrado()
    {
        var reloj = RelojDePrueba.Cualquiera();
        using var bd = new BaseDeDatos();
        var idServicio = await new ServicioNegocio(bd.Contexto, reloj).CrearServicioAsync(idLugar: 1, proyeccion: 50);
        var controlador = CrearControlador(bd, reloj, new RegistroNegocioAdelantado());

        var resultado = Assert.IsType<JsonResult>(await controlador.Registrar("RF001", idServicio, 1));

        var json = JsonSerializer.SerializeToElement(resultado.Value);
        Assert.False(json.GetProperty("success").GetBoolean());
        Assert.Equal("Ya registrado en este servicio", json.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Registrar_EnUnServicioFinalizado_RespondeQueYaTermino()
    {
        // La pantalla quedó abierta con el servicio de antes: ya se cerró (por ejemplo,
        // se auto-finalizó al cerrar el navegador) y el lector sigue mandando credenciales
        var reloj = RelojDePrueba.Cualquiera();
        using var bd = new BaseDeDatos();
        var servicios = new ServicioNegocio(bd.Contexto, reloj);
        var idServicio = await servicios.CrearServicioAsync(idLugar: 1, proyeccion: 50);
        await servicios.FinalizarServicioAsync(idServicio, totalComensales: 0, totalInvitados: 0, duracionMinutos: 30);
        var controlador = CrearControlador(bd, reloj);

        var resultado = Assert.IsType<JsonResult>(await controlador.Registrar("RF001", idServicio, 1));

        var json = JsonSerializer.SerializeToElement(resultado.Value);
        Assert.False(json.GetProperty("success").GetBoolean());
        Assert.Equal("El servicio ya terminó", json.GetProperty("message").GetString());
        Assert.Equal(0, await new RegistroNegocio(bd.Contexto, reloj).ContarAsync(idServicio));
    }

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
