using Microsoft.Data.SqlClient;
using SCA_MVC.Data;
using SCA_MVC.Data.Mappers;
using SCA_MVC.Models;
using System.Data;

namespace SCA_MVC.Services
{
    public class ServicioNegocio : IServicioNegocio
    {
        private readonly AccesoDatos _accesoDatos;

        public ServicioNegocio(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }

        public Task<List<Servicio>> ListarTodosAsync()
        {
            return _accesoDatos.ListarAsync("sp_ListarServicios", CommandType.StoredProcedure, MapServicioListado);
        }

        public Task<Servicio?> ObtenerActivoAsync(int idLugar)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdLugar", idLugar)
            };

            return _accesoDatos.ObtenerPrimeroAsync("sp_ObtenerServicioActivo", CommandType.StoredProcedure, ServicioMapper.Map, parametros);
        }

        public Task<Servicio?> ObtenerUltimoAsync()
        {
            return _accesoDatos.ObtenerPrimeroAsync("sp_ObtenerUltimoServicio", CommandType.StoredProcedure, ServicioMapper.Map);
        }

        public async Task<int> CrearServicioAsync(int idLugar, int? proyeccion)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdLugar", idLugar),
                new SqlParameter("@Proyeccion", (object?)proyeccion ?? DBNull.Value)
            };

            var id = await _accesoDatos.EscalarAsync("sp_IniciarServicio", CommandType.StoredProcedure, parametros);
            return Convert.ToInt32(id);
        }

        public Task FinalizarServicioAsync(int idServicio, int totalComensales, int totalInvitados, int? duracionMinutos)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdServicio", idServicio),
                new SqlParameter("@TotalComensales", totalComensales),
                new SqlParameter("@TotalInvitados", totalInvitados),
                new SqlParameter("@DuracionMinutos", (object?)duracionMinutos ?? DBNull.Value)
            };

            return _accesoDatos.EjecutarAsync("sp_FinalizarServicio", CommandType.StoredProcedure, parametros);
        }

        public Task<List<Servicio>> ListarPorFechaAsync(DateTime fechaDesde, DateTime fechaHasta, int? idLugar = null)
        {
            var parametros = new[]
            {
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@IdLugar", (object?)idLugar ?? DBNull.Value)
            };

            return _accesoDatos.ListarAsync("sp_ListarServiciosRango", CommandType.StoredProcedure, MapServicioRango, parametros);
        }

        public async Task<int> FinalizarPendientesAsync()
        {
            var resultado = await _accesoDatos.EscalarAsync("sp_FinalizarServiciosPendientes", CommandType.StoredProcedure);
            return Convert.ToInt32(resultado);
        }

        private static Servicio MapServicioListado(SqlDataReader reader)
        {
            return new Servicio
            {
                IdServicio = DbMapper.GetInt32(reader, nameof(Servicio.IdServicio)),
                Fecha = DbMapper.GetDateTime(reader, nameof(Servicio.Fecha)),
                Proyeccion = DbMapper.GetNullableInt32(reader, nameof(Servicio.Proyeccion)),
                DuracionMinutos = DbMapper.GetNullableInt32(reader, nameof(Servicio.DuracionMinutos)),
                TotalComensales = DbMapper.GetInt32(reader, nameof(Servicio.TotalComensales)),
                TotalInvitados = DbMapper.GetInt32(reader, nameof(Servicio.TotalInvitados))
            };
        }

        private static Servicio MapServicioRango(SqlDataReader reader)
        {
            return new Servicio
            {
                IdServicio = DbMapper.GetInt32(reader, nameof(Servicio.IdServicio)),
                Fecha = DbMapper.GetDateTime(reader, nameof(Servicio.Fecha)),
                Proyeccion = DbMapper.GetNullableInt32(reader, nameof(Servicio.Proyeccion)),
                DuracionMinutos = DbMapper.GetNullableInt32(reader, nameof(Servicio.DuracionMinutos)),
                TotalComensales = DbMapper.GetInt32(reader, nameof(Servicio.TotalComensales)),
                TotalInvitados = DbMapper.GetInt32(reader, nameof(Servicio.TotalInvitados))
            };
        }
    }
}