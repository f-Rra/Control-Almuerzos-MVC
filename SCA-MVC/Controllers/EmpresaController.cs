using Microsoft.AspNetCore.Mvc;

namespace SCA_MVC.Controllers
{
    public class EmpresaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
