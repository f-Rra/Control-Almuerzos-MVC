using SCA_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public interface IReporteNegocio
    {
        Task<List<ServicioReporte>>       ObtenerListaServiciosAsync(DateTime desde, DateTime hasta, int? idLugar);
        Task<List<AsistenciaPorEmpresa>>  ObtenerAsistenciasPorEmpresaAsync(DateTime desde, DateTime hasta, int? idLugar);
        Task<List<AsistenciaPorDia>>      ObtenerDistribucionPorDiaAsync(DateTime desde, DateTime hasta, int? idLugar);
        Task<List<CoberturaReporte>>      ObtenerCoberturaVsProyeccionAsync(DateTime desde, DateTime hasta, int? idLugar);
        Task<int>                         ObtenerTotalAsistenciasRangoAsync(DateTime desde, DateTime hasta, int? idLugar);
    }
}
