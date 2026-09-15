using final_LAB2.Models;
using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace final_LAB2.Controllers
{
    public class UsuarioController : Controller
    {
        private const int PageSize = 10;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmpleadoService _empleadoService;
        private readonly IFileService _fileService;

        public UsuarioController(IUsuarioService usuarioService, IEmpleadoService empleadoService, IFileService fileService)
        {
            _usuarioService = usuarioService;
            _empleadoService = empleadoService;
            _fileService = fileService;
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Disable(int id)
        {
            _usuarioService.DesactivarUsuario(id);
            TempData["SuccessMessage"] = "Usuario deshabilitado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        public IActionResult Edit(int id, bool desdePerfil = false)
        {
            var usuarioLogueadoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Admin puede editar cualquiera, el resto solo su propio perfil
            if (!User.IsInRole("Admin") && id != usuarioLogueadoId)
            {
                TempData["ErrorMessage"] = "No tenés permiso para editar este perfil.";
                return RedirectToAction("Index", "Home");
            }

            var usuario = _usuarioService.ObtenerPorId(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DesdePerfil = desdePerfil;
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario, string? nuevaPassword,
                        string? confirmarPassword, IFormFile? archivoAvatar)
        {
            var usuarioLogueadoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            bool desdePerfil = id == usuarioLogueadoId;

            if (!User.IsInRole("Admin") && id != usuarioLogueadoId)
            {
                TempData["ErrorMessage"] = "No tenés permiso para editar este perfil.";
                return RedirectToAction("Index", "Home");
            }

            if (id != usuario.Id) return NotFound();

            ModelState.Remove(nameof(Usuario.Password));

            if (!string.IsNullOrEmpty(nuevaPassword) && nuevaPassword != confirmarPassword)
                ModelState.AddModelError("confirmarPassword", "Las contraseñas no coinciden.");

            if (!ModelState.IsValid)
            {
                ViewBag.DesdePerfil = desdePerfil;
                return View(usuario);
            }

            try
            {
                string? nuevoAvatarUrl = null;
                
                // delego la tarea de los archivos
                if (archivoAvatar != null && archivoAvatar.Length > 0)
                {
                    var usuarioOriginal = _usuarioService.ObtenerPorId(id);
                    if (usuarioOriginal != null)
                    {
                        _fileService.EliminarArchivo(usuarioOriginal.AvatarUrl);
                    }
                    nuevoAvatarUrl = _fileService.GuardarArchivo(archivoAvatar, "Avatars");
                }

                _usuarioService.ActualizarDatos(usuario, nuevoAvatarUrl);

                if (desdePerfil && !string.IsNullOrEmpty(nuevaPassword))
                    _usuarioService.CambiarPassword(id, nuevaPassword);

                TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                return desdePerfil ? RedirectToAction("Index", "Home") : RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.DesdePerfil = desdePerfil;
                return View(usuario);
            }
        }

    }
}
