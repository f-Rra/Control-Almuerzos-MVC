using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface IRegistroNegocio
    {
        // Registra al empleado en el servicio, o dice por qué no pudo (ver ResultadoRegistro)
        Task<ResultadoRegistro> RegistrarAsync(int idEmpleado, int idEmpresa, int idServicio, int idLugar);
        Task<List<Registro>> ListarPorServicioAsync(int idServicio);
        Task<bool> YaRegistradoAsync(int idEmpleado, int idServicio);
        Task<int> ContarAsync(int idServicio);
        Task<List<Registro>> PorEmpresaYFechaAsync(int idEmpresa, DateTime fechaInicio, DateTime fechaFin);
    }
}