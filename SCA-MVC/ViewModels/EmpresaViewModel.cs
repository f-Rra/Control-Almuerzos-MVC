using SCA_MVC.Models;

namespace SCA_MVC.ViewModels
{
    public class EmpresaViewModel
    {
        public List<Empresa> Empresas { get; set; } = new();
        public Empresa EmpresaActual { get; set; } = new();
        public string? Filtro { get; set; }
        public int? EmpresaSeleccionadaId { get; set; }

        public int TotalEmpleados { get; set; }
        public int Inactivos { get; set; }
        public int AsistenciasMesActual { get; set; }
        public decimal PromedioDiario { get; set; }
    }
}