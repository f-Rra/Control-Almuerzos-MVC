using System.ComponentModel.DataAnnotations;

namespace SCA_MVC.Models
{
    public class Empresa
    {
        [Key]
        public int IdEmpresa { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Empresa")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        public int CantidadEmpleados { get; set; }

        // Propiedades de navegación
        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
        public ICollection<Registro> Registros { get; set; } = new List<Registro>();
    }
}
