using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class RegistroNegocio : IRegistroNegocio
    {
        private readonly ApplicationDbContext _context;

        public RegistroNegocio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(int idEmpleado, int idEmpresa, int idServicio, int idLugar)
        {
            var registro = new Registro
            {
                IdEmpleado = idEmpleado,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdLugar = idLugar,
                Fecha = DateTime.Today,
                Hora = DateTime.Now.TimeOfDay
            };

            _context.Registros.Add(registro);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Registro>> ListarPorServicioAsync(int idServicio)
        {
            var registros = await _context.Registros
                .Include(r => r.Empleado)
                .ThenInclude(e => e.Empresa)
                .Where(r => r.IdServicio == idServicio)
                .ToListAsync();
                
            foreach (var r in registros)
            {
                r.NombreEmpleado = r.Empleado != null ? $"{r.Empleado.Nombre} {r.Empleado.Apellido}" : "-";
                r.NombreEmpresa = r.Empleado?.Empresa?.Nombre ?? "-";
            }
            return registros;
        }

        public Task<bool> YaRegistradoAsync(int idEmpleado, int idServicio)
        {
            return _context.Registros
                .AnyAsync(r => r.IdEmpleado == idEmpleado && r.IdServicio == idServicio);
        }

        public Task<int> ContarAsync(int idServicio)
        {
            return _context.Registros.CountAsync(r => r.IdServicio == idServicio);
        }

        public async Task<List<Registro>> PorEmpresaYFechaAsync(int idEmpresa, DateTime fechaInicio, DateTime fechaFin)
        {
            var registros = await _context.Registros
                .Include(r => r.Empleado)
                .Include(r => r.Empresa)
                .Where(r => r.IdEmpresa == idEmpresa && r.Fecha >= fechaInicio.Date && r.Fecha <= fechaFin.Date)
                .ToListAsync();

            foreach (var r in registros)
            {
                r.NombreEmpleado = r.Empleado != null ? $"{r.Empleado.Nombre} {r.Empleado.Apellido}" : "-";
                r.NombreEmpresa = r.Empleado?.Empresa?.Nombre ?? "-";
            }
            return registros;
        }
    }
}