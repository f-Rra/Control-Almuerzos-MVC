using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Helpers;
using SCA_MVC.Models;
using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class RegistroNegocio : IRegistroNegocio
    {
        private readonly ApplicationDbContext _context;
        private readonly TimeProvider _reloj;

        public RegistroNegocio(ApplicationDbContext context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        public async Task<ResultadoRegistro> RegistrarAsync(int idEmpleado, int idEmpresa, int idServicio, int idLugar)
        {
            // El servicio y el lugar llegan del navegador, con lo que tenía la pantalla al
            // cargarse. Si el servicio ya se cerró mientras tanto (por ejemplo, se
            // auto-finalizó al cerrar el navegador), no se puede seguir registrando en él.
            // Activo es lo mismo que en ServicioNegocio: sin duración todavía.
            var servicio = await _context.Servicios
                .AsNoTracking()
                .Where(s => s.IdServicio == idServicio)
                .Select(s => new { s.IdLugar, s.DuracionMinutos })
                .FirstOrDefaultAsync();

            if (servicio == null)
                return ResultadoRegistro.ServicioInexistente;

            if (servicio.DuracionMinutos != null)
                return ResultadoRegistro.ServicioFinalizado;

            if (servicio.IdLugar != idLugar)
                return ResultadoRegistro.LugarIncorrecto;

            // Usamos SQL directo para evitar conflicto con triggers de SQL Server.
            // EF Core genera INSERT...OUTPUT para recuperar el PK generado; si la tabla
            // tiene triggers activos, SQL Server lanza una excepción (OUTPUT clause con triggers).
            // Con ExecuteSqlRawAsync el INSERT se envía directamente sin que EF Core
            // intente verificar filas afectadas ni recuperar el ID generado.
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO Registros (IdEmpleado, IdEmpresa, IdServicio, IdLugar, Fecha, Hora)
                      VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                    idEmpleado, idEmpresa, idServicio, idLugar,
                    _reloj.Hoy(), _reloj.Ahora().TimeOfDay);
                return ResultadoRegistro.Registrado;
            }
            catch (DbException)
            {
                // Si el lector manda la misma credencial dos veces seguidas, las dos
                // lecturas pasan el chequeo de YaRegistradoAsync antes de que la primera
                // inserte, y el índice único IX_Registro_Empleado_Servicio rechaza la
                // segunda. No se mira el número de error de SQL Server para que valga
                // con cualquier base: si el empleado ya figura registrado, fue eso; si
                // no, es otro problema y tiene que seguir saliendo como error.
                if (await YaRegistradoAsync(idEmpleado, idServicio))
                    return ResultadoRegistro.YaRegistrado;

                throw;
            }
        }

        public async Task<List<Registro>> ListarPorServicioAsync(int idServicio)
        {
            var registros = await _context.Registros
                .AsNoTracking()
                .Include(r => r.Empleado)
                .ThenInclude(e => e!.Empresa)
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
                .AsNoTracking()
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