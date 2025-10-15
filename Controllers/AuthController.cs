using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LiberiaDriveMVC.Services;
using System.Collections.Generic;
using System.Data;

namespace LiberiaDriveMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseService _db;

        public AuthController(DatabaseService db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contrasena)
        {
            // Buscar usuario
            string query = "SELECT u.*, r.NombreRol FROM Usuario u INNER JOIN Rol r ON u.IdRol = r.IdRol WHERE u.NombreUsuario = @NombreUsuario AND u.Estado = 1";
            var parametros = new Dictionary<string, object> { { "@NombreUsuario", nombreUsuario } };
            var dt = _db.EjecutarSPDataTable(query, parametros, true);

            if (dt.Rows.Count == 0)
            {
                ViewBag.Error = "Usuario no encontrado o inactivo.";
                return View();
            }

            var user = dt.Rows[0];
            string hash = user["ContrasenaHash"].ToString();

            // Comparar contraseñas (si están hasheadas, usar verificación)
            if (contrasena != hash) // ⚠️ temporal, luego se cambia por verificación hash
            {
                ViewBag.Error = "Contraseña incorrecta.";
                return View();
            }

            // Crear identidad
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user["NombreUsuario"].ToString()),
                new Claim(ClaimTypes.Role, user["NombreRol"].ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
