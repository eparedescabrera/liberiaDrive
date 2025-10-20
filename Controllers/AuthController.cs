using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LiberiaDriveMVC.Services;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;  // ✅ para SHA256
using System.Text;                  // ✅ para Encoding

namespace LiberiaDriveMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseService _db;

        public AuthController(DatabaseService db)
        {
            _db = db;
        }

[HttpGet]
public IActionResult Login()
{
    return View();
}






        [HttpPost]
public async Task<IActionResult> Login(string nombreUsuario, string contrasena)
{
    // 1) Buscar usuario activo
    string query = @"SELECT u.*, r.NombreRol 
                     FROM Usuario u 
                     INNER JOIN Rol r ON u.IdRol = r.IdRol 
                     WHERE u.NombreUsuario = @NombreUsuario AND u.Estado = 1";
    var parametros = new Dictionary<string, object> { { "@NombreUsuario", nombreUsuario } };
    var dt = _db.EjecutarSPDataTable(query, parametros, isSql: true);

    if (dt.Rows.Count == 0)
    {
        ViewBag.Error = "Usuario no encontrado o inactivo.";
        return View();
    }

    var user = dt.Rows[0];
    string storedHash = user["ContrasenaHash"]?.ToString() ?? "";

    // 2) Hashear lo que el usuario escribió (igual que en ResetPassword)
    string inputHash = Convert.ToBase64String(
        SHA256.HashData(Encoding.UTF8.GetBytes(contrasena ?? ""))
    );

    // 3) Comparar hash vs hash (no texto)
    if (!string.Equals(storedHash, inputHash, StringComparison.Ordinal))
    {
        ViewBag.Error = "Contraseña incorrecta.";
        return View();
    }

    // 4) Construir identidad y autenticar
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user["NombreUsuario"].ToString()!),
        new Claim(ClaimTypes.Role, user["NombreRol"].ToString()!)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return RedirectToAction("Index", "Home");
}
        // ===============================
        // LOGOUT
        // ===============================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ===============================
        // RECUPERAR CONTRASEÑA - GET
        // ===============================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // ===============================
        // RECUPERAR CONTRASEÑA - POST
        // ===============================
        [HttpPost]
        public IActionResult ForgotPassword(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                ViewBag.Error = "⚠️ Debes ingresar un correo electrónico.";
                return View();
            }

            string query = "SELECT * FROM Usuario WHERE Correo = @Correo";
            var parametros = new Dictionary<string, object> { { "@Correo", correo } };
            var dt = _db.EjecutarSPDataTable(query, parametros, true);

            if (dt.Rows.Count == 0)
            {
                ViewBag.Error = "❌ No existe ninguna cuenta con ese correo.";
                return View();
            }

            // Generar token temporal
            string token = Guid.NewGuid().ToString();
            DateTime expira = DateTime.Now.AddMinutes(15);

            string insertToken = @"
                INSERT INTO TokenRecuperacion (Correo, Token, FechaExpira)
                VALUES (@Correo, @Token, @FechaExpira)";

            var parametrosToken = new Dictionary<string, object>
            {
                { "@Correo", correo },
                { "@Token", token },
                { "@FechaExpira", expira }
            };

            // ✅ Usamos 'true' para ejecutar SQL directo
            _db.EjecutarSPNonQuery(insertToken, parametrosToken, true);

            // 🔹 Por ahora mostramos el token en pantalla
            ViewBag.Mensaje = $"✅ Se ha generado un enlace temporal. Copie este token: <b>{token}</b>";
            ViewBag.Correo = correo;
            return View();
        }

        // ===============================
        // RESTABLECER CONTRASEÑA - GET
        // ===============================
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login");

            ViewBag.Token = token;
            return View();
        }

        // ===============================
        // RESTABLECER CONTRASEÑA - POST
        // ===============================
        [HttpPost]
        public IActionResult ResetPassword(string token, string nuevaContrasena, string confirmarContrasena)
        {
            if (string.IsNullOrWhiteSpace(nuevaContrasena) || string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                ViewBag.Error = "⚠️ Todos los campos son obligatorios.";
                ViewBag.Token = token;
                return View();
            }

            if (nuevaContrasena != confirmarContrasena)
            {
                ViewBag.Error = "⚠️ Las contraseñas no coinciden.";
                ViewBag.Token = token;
                return View();
            }

            string query = @"
                SELECT * FROM TokenRecuperacion 
                WHERE Token = @Token AND Usado = 0 AND FechaExpira > GETDATE()";

            var parametros = new Dictionary<string, object> { { "@Token", token } };
            var dt = _db.EjecutarSPDataTable(query, parametros, true);

            if (dt.Rows.Count == 0)
            {
                ViewBag.Error = "❌ Token inválido o expirado.";
                return View();
            }

            string correo = dt.Rows[0]["Correo"].ToString();

            // 🔒 Hashear nueva contraseña
            string hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(nuevaContrasena)));

            // Actualizar contraseña
            string update = "UPDATE Usuario SET ContrasenaHash = @ContrasenaHash WHERE Correo = @Correo";
            var parametrosUpdate = new Dictionary<string, object>
            {
                { "@ContrasenaHash", hash },
                { "@Correo", correo }
            };
            _db.EjecutarSPNonQuery(update, parametrosUpdate, true); // ✅ SQL directo

            // Marcar token como usado
            string updateToken = "UPDATE TokenRecuperacion SET Usado = 1 WHERE Token = @Token";
            _db.EjecutarSPNonQuery(updateToken, parametros, true); // ✅ SQL directo

            TempData["Success"] = "✅ Contraseña actualizada correctamente. Inicia sesión.";
            return RedirectToAction("Login");
        }

        // ===============================
        // ACCESO DENEGADO
        // ===============================
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
