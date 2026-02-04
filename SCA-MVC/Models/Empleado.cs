using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCA_MVC.Models
{
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La credencial RFID es obligatoria")]
        [StringLength(50, ErrorMessage = "La credencial no puede exceder 50 caracteres")]
        [Display(Name = "Credencial RFID")]
        public string IdCredencial { get; set; } = string.Empty;

        [Display(Name = "Empresa")]
        public int IdEmpresa { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        // Propiedades de navegación
        public Empresa? Empresa { get; set; }
        public ICollection<Registro> Registros { get; set; } = new List<Registro>();

        // Propiedad calculada
        [NotMapped]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
