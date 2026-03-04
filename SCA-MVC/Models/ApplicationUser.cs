using Microsoft.AspNetCore.Identity;

namespace SCA_MVC.Models
{
    /// <summary>
    /// Extiende IdentityUser con propiedades adicionales del dominio.
    /// IdentityUser ya provee: Id (string/GUID), UserName, Email,
    /// PasswordHash, PhoneNumber, LockoutEnd, AccessFailedCount, etc.
    /// Aquí agregamos Nombre y Apellido para mostrar el nombre completo
    /// del operador en el topbar y en los reportes de auditoría.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>Nombre de pila del operador del sistema.</summary>
        [PersonalData]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Apellido del operador del sistema.</summary>
        [PersonalData]
        public string Apellido { get; set; } = string.Empty;

        /// <summary>Nombre completo calculado (no mapeado a columna).</summary>
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    }
}
