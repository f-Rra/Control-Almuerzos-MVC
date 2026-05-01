using System.Collections.Generic;

namespace SCA_MVC.ViewModels
{
    public class DashboardViewModel
    {
        public List<ServicioReporte>  UltimosServicios     { get; set; } = new();
        public ServicioReporte?       ServicioSeleccionado { get; set; }
        public List<ComparativaDia>   ComparativaSemanal   { get; set; } = new();

        public bool   HayServicioHoy  { get; set; }
        public bool   ServicioActivo  { get; set; }
        public int    RegistradosHoy  { get; set; }
        public string LugarHoy        { get; set; } = "";
    }

    public class ComparativaDia
    {
        public string Dia    { get; set; } = "";
        public int    Total  { get; set; }
        public int    MaxRef { get; set; }
    }
}
