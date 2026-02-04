using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCA_MVC.Models
{
    public class Registro
    {
        [Key]
        public int IdRegistro { get; set; }

        [Display(Name = "Empleado")]
        public int? IdEmpleado { get; set; }

        [Display(Name = "Empresa")]
        public int IdEmpresa { get; set; }

        [Display(Name = "Servicio")]
        public int IdServicio { get; set; }

        [Display(Name = "Lugar")]
        public int IdLugar { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora")]
        public TimeSpan Hora { get; set; }

        // Propiedades de navegación
        public Empleado? Empleado { get; set; }
        public Empresa? Empresa { get; set; }
        public Servicio? Servicio { get; set; }
        public Lugar? Lugar { get; set; }

        // Propiedad calculada
        [NotMapped]
        [Display(Name = "Hora Formateada")]
        public string HoraFormateada => Hora.ToString(@"hh\:mm");
    }
}
