using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class LugarNegocio : ILugarNegocio
    {
        private readonly ApplicationDbContext _context;

        public LugarNegocio(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Lugar>> ListarAsync()
        {
            return _context.Lugares.AsNoTracking().ToListAsync();
        }

        public Task<Lugar?> BuscarPorNombreAsync(string nombre)
        {
            return _context.Lugares.AsNoTracking().FirstOrDefaultAsync(l => l.Nombre == nombre);
        }
    }
}