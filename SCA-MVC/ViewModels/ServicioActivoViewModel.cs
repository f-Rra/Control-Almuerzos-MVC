using SCA_MVC.Models;
using System.Collections.Generic;

namespace SCA_MVC.ViewModels
{
    public class ServicioActivoViewModel
    {
        // El servicio actualmente en curso (null si no hay uno)
        public Servicio? ServicioActivo { get; set; }

        // Datos para el formulario de inicio de servicio
        public int IdLugarSeleccionado { get; set; }
        public int ProyeccionEstablecida { get; set; }

        // Listas para combos y visualización
        public List<Lugar> LugaresDisponibles { get; set; } = new List<Lugar>();
        public List<Registro> UltimosRegistros { get; set; } = new List<Registro>();

        // Estadísticas rápidas para el panel de estado
        public int RegistradosHoy { get; set; }
        public int PendientesHoy { get; set; }
        public double PorcentajeCobertura { get; set; }
    }
}
