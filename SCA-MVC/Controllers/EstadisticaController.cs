using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Services;
using System.Threading.Tasks;

namespace SCA_MVC.Controllers
{
    [Authorize]
    public class EstadisticaController : Controller
    {
        #region Dependencias

        private readonly IEstadisticasNegocio _estadisticasNegocio;

        public EstadisticaController(IEstadisticasNegocio estadisticasNegocio)
        {
            _estadisticasNegocio = estadisticasNegocio;
        }

        #endregion

        // =====================================================================

        #region Acciones Públicas

        public async Task<IActionResult> Index()
        {
            var model = await _estadisticasNegocio.ObtenerTodosAsync();
            return View(model);
        }

        #endregion
    }
}
