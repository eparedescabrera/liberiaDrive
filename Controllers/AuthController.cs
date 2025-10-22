using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LiberiaDriveMVC.Services;
using System.Security.Cryptography;
using System.Text;
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

        // ===============================
        // LOGIN (GET)
        // ===============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ===============================
        // LOGIN (POST)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contrasena)
        {
            string query = @"SELECT u.*, r.NombreRol 
                             FROM Usuario u 
                             INNER JOIN Rol r ON u.IdRol = r.IdRol 
                             WHERE u.NombreUsuario = @NombreUsuario AND u.Estado = 1";

            var parametros = new Dictionary<string, object> { { "@NombreUsuario", nombreUsuario } };
            var dt = _db.EjecutarSPDataTable(query, parametros, true);

            if (dt.Rows.Count == 0)
            {
                ViewBag.Error = "Usuario no encontrado o inactivo.";
                return View();
            }

            var user = dt.Rows[0];
            string storedHash = user["ContrasenaHash"].ToString() ?? "";
            string inputHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(contrasena ?? "")));

            if (!string.Equals(storedHash, inputHash, StringComparison.Ordinal))
            {
                ViewBag.Error = "Contraseña incorrecta.";
                return View();
            }

            // Crear sesión
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user["NombreUsuario"].ToString()!),
                new Claim(ClaimTypes.Role, user["NombreRol"].ToString()!)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            string rol = user["NombreRol"].ToString();

if (rol == "Administrador")
    return RedirectToAction("Index", "Admin");  // 🔹 Ahora va al panel administrativo
else
    return RedirectToAction("Index", "Home");   // 🔹 Página pública para cliente
        }
        

        // ===============================
        // LOGOUT
        // ===============================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Cookies");
            return RedirectToAction("Login");
        }

        // ===============================
        // REGISTRARSE (GET)
        // ===============================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ===============================
        // REGISTRARSE (POST)
        // ===============================
        [HttpPost]
        public IActionResult Register(string nombreUsuario, string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "⚠️ Todos los campos son obligatorios.";
                return View();
            }

            // Verificar duplicados
            string checkQuery = "SELECT * FROM Usuario WHERE NombreUsuario = @Usuario OR Correo = @Correo";
            var checkParams = new Dictionary<string, object>
            {
                { "@Usuario", nombreUsuario },
                { "@Correo", correo }
            };
            var dt = _db.EjecutarSPDataTable(checkQuery, checkParams, true);
            if (dt.Rows.Count > 0)
            {
                ViewBag.Error = "⚠️ Ya existe un usuario con ese nombre o correo.";
                return View();
            }

            // Hashear contraseña
            string hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(contrasena)));

            // Rol fijo: Cliente
            string rolQuery = "SELECT TOP 1 IdRol FROM Rol WHERE NombreRol = 'Cliente'";
            var rolDt = _db.EjecutarSPDataTable(rolQuery, null, true);
            if (rolDt.Rows.Count == 0)
            {
                ViewBag.Error = "❌ No se encontró el rol 'Cliente'.";
                return View();
            }
            int idRolCliente = Convert.ToInt32(rolDt.Rows[0]["IdRol"]);

            string insert = @"
                INSERT INTO Usuario (NombreUsuario, Correo, ContrasenaHash, IdRol, Estado, FechaRegistro)
                VALUES (@Usuario, @Correo, @Hash, @IdRol, 1, GETDATE())";

            var parametros = new Dictionary<string, object>
            {
                { "@Usuario", nombreUsuario },
                { "@Correo", correo },
                { "@Hash", hash },
                { "@IdRol", idRolCliente }
            };

            _db.EjecutarSPNonQuery(insert, parametros, true);
            TempData["Success"] = "✅ Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
