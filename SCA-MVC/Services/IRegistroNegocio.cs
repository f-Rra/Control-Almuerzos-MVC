using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface IRegistroNegocio
    {
        // true si lo registró; false si el empleado ya estaba registrado en ese servicio
        Task<bool> RegistrarAsync(int idEmpleado, int idEmpresa, int idServicio, int idLugar);
        Task<List<Registro>> ListarPorServicioAsync(int idServicio);
        Task<bool> YaRegistradoAsync(int idEmpleado, int idServicio);
        Task<int> ContarAsync(int idServicio);
        Task<List<Registro>> PorEmpresaYFechaAsync(int idEmpresa, DateTime fechaInicio, DateTime fechaFin);
    }
}