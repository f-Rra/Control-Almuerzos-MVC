namespace SCA_MVC.ViewModels
{
    public class EstadisticasViewModel
    {
        // ── Empleados ──────────────────────────────────────────
        public int TotalEmpleados   { get; set; }
        public int EmpleadosActivos { get; set; }
        public int EmpleadosInactivos { get; set; }

        // ── Empresas ───────────────────────────────────────────
        public int TotalEmpresasActivas   { get; set; }
        public int EmpresasConEmpleados   { get; set; }
        public decimal PromedioEmpleados  { get; set; }

        // ── Servicios ──────────────────────────────────────────
        public int ServiciosEsteMes   { get; set; }
        public int ServiciosEsteAnio  { get; set; }
        public int PromedioPorDia     { get; set; }

        // ── Top Empresas ───────────────────────────────────────
        public List<TopEmpresaItem> TopEmpresas { get; set; } = new();
    }

    public class TopEmpresaItem
    {
        public int     Ranking          { get; set; }
        public string  NombreEmpresa    { get; set; } = "";
        public int     TotalAsistencias { get; set; }
        public decimal Porcentaje       { get; set; }
    }
}
