using SCA_MVC.Models;

namespace SCA_MVC.ViewModels
{
    public class EmpleadoViewModel
    {
        // Lista de empleados que se muestra en la tabla (resultado del filtro activo)
        public List<Empleado> Empleados { get; set; } = new();

        // Empleado seleccionado o en edición en el panel lateral
        public Empleado EmpleadoActual { get; set; } = new();

        // Filtros activos en la vista (se usan para mantener el estado al recargar)
        public string? Filtro { get; set; }
        public int? EmpresaFiltroId { get; set; }
        public int? EmpleadoSeleccionadoId { get; set; }

        // Lista de empresas activas para poblar el combo del formulario y el filtro
        public List<Empresa> Empresas { get; set; } = new();
    }
}
