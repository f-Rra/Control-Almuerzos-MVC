using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SCA_MVC.Models;

namespace SCA_MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Dependencias

        private readonly ILogger<HomeController> _logger;
        private readonly IReporteNegocio _reporteNegocio;

        public HomeController(ILogger<HomeController> logger, IReporteNegocio reporteNegocio)
        {
            _logger         = logger;
            _reporteNegocio = reporteNegocio;
        }

        #endregion

        // =====================================================================

        #region Acciones Públicas

        public async Task<IActionResult> Index()
        {
            // Últimos 30 días para obtener suficientes servicios
            var desde = DateTime.Today.AddDays(-30);
            var hasta = DateTime.Today;

            var todos = await _reporteNegocio.ObtenerListaServiciosAsync(desde, hasta, null);

            // Todos los servicios más recientes primero
            var ultimos = todos
                .OrderByDescending(s => s.Fecha)
                .ThenByDescending(s => s.IdServicio)
                .ToList();

            var seleccionado = ultimos.FirstOrDefault();

            // Comparativa: total por día de la semana de los últimos 7 días
            var hace7 = DateTime.Today.AddDays(-6);
            var ultimaSemana = todos
                .Where(s => s.Fecha.Date >= hace7)
                .ToList();

            // Comparativa: total por día hábil (Lun–Vie) de los últimos 7 días
            // Los fines de semana se excluyen para que el panel ocupe menos espacio
            var diasSemana = new[] { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };
            var comparativa = new List<ComparativaDia>();

            for (int i = 0; i < 7; i++)
            {
                var fecha = hace7.AddDays(i);
                // Saltar sábados (DayOfWeek.Saturday) y domingos (DayOfWeek.Sunday)
                if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var total = ultimaSemana
                    .Where(s => s.Fecha.Date == fecha.Date)
                    .Sum(s => s.TotalGeneral);
                comparativa.Add(new ComparativaDia
                {
                    Dia   = diasSemana[(int)fecha.DayOfWeek == 0 ? 6 : (int)fecha.DayOfWeek - 1],
                    Total = total
                });
            }

            // Calcular referencia máxima para escalar barras
            int maxRef = comparativa.Max(c => c.Total);
            if (maxRef == 0) maxRef = 1;
            foreach (var c in comparativa) c.MaxRef = maxRef;

            var model = new DashboardViewModel
            {
                UltimosServicios     = ultimos,
                ServicioSeleccionado = seleccionado,
                ComparativaSemanal   = comparativa
            };

            return View(model);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        #endregion
    }
}
