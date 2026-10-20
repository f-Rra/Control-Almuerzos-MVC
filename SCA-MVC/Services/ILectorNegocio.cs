using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface ILectorNegocio
    {
        // Crea el lector y devuelve su clave en claro. Es la única vez que se la puede ver:
        // en la base queda solo el hash.
        Task<(Lector Lector, string Clave)> CrearAsync(string nombre, int idLugar);

        // El lector dueño de esa clave, o null si no existe o está revocado
        Task<Lector?> AutenticarAsync(string? clave);

        Task<List<Lector>> ListarAsync();

        // false si el lector no existe
        Task<bool> RevocarAsync(int idLector);
    }
}
