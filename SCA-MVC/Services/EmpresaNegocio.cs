using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class EmpresaNegocio : IEmpresaNegocio
    {
        private readonly ApplicationDbContext _context;

        public EmpresaNegocio(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Empresa>> ListarAsync()
        {
            return _context.Empresas.AsNoTracking().ToListAsync();
        }

        public async Task<List<Empresa>> ListarConEmpleadosAsync()
        {
            var empresas = await _context.Empresas
                .AsNoTracking()
                .Include(e => e.Empleados.Where(emp => emp.Estado))
                .ToListAsync();
                
            foreach (var emp in empresas)
            {
                emp.CantidadEmpleados = emp.Empleados.Count;
            }
            return empresas;
        }

        public Task<Empresa?> BuscarPorIdAsync(int idEmpresa)
        {
            return _context.Empresas.AsNoTracking().FirstOrDefaultAsync(e => e.IdEmpresa == idEmpresa);
        }

        public async Task<int> AgregarAsync(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return empresa.IdEmpresa;
        }

        public async Task ModificarAsync(Empresa empresa)
        {
            var entity = await _context.Empresas.FindAsync(empresa.IdEmpresa);
            if (entity != null)
            {
                entity.Nombre = empresa.Nombre;
                entity.Estado = empresa.Estado;
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarAsync(int idEmpresa)
        {
            var entity = await _context.Empresas.FindAsync(idEmpresa);
            if (entity != null)
            {
                entity.Estado = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Empresa>> FiltrarAsync(string? filtro)
        {
            var query = _context.Empresas.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(e => e.Nombre.Contains(filtro));
            }
            var empresas = await query
                .Include(e => e.Empleados.Where(emp => emp.Estado))
                .ToListAsync();

            foreach (var emp in empresas)
            {
                emp.CantidadEmpleados = emp.Empleados.Count;
            }
            return empresas;
        }
    }
}