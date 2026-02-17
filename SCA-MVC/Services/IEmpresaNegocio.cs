using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface IEmpresaNegocio
    {
        Task<List<Empresa>> ListarAsync();
        Task<List<Empresa>> ListarConEmpleadosAsync();
        Task<Empresa?> BuscarPorIdAsync(int idEmpresa);
        Task<int> AgregarAsync(Empresa empresa);
        Task ModificarAsync(Empresa empresa);
        Task EliminarAsync(int idEmpresa);
        Task<List<Empresa>> FiltrarAsync(string? filtro);
    }
}