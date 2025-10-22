using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiberiaDriveMVC.Controllers
{
    [Authorize(Roles = "Administrador")] // 🔒 Solo admin
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Panel Administrativo - LiberiaDrive";
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Estadísticas - Panel Admin";
            return View();
        }
    }
}
