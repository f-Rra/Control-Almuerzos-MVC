using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class EmpleadoNegocio : IEmpleadoNegocio
    {
        private readonly ApplicationDbContext _context;

        public EmpleadoNegocio(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Empleado>> ListarAsync()
        {
            return _context.Empleados.Include(e => e.Empresa).ToListAsync();
        }

        public Task<Empleado?> BuscarPorCredencialAsync(string idCredencial)
        {
            return _context.Empleados.Include(e => e.Empresa)
                .FirstOrDefaultAsync(e => e.IdCredencial == idCredencial);
        }

        public Task<Empleado?> BuscarPorIdAsync(int idEmpleado)
        {
            return _context.Empleados.Include(e => e.Empresa)
                .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);
        }

        public async Task<bool> ExisteCredencialAsync(string idCredencial)
        {
            return await _context.Empleados.AnyAsync(e => e.IdCredencial == idCredencial);
        }

        public async Task<int> AgregarAsync(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();
            return empleado.IdEmpleado;
        }

        public async Task ModificarAsync(Empleado empleado)
        {
            var entity = await _context.Empleados.FindAsync(empleado.IdEmpleado);
            if (entity != null)
            {
                entity.IdCredencial = empleado.IdCredencial;
                entity.Nombre = empleado.Nombre;
                entity.Apellido = empleado.Apellido;
                entity.IdEmpresa = empleado.IdEmpresa;
                entity.Estado = empleado.Estado;
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarAsync(int idEmpleado)
        {
            var entity = await _context.Empleados.FindAsync(idEmpleado);
            if (entity != null)
            {
                entity.Estado = false;
                await _context.SaveChangesAsync();
            }
        }

        public Task<List<Empleado>> FiltrarEmpleadosAsync(string? filtro, int? idEmpresa)
        {
            var query = _context.Empleados.Include(e => e.Empresa).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(e => e.Nombre.Contains(filtro) || 
                                         e.Apellido.Contains(filtro) || 
                                         e.IdCredencial.Contains(filtro));
            }

            if (idEmpresa.HasValue && idEmpresa.Value > 0)
            {
                query = query.Where(e => e.IdEmpresa == idEmpresa.Value);
            }

            return query.ToListAsync();
        }

        public async Task<List<Empleado>> EmpleadosSinAlmorzarAsync(int idServicio)
        {
            var registrados = _context.Registros
                .Where(r => r.IdServicio == idServicio && r.IdEmpleado != null)
                .Select(r => r.IdEmpleado);

            return await _context.Empleados
                .Include(e => e.Empresa)
                .Where(e => e.Estado && !registrados.Contains(e.IdEmpleado))
                .ToListAsync();
        }

        public async Task<List<Empleado>> FiltrarSinAlmorzarAsync(int idServicio, int? idEmpresa, string? nombre)
        {
            var registrados = _context.Registros
                .Where(r => r.IdServicio == idServicio && r.IdEmpleado != null)
                .Select(r => r.IdEmpleado);

            var query = _context.Empleados
                .Include(e => e.Empresa)
                .Where(e => e.Estado && !registrados.Contains(e.IdEmpleado));

            if (idEmpresa.HasValue && idEmpresa.Value > 0)
            {
                query = query.Where(e => e.IdEmpresa == idEmpresa.Value);
            }

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Apellido.Contains(nombre));
            }

            return await query.ToListAsync();
        }
    }
}