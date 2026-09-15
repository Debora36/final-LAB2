using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using final_LAB2.Models.ViewModels;
 
namespace final_LAB2.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEmpleadoService _empleadoService;
 
        public AuthController(IUsuarioService usuarioService, IEmpleadoService empleadoService)
        {
            _usuarioService = usuarioService;
            _empleadoService = empleadoService;
        }
 
        [AllowAnonymous]
        // GET: /Auth/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
 
            // ValidarCredenciales ya descarta usuarios con Activo = false
            var usuario = _usuarioService.ValidarCredenciales(model.Username, model.Password);
 
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };
 
            if (!string.IsNullOrWhiteSpace(usuario.AvatarUrl))
            {
                claims.Add(new Claim("AvatarUrl", usuario.AvatarUrl));
            }
            var empleado = _empleadoService.ObtenerPorUsuarioId(usuario.Id);
            if (empleado != null)
            {
                claims.Add(new Claim("EmpleadoId", empleado.Id.ToString()));
            }
 
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);//se crea la identidad de claims para el usuario autenticado
            var authProperties = new AuthenticationProperties //configuración de la cookie de autenticación
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(3)//si el usuario selecciona "Recordarme", la cookie expira en 30 días, sino en 3 horas
            };
 
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
 
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
 
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);//cierra la sesión del usuario y elimina la cookie de autenticación
            return RedirectToAction("Login");
        }
 
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}