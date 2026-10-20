using System.ComponentModel.DataAnnotations;

namespace SCA_MVC.Api
{
    // Lo que manda el lector: el UID de la tarjeta que acaban de acercarle
    public class RegistrarLecturaSolicitud
    {
        [Required(ErrorMessage = "La credencial es obligatoria.")]
        [StringLength(50, ErrorMessage = "La credencial no puede exceder 50 caracteres.")]
        public string Credencial { get; set; } = string.Empty;
    }

    // Lo que responde la API cuando registró: de quién es la tarjeta y dónde quedó anotada
    public class RegistroCreadoRespuesta
    {
        public int IdServicio { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public string Lugar { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }

        // HH:mm, hora de Argentina
        public string Hora { get; set; } = string.Empty;
    }
}
