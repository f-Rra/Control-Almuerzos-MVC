using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface IServicioNegocio
    {
        Task<Servicio?> ObtenerActivoAsync(int idLugar);
        Task<Servicio?> ObtenerActivoGlobalAsync();
        Task<int> CrearServicioAsync(int idLugar, int? proyeccion, int invitados = 0);
        Task FinalizarServicioAsync(int idServicio, int totalComensales, int totalInvitados, int? duracionMinutos);
        Task<int> FinalizarPendientesAsync();
    }
}