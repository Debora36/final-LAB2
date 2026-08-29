using final_LAB2.Models;
using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_LAB2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmpleadoController : Controller
    {
        private const int PageSize = 10;
        private readonly IEmpleadoService _empleadoService;

        public EmpleadoController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            if (pageIndex < 1) pageIndex = 1;

            var (items, totalCount) = _empleadoService.ObtenerPaginado(pageIndex, PageSize);

            var modelo = new PaginatedListViewModel<Empleado>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            return View(modelo);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.UsuariosElegibles = _empleadoService.ObtenerUsuariosElegibles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UsuariosElegibles = _empleadoService.ObtenerUsuariosElegibles();
                return View(empleado);
            }

            _empleadoService.Crear(empleado);
            TempData["SuccessMessage"] = "Empleado creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var empleado = _empleadoService.ObtenerPorId(id);
            if (empleado == null)
            {
                TempData["ErrorMessage"] = "Empleado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UsuariosElegibles = _empleadoService.ObtenerUsuariosElegibles(empleado.UsuarioId ?? 0);
            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UsuariosElegibles = _empleadoService.ObtenerUsuariosElegibles(empleado.UsuarioId ?? 0);
                return View(empleado);
            }

            var empleadoActual = _empleadoService.ObtenerPorId(id);
            if (empleadoActual == null)
            {
                TempData["ErrorMessage"] = "Empleado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Activo se maneja únicamente desde Disable, nunca desde este formulario
            empleado.Activo = empleadoActual.Activo;
            _empleadoService.Actualizar(empleado);

            TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Disable(int id)
        {
            _empleadoService.Desactivar(id);
            TempData["SuccessMessage"] = "Empleado deshabilitado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        
    }
}
