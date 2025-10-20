using Microsoft.AspNetCore.Mvc;

namespace LiberiaDriveMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Inicio - Liberia Drive";
            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = "Quiénes Somos";
            return View();
        }

        public IActionResult Services()
        {
            ViewData["Title"] = "Servicios";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Contáctenos";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Política de Privacidad";
            return View();
        }
    }
}
