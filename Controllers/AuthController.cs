using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LiberiaDriveMVC.Models;
using LiberiaDriveMVC.Models.ViewModels;
using LiberiaDriveMVC.Services;
using Microsoft.AspNetCore.Identity;

namespace LiberiaDriveMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ===============================
        // LOGIN (GET)
        // ===============================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // ===============================
        // LOGIN (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetActiveUserByUsernameAsync(model.NombreUsuario);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado o inactivo.");
                return View(model);
            }

            var verification = _userService.VerifyPassword(user, model.Contrasena);

            if (verification == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new(ClaimTypes.Name, user.NombreUsuario),
                new(ClaimTypes.Email, user.Correo),
                new(ClaimTypes.Role, user.IdRolNavigation.NombreRol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return user.IdRolNavigation.NombreRol switch
            {
                "Administrador" => RedirectToAction("Index", "Admin"),
                _ => RedirectToAction("Index", "Home")
            };
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
            return View(new RegisterViewModel());
        }

        // ===============================
        // REGISTRARSE (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _userService.IsUsernameOrEmailTakenAsync(model.NombreUsuario, model.Correo))
            {
                ModelState.AddModelError(string.Empty, "⚠️ Ya existe un usuario con ese nombre o correo.");
                return View(model);
            }

            try
            {
                await _userService.CreateClientUserAsync(model.NombreUsuario, model.Correo, model.Contrasena);
                TempData["Success"] = "✅ Registro exitoso. Ahora puedes iniciar sesión.";
                return RedirectToAction("Login");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "No se pudo crear el usuario por falta de rol configurado.");
                ModelState.AddModelError(string.Empty, "❌ No se encontró el rol 'Cliente'. Contacta al administrador.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar usuario {User}", model.NombreUsuario);
                ModelState.AddModelError(string.Empty, "❌ Ocurrió un error inesperado. Inténtalo nuevamente.");
            }

            return View(model);
        }

        public IActionResult AccessDenied() => View();
    }
}
