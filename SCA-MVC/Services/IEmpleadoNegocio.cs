using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface IEmpleadoNegocio
    {
        Task<List<Empleado>> ListarAsync();
        Task<Empleado?> BuscarPorCredencialAsync(string idCredencial);
        Task<Empleado?> BuscarPorIdAsync(int idEmpleado);
        Task<int> AgregarAsync(Empleado empleado);
        Task ModificarAsync(Empleado empleado);
        Task EliminarAsync(int idEmpleado);
        Task<bool> ExisteCredencialAsync(string idCredencial);
        Task<List<Empleado>> FiltrarEmpleadosAsync(string? filtro, int? idEmpresa);
        Task<List<Empleado>> EmpleadosSinAlmorzarAsync(int idServicio);
        Task<List<Empleado>> FiltrarSinAlmorzarAsync(int idServicio, int? idEmpresa, string? nombre);
    }
}