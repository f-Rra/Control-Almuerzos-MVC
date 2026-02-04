using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCA_MVC.Models
{
    public class Servicio
    {
        [Key]
        public int IdServicio { get; set; }

        [Display(Name = "Lugar")]
        public int IdLugar { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Proyección")]
        public int? Proyeccion { get; set; }

        [Display(Name = "Duración (min)")]
        public int? DuracionMinutos { get; set; }

        [Display(Name = "Total Comensales")]
        public int TotalComensales { get; set; }

        [Display(Name = "Total Invitados")]
        public int TotalInvitados { get; set; }

        // Propiedades de navegación
        public Lugar? Lugar { get; set; }
        public ICollection<Registro> Registros { get; set; } = new List<Registro>();

        // Propiedades calculadas
        [NotMapped]
        [Display(Name = "Total General")]
        public int TotalGeneral => TotalComensales + TotalInvitados;

        [NotMapped]
        [Display(Name = "Estado")]
        public string Estado => DuracionMinutos == null ? "Activo" : "Finalizado";
    }
}
