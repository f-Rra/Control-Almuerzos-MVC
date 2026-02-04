using System.ComponentModel.DataAnnotations;

namespace SCA_MVC.Models
{
    public class Lugar
    {
        [Key]
        public int IdLugar { get; set; }

        [Required(ErrorMessage = "El nombre del lugar es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        [Display(Name = "Lugar")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        // Propiedades de navegación
        public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
        public ICollection<Registro> Registros { get; set; } = new List<Registro>();
    }
}
