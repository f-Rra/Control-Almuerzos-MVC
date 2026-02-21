using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public interface IServicioNegocio
    {
        Task<List<Servicio>> ListarTodosAsync();
        Task<Servicio?> ObtenerActivoAsync(int idLugar);
        Task<Servicio?> ObtenerActivoGlobalAsync();
        Task<Servicio?> ObtenerUltimoAsync();
        Task<int> CrearServicioAsync(int idLugar, int? proyeccion, int invitados = 0);
        Task FinalizarServicioAsync(int idServicio, int totalComensales, int totalInvitados, int? duracionMinutos);
        Task<List<Servicio>> ListarPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta, int? idLugar = null);
        Task<int> FinalizarPendientesAsync();
    }
}