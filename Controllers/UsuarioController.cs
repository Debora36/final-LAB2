using final_LAB2.Models;
using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_LAB2.Controllers
{
    // Solo el Admin gestiona usuarios: cubre el requisito de "funcionalidad restringida por rol"
    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        private const int PageSize = 10;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            if (pageIndex < 1) pageIndex = 1;

            var (items, totalCount) = _usuarioService.ObtenerPaginado(pageIndex, PageSize);

            var modelo = new PaginatedListViewModel<Usuario>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Disable(int id)
        {
            _usuarioService.DesactivarUsuario(id);
            TempData["SuccessMessage"] = "Usuario deshabilitado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario, string confirmPassword)
        {
            if (usuario.Password != confirmPassword)
            {
                ModelState.AddModelError(nameof(confirmPassword), "Las contraseñas no coinciden.");
            }
 
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
 
            try
            {
                // RegistrarUsuario hashea la contraseña y valida que el username no exista
                _usuarioService.RegistrarUsuario(usuario, usuario.Password);
                TempData["SuccessMessage"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(usuario);
            }
        }
 
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var usuario = _usuarioService.ObtenerPorId(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }
 
            return View(usuario);
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario, string? nuevaPassword, string? confirmarPassword)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }
 
            // El form no pide Password: se cambia aparte con nuevaPassword/confirmarPassword
            ModelState.Remove(nameof(Usuario.Password));
 
            if (!string.IsNullOrEmpty(nuevaPassword) && nuevaPassword != confirmarPassword)
            {
                ModelState.AddModelError(nameof(confirmarPassword), "Las contraseñas no coinciden.");
            }
 
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
 
            var usuarioActual = _usuarioService.ObtenerPorId(id);
            if (usuarioActual == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }
 
            // Activo se maneja únicamente desde Disable, nunca desde este formulario
            usuario.Activo = usuarioActual.Activo;
            usuario.Password = usuarioActual.Password; // valor irrelevante: Actualizar() no lo escribe
 
            _usuarioService.ActualizarDatos(usuario);
 
            if (!string.IsNullOrEmpty(nuevaPassword))
            {
                _usuarioService.CambiarPassword(id, nuevaPassword);
            }
 
            TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
