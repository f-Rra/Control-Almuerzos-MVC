using System.ComponentModel.DataAnnotations;

namespace SCA_MVC.ViewModels
{
    /// <summary>
    /// Ítem de la tabla de usuarios: datos de solo lectura para mostrar en la lista.
    /// </summary>
    public class UsuarioListItem
    {
        public string Id { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }

    /// <summary>
    /// Datos del formulario de alta / edición de usuario.
    /// </summary>
    public class UsuarioFormViewModel
    {
        // Vacío en modo alta; contiene el Id de Identity en modo edición
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 30 caracteres.")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        // Obligatoria al crear; opcional al editar (dejar vacío = no cambiar contraseña)
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string? NuevaContrasena { get; set; }
    }

    /// <summary>
    /// ViewModel principal de la vista Usuario/Index.
    /// Agrupa la lista de usuarios, el formulario lateral y los metadatos de filtro.
    /// </summary>
    public class UsuarioViewModel
    {
        public List<UsuarioListItem> Usuarios { get; set; } = new();
        public UsuarioFormViewModel UsuarioActual { get; set; } = new();
        public string? Filtro { get; set; }
        public string? UsuarioSeleccionadoId { get; set; }
        public List<string> RolesDisponibles { get; set; } = new();
    }
}
