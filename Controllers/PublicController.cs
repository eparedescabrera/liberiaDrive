using Microsoft.AspNetCore.Mvc;

namespace LiberiaDriveMVC.Controllers
{
    public class PublicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cursos()
        {
            return View();
        }

        public IActionResult Contacto()
        {
            return View();
        }

        public IActionResult Nosotros()
        {
            return View();
        }
    }
}
