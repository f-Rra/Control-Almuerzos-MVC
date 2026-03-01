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

            var empleados = await _db.Empleados.ToListAsync();
            vm.TotalEmpleados = empleados.Count;
            vm.EmpleadosActivos = empleados.Count(e => e.Estado);
            vm.EmpleadosInactivos = empleados.Count(e => !e.Estado);

            var empresas = await _db.Empresas.Include(e => e.Empleados).ToListAsync();
            vm.TotalEmpresasActivas = empresas.Count(e => e.Estado);
            vm.EmpresasConEmpleados = empresas.Count(e => e.Estado && e.Empleados.Any(emp => emp.Estado));
            vm.PromedioEmpleados = vm.EmpresasConEmpleados > 0 
                ? Math.Round((decimal)vm.EmpleadosActivos / vm.EmpresasConEmpleados, 1) 
                : 0;

            var servicios = await _db.Servicios.ToListAsync();
            vm.ServiciosEsteMes = servicios.Count(s => s.Fecha.Month == DateTime.Today.Month && s.Fecha.Year == DateTime.Today.Year);
            vm.ServiciosEsteAnio = servicios.Count(s => s.Fecha.Year == DateTime.Today.Year);
            
            var diasTranscurridos = Math.Max(1, DateTime.Today.Day);
            vm.PromedioPorDia = (int)Math.Round((double)vm.ServiciosEsteMes / diasTranscurridos);

            // Top 5 Empresas
            var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var finMes = DateTime.Today;

            var asistenciasMes = await _db.Registros
                .Where(r => r.Fecha >= inicioMes && r.Fecha <= finMes)
                .Include(r => r.Empresa)
                .GroupBy(r => r.Empresa)
                .Select(g => new { Empresa = g.Key, Conteos = g.Count() })
                .OrderByDescending(x => x.Conteos)
                .Take(5)
                .ToListAsync();

            var totalAsistencias = await _db.Registros.CountAsync(r => r.Fecha >= inicioMes && r.Fecha <= finMes);

            vm.TopEmpresas = asistenciasMes.Select((x, i) => new TopEmpresaItem
            {
                Ranking = i + 1,
                NombreEmpresa = x.Empresa?.Nombre ?? "-",
                TotalAsistencias = x.Conteos,
                Porcentaje = totalAsistencias > 0 ? Math.Round((decimal)x.Conteos / totalAsistencias * 100, 2) : 0
            }).ToList();

            return vm;
        }
    }
}
