using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface ILugarNegocio
    {
        Task<List<Lugar>> ListarAsync();
        Task<Lugar?> BuscarPorNombreAsync(string nombre);
    }
}