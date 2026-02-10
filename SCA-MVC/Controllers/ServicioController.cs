using Microsoft.AspNetCore.Mvc;

namespace SCA_MVC.Controllers
{
    public class ServicioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
