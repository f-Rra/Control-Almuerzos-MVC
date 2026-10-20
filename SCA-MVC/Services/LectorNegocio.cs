using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;
using SCA_MVC.Helpers;
using SCA_MVC.Models;

namespace SCA_MVC.Services
{
    public class LectorNegocio : ILectorNegocio
    {
        private readonly ApplicationDbContext _context;
        private readonly TimeProvider _reloj;

        public LectorNegocio(ApplicationDbContext context, TimeProvider reloj)
        {
            _context = context;
            _reloj = reloj;
        }

        // SHA-256 y no un hash lento como el de las contraseñas (BCrypt, PBKDF2): esos existen
        // porque una contraseña humana se puede adivinar probando millones de combinaciones.
        // Esta clave son 32 bytes al azar, imposibles de adivinar, y un hash rápido y sin sal
        // permite buscar al lector directo por el índice en lugar de probar uno por uno.
        public static string HashDe(string clave) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(clave))).ToLowerInvariant();

        public async Task<(Lector Lector, string Clave)> CrearAsync(string nombre, int idLugar)
        {
            if (!await _context.Lugares.AnyAsync(l => l.IdLugar == idLugar))
                throw new InvalidOperationException("El lugar no existe.");

            // El prefijo no agrega seguridad: sirve para reconocer de qué es la clave si
            // alguien la deja pegada en un archivo o en un chat
            var clave = "sca_" + WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

            var lector = new Lector
            {
                Nombre = nombre.Trim(),
                IdLugar = idLugar,
                ApiKeyHash = HashDe(clave),
                Activo = true,
                FechaAlta = _reloj.Ahora()
            };

            _context.Lectores.Add(lector);
            await _context.SaveChangesAsync();
            return (lector, clave);
        }

        public async Task<Lector?> AutenticarAsync(string? clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                return null;

            var hash = HashDe(clave.Trim());
            return await _context.Lectores
                .AsNoTracking()
                .Include(l => l.Lugar)
                .FirstOrDefaultAsync(l => l.ApiKeyHash == hash && l.Activo);
        }

        public Task<List<Lector>> ListarAsync() =>
            _context.Lectores.AsNoTracking().Include(l => l.Lugar).OrderBy(l => l.Nombre).ToListAsync();

        public async Task<bool> RevocarAsync(int idLector)
        {
            var lector = await _context.Lectores.FindAsync(idLector);
            if (lector == null)
                return false;

            lector.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
