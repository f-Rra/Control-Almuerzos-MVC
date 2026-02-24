using System;
using System.Collections.Generic;

namespace SCA_MVC.ViewModels
{
    public class ReporteViewModel
    {
        public DateTime FechaDesde { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime FechaHasta { get; set; } = DateTime.Today;
        public int? IdLugar { get; set; }
        public string TipoReporte { get; set; } = "servicios"; // servicios | empresas | cobertura | dias

        // KPIs
        public int TotalServicios { get; set; }
        public int TotalAsistencias { get; set; }
        public int PromedioAsistencias { get; set; }

        // Datos por tipo
        public List<ServicioReporte>      ListaServicios        { get; set; } = new();
        public List<AsistenciaPorEmpresa> AsistenciasPorEmpresa { get; set; } = new();
        public List<AsistenciaPorDia>     AsistenciasPorDia     { get; set; } = new();
        public List<CoberturaReporte>     CoberturaVsProyeccion { get; set; } = new();
    }

    // ── Reporte 1: Lista de Servicios ────────────────────────────────────────
    public class ServicioReporte
    {
        public int      IdServicio      { get; set; }
        public DateTime Fecha           { get; set; }
        public string   Lugar           { get; set; } = "";
        public int      Proyeccion      { get; set; }
        public int?     DuracionMinutos { get; set; }
        public int      TotalComensales { get; set; }
        public int      TotalInvitados  { get; set; }
        public int      TotalGeneral    => TotalComensales + TotalInvitados;
    }

    // ── Reporte 2: Asistencias por Empresa ───────────────────────────────────
    public class AsistenciaPorEmpresa
    {
        public string Empresa          { get; set; } = "";
        public int    TotalAsistencias { get; set; }
    }

    // ── Reporte 3: Distribución por Día ──────────────────────────────────────
    public class AsistenciaPorDia
    {
        public int    Orden { get; set; }
        public string Dia   { get; set; } = "";
        public int    Total { get; set; }
    }

    // ── Reporte 4: Cobertura vs Proyección ───────────────────────────────────
    public class CoberturaReporte
    {
        public DateTime Fecha               { get; set; }
        public string   Lugar               { get; set; } = "";
        public int      Proyeccion          { get; set; }
        public int      Atendidos           { get; set; }
        public decimal? CoberturaPorcentaje { get; set; }
        public int      Diferencia          { get; set; }
    }
}
