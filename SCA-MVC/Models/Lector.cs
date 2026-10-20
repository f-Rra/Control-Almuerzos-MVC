using System.ComponentModel.DataAnnotations;

namespace SCA_MVC.Models
{
    // Un lector RFID de red que se identifica contra la API con una clave propia. La clave
    // no se guarda: solo su hash, así que ni mirando la base se puede recuperar.
    public class Lector
    {
        [Key]
        public int IdLector { get; set; }

        [Required(ErrorMessage = "El nombre del lector es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        // Dónde está instalado: el lector registra siempre en el servicio activo de su lugar
        public int IdLugar { get; set; }

        // SHA-256 de la clave, en hexadecimal (64 caracteres)
        [Required]
        [StringLength(64)]
        public string ApiKeyHash { get; set; } = string.Empty;

        // Un lector revocado conserva su fila (queda el historial) pero ya no se autentica
        public bool Activo { get; set; }

        // Hora de Argentina, como el resto de las fechas que se guardan
        public DateTime FechaAlta { get; set; }

        public Lugar? Lugar { get; set; }
    }
}
