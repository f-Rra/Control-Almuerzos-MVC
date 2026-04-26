using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class EstadisticasNegocio : IEstadisticasNegocio
    {
        private readonly ApplicationDbContext _db;

        public EstadisticasNegocio(ApplicationDbContext db) => _db = db;

        public async Task<EstadisticasViewModel> ObtenerTodosAsync()
        {
            var vm = new EstadisticasViewModel();

            vm.TotalEmpleados   = await _db.Empleados.CountAsync();
            vm.EmpleadosActivos  = await _db.Empleados.CountAsync(e => e.Estado);
            vm.EmpleadosInactivos = vm.TotalEmpleados - vm.EmpleadosActivos;

            vm.TotalEmpresasActivas  = await _db.Empresas.CountAsync(e => e.Estado);
            vm.EmpresasConEmpleados  = await _db.Empresas.CountAsync(e => e.Estado && e.Empleados.Any(emp => emp.Estado));
            vm.PromedioEmpleados = vm.EmpresasConEmpleados > 0
                ? Math.Round((decimal)vm.EmpleadosActivos / vm.EmpresasConEmpleados, 1)
                : 0;

            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            vm.ServiciosEsteMes  = await _db.Servicios.CountAsync(s => s.Fecha >= inicioMes && s.Fecha <= hoy);
            vm.ServiciosEsteAnio = await _db.Servicios.CountAsync(s => s.Fecha.Year == hoy.Year);

            var diasTranscurridos = Math.Max(1, hoy.Day);
            vm.PromedioPorDia = (int)Math.Round((double)vm.ServiciosEsteMes / diasTranscurridos);

            // Top 5 Empresas
            var asistenciasMes = await _db.Registros
                .Where(r => r.Fecha >= inicioMes && r.Fecha <= hoy)
                .GroupBy(r => new { r.IdEmpresa, NombreEmpresa = r.Empresa!.Nombre })
                .Select(g => new { g.Key.NombreEmpresa, Conteos = g.Count() })
                .OrderByDescending(x => x.Conteos)
                .Take(5)
                .ToListAsync();

            var totalAsistencias = await _db.Registros.CountAsync(r => r.Fecha >= inicioMes && r.Fecha <= hoy);

            vm.TopEmpresas = asistenciasMes.Select((x, i) => new TopEmpresaItem
            {
                Ranking = i + 1,
                NombreEmpresa = x.NombreEmpresa ?? "-",
                TotalAsistencias = x.Conteos,
                Porcentaje = totalAsistencias > 0 ? Math.Round((decimal)x.Conteos / totalAsistencias * 100, 2) : 0
            }).ToList();

            return vm;
        }
    }
}
