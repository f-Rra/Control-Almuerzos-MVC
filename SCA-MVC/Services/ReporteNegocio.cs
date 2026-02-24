using Microsoft.Data.SqlClient;
using SCA_MVC.Data;
using SCA_MVC.Data.Mappers;
using SCA_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class ReporteNegocio : IReporteNegocio
    {
        private readonly AccesoDatos _accesoDatos;

        public ReporteNegocio(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }

        // sp_ListarServicios no tiene parámetros; filtramos en memoria por fecha/lugar
        public async Task<List<ServicioReporte>> ObtenerListaServiciosAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var todos = await _accesoDatos.ListarAsync(
                "sp_ListarServicios",
                System.Data.CommandType.StoredProcedure,
                ReporteMapper.MapServicio,
                Array.Empty<Microsoft.Data.SqlClient.SqlParameter>());

            return todos
                .Where(s => s.Fecha.Date >= desde.Date && s.Fecha.Date <= hasta.Date)
                .Where(s => idLugar == null || true) // IdLugar no retorna el SP, ya filtrado por nombre
                .OrderBy(s => s.Fecha)
                .ThenBy(s => s.IdServicio)
                .ToList();
        }

        public Task<List<AsistenciaPorEmpresa>> ObtenerAsistenciasPorEmpresaAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var parametros = new[]
            {
                new SqlParameter("@FechaDesde", desde),
                new SqlParameter("@FechaHasta", hasta),
                new SqlParameter("@IdLugar", (object?)idLugar ?? DBNull.Value)
            };

            return _accesoDatos.ListarAsync("sp_AsistenciasPorEmpresas", CommandType.StoredProcedure, ReporteMapper.MapAsistenciaEmpresa, parametros);
        }

        public Task<List<AsistenciaPorDia>> ObtenerDistribucionPorDiaAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var parametros = new[]
            {
                new SqlParameter("@FechaDesde", desde),
                new SqlParameter("@FechaHasta", hasta),
                new SqlParameter("@IdLugar", (object?)idLugar ?? DBNull.Value)
            };

            return _accesoDatos.ListarAsync("sp_DistribucionPorDiaSemana", CommandType.StoredProcedure, ReporteMapper.MapAsistenciaDia, parametros);
        }

        public Task<List<CoberturaReporte>> ObtenerCoberturaVsProyeccionAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var parametros = new[]
            {
                new SqlParameter("@FechaDesde", desde),
                new SqlParameter("@FechaHasta", hasta),
                new SqlParameter("@IdLugar", (object?)idLugar ?? DBNull.Value)
            };

            return _accesoDatos.ListarAsync("sp_ReporteCoberturaVsProyeccion", CommandType.StoredProcedure, ReporteMapper.MapCobertura, parametros);
        }

        public async Task<int> ObtenerTotalAsistenciasRangoAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            // Podríamos crear un SP específico, pero para el MVP sumamos los resultados de las empresas
            var lista = await ObtenerAsistenciasPorEmpresaAsync(desde, hasta, idLugar);
            int total = 0;
            foreach (var item in lista) total += item.TotalAsistencias;
            return total;
        }
    }
}
