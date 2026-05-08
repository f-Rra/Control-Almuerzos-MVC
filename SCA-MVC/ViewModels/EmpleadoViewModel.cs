using SCA_MVC.Models;

namespace SCA_MVC.ViewModels
{
    public class EmpleadoViewModel
    {
        public const int PageSize = 10;

        public List<Empleado> Empleados { get; set; } = new();
        public Empleado EmpleadoActual { get; set; } = new();

        public string? Filtro { get; set; }
        public int? EmpresaFiltroId { get; set; }
        public int? EmpleadoSeleccionadoId { get; set; }

        public List<Empresa> Empresas { get; set; } = new();

        public int PaginaActual { get; set; } = 1;
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / PageSize);
    }
}
