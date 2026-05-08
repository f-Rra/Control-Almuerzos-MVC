using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class ServicioNegocio : IServicioNegocio
    {
        private readonly ApplicationDbContext _context;

        public ServicioNegocio(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Servicio?> ObtenerActivoAsync(int idLugar)
        {
            return _context.Servicios.AsNoTracking().Include(s => s.Lugar)
                .FirstOrDefaultAsync(s => s.IdLugar == idLugar && s.DuracionMinutos == null);
        }

        public Task<Servicio?> ObtenerActivoGlobalAsync()
        {
            return _context.Servicios.AsNoTracking().Include(s => s.Lugar)
                .FirstOrDefaultAsync(s => s.DuracionMinutos == null);
        }

        public async Task<int> CrearServicioAsync(int idLugar, int? proyeccion, int invitados = 0)
        {
            var servicio = new Servicio
            {
                IdLugar = idLugar,
                Proyeccion = proyeccion,
                TotalInvitados = invitados,
                Fecha = DateTime.Today,
                HoraInicio = DateTime.UtcNow
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();
            return servicio.IdServicio;
        }

        public async Task FinalizarServicioAsync(int idServicio, int totalComensales, int totalInvitados, int? duracionMinutos)
        {
            var servicio = await _context.Servicios.FindAsync(idServicio);
            if (servicio != null)
            {
                servicio.TotalComensales = totalComensales;
                servicio.TotalInvitados = totalInvitados;
                servicio.DuracionMinutos = duracionMinutos;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> FinalizarPendientesAsync()
        {
            var pendientes = await _context.Servicios
                .Where(s => s.DuracionMinutos == null && s.Fecha.Date < DateTime.Today.Date)
                .ToListAsync();

            if (pendientes.Any())
            {
                foreach(var s in pendientes)
                {
                    s.DuracionMinutos = 120; // Default a 2 horas
                    s.TotalComensales = await _context.Registros.CountAsync(r => r.IdServicio == s.IdServicio);
                }
                await _context.SaveChangesAsync();
                return pendientes.Count;
            }
            return 0;
        }
    }
}