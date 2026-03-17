using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SCA_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        #region Acciones Públicas

        public IActionResult Index()
        {
            return View();
        }
        
        #endregion
    }
}
