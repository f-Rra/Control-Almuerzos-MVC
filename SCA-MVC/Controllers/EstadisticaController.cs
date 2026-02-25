using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Services;
using System.Threading.Tasks;

namespace SCA_MVC.Controllers
{
    public class EstadisticaController : Controller
    {
        private readonly IEstadisticasNegocio _estadisticasNegocio;

        public EstadisticaController(IEstadisticasNegocio estadisticasNegocio)
        {
            _estadisticasNegocio = estadisticasNegocio;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _estadisticasNegocio.ObtenerTodosAsync();
            return View(model);
        }
    }
}
