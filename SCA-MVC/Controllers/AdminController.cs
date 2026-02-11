using Microsoft.AspNetCore.Mvc;

namespace SCA_MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
