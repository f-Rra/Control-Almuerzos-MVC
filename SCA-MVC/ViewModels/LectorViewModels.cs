using System.ComponentModel.DataAnnotations;
using SCA_MVC.Models;

namespace SCA_MVC.ViewModels
{
    // Lo que muestra la pantalla de lectores: la lista y el formulario de alta
    public class LectoresViewModel
    {
        public List<Lector> Lectores { get; set; } = new();
        public List<Lugar> Lugares { get; set; } = new();
        public LectorFormViewModel Nuevo { get; set; } = new();
    }

    // El formulario de alta: cómo se llama el lector y dónde está instalado
    public class LectorFormViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Elegí el lugar donde está instalado.")]
        [Display(Name = "Lugar")]
        public int? IdLugar { get; set; }
    }

    // La pantalla que sigue al alta: es la única vez que se ve la clave
    public class LectorCreadoViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Lugar { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }
}
