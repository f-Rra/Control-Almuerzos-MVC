using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class ReporteNegocio : IReporteNegocio
    {
        private readonly ApplicationDbContext _db;

        public ReporteNegocio(ApplicationDbContext db) => _db = db;

        public async Task<List<ServicioReporte>> ObtenerListaServiciosAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var query = _db.Servicios.Include(s => s.Lugar)
                .Where(s => s.Fecha.Date >= desde.Date && s.Fecha.Date <= hasta.Date);

            if (idLugar.HasValue && idLugar.Value > 0)
                query = query.Where(s => s.IdLugar == idLugar.Value);

            var servicios = await query.OrderBy(s => s.Fecha).ToListAsync();

            return servicios.Select(s => new ServicioReporte
            {
                IdServicio = s.IdServicio,
                Fecha = s.Fecha,
                Lugar = s.Lugar?.Nombre ?? "-",
                Proyeccion = s.Proyeccion ?? 0,
                DuracionMinutos = s.DuracionMinutos,
                TotalComensales = s.TotalComensales,
                TotalInvitados = s.TotalInvitados
            }).ToList();
        }

        public async Task<List<AsistenciaPorEmpresa>> ObtenerAsistenciasPorEmpresaAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var query = _db.Registros.Include(r => r.Empresa)
                .Where(r => r.Fecha >= desde.Date && r.Fecha <= hasta.Date);

            if (idLugar.HasValue && idLugar.Value > 0)
                query = query.Where(r => r.IdLugar == idLugar.Value);

            var agrupado = await query
                .GroupBy(r => r.Empresa)
                .Select(g => new
                {
                    Empresa = g.Key,
                    Asistencias = g.Count()
                })
                .OrderByDescending(x => x.Asistencias)
                .ToListAsync();

            return agrupado.Select(x => new AsistenciaPorEmpresa
            {
                Empresa = x.Empresa?.Nombre ?? "-",
                TotalAsistencias = x.Asistencias
            }).ToList();
        }

        public async Task<List<AsistenciaPorDia>> ObtenerDistribucionPorDiaAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var query = _db.Registros
                .Where(r => r.Fecha >= desde.Date && r.Fecha <= hasta.Date);

            if (idLugar.HasValue && idLugar.Value > 0)
                query = query.Where(r => r.IdLugar == idLugar.Value);

            var list = await query.ToListAsync();

            var dias = list.GroupBy(r => r.Fecha.DayOfWeek)
                .Select(g => new
                {
                    DiaSemana = TraducirDia(g.Key),
                    TotalAsistencias = g.Count()
                })
                .ToList();

            var ordenDias = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

            return dias.OrderBy(d => Array.IndexOf(ordenDias, d.DiaSemana))
                .Select(d => new AsistenciaPorDia
                {
                    Orden = Array.IndexOf(ordenDias, d.DiaSemana) + 1,
                    Dia = d.DiaSemana,
                    Total = d.TotalAsistencias
                }).ToList();
        }

        private string TraducirDia(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => ""
            };
        }

        public async Task<List<CoberturaReporte>> ObtenerCoberturaVsProyeccionAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var query = _db.Servicios.Include(s => s.Lugar)
                .Where(s => s.Fecha >= desde.Date && s.Fecha <= hasta.Date);

            if (idLugar.HasValue && idLugar.Value > 0)
                query = query.Where(s => s.IdLugar == idLugar.Value);

            var agrupado = await query.GroupBy(s => new { FechaDate = s.Fecha.Date, NombreLugar = s.Lugar.Nombre }).Select(g => new {
                Fecha = g.Key.FechaDate,
                Lugar = g.Key.NombreLugar,
                Proyeccion = g.Sum(x => x.Proyeccion ?? 0),
                Asistencias = g.Sum(x => x.TotalComensales + x.TotalInvitados)
            }).OrderBy(x => x.Fecha).ToListAsync();

            return agrupado.Select(x => new CoberturaReporte
            {
                Fecha = x.Fecha,
                Lugar = x.Lugar ?? "-",
                Proyeccion = x.Proyeccion,
                Atendidos = x.Asistencias,
                Diferencia = x.Asistencias - x.Proyeccion,
                CoberturaPorcentaje = x.Proyeccion > 0 ? Math.Round((decimal)x.Asistencias / x.Proyeccion * 100, 2) : 0
            }).ToList();
        }

        public async Task<int> ObtenerTotalAsistenciasRangoAsync(DateTime desde, DateTime hasta, int? idLugar)
        {
            var query = _db.Registros
                .Where(r => r.Fecha >= desde.Date && r.Fecha <= hasta.Date);

            if (idLugar.HasValue && idLugar.Value > 0)
                query = query.Where(r => r.IdLugar == idLugar.Value);

            return await query.CountAsync();
        }
    }
}
