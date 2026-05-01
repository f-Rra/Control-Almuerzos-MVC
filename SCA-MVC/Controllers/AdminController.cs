using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Models;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;

namespace SCA_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEmpresaNegocio _empresaNegocio;
        private readonly IEmpleadoNegocio _empleadoNegocio;
        private readonly IReporteNegocio _reporteNegocio;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            IEmpresaNegocio empresaNegocio,
            IEmpleadoNegocio empleadoNegocio,
            IReporteNegocio reporteNegocio,
            UserManager<ApplicationUser> userManager)
        {
            _empresaNegocio  = empresaNegocio;
            _empleadoNegocio = empleadoNegocio;
            _reporteNegocio  = reporteNegocio;
            _userManager     = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var hoy       = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            var empresas  = await _empresaNegocio.ListarAsync();
            var empleados = await _empleadoNegocio.ListarAsync();

            var vm = new AdminViewModel
            {
                EmpresasActivas  = empresas.Count(e => e.Estado),
                EmpleadosActivos = empleados.Count(e => e.Estado),
                AsistenciasMes   = await _reporteNegocio.ObtenerTotalAsistenciasRangoAsync(inicioMes, hoy, null),
                TotalUsuarios    = _userManager.Users.Count()
            };

            return View(vm);
        }
    }
}
