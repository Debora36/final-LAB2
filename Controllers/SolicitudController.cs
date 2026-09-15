using final_LAB2.Models;
using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace final_LAB2.Controllers
{
    [Authorize(Roles = "Admin,Tecnico,Empleado")]
    public class SolicitudController : Controller
    {
        private const int PageSize = 10;
        private readonly ISolicitudService _solicitudService;
        private readonly ICategoriaService _categoriaService;
        private readonly IEmpleadoService _empleadoService;
        private readonly IEquipoService _equipoService;

        public SolicitudController(ISolicitudService solicitudService, ICategoriaService categoriaService, IEmpleadoService empleadoService, IEquipoService equipoService)
        {
            _solicitudService = solicitudService;
            _categoriaService = categoriaService;
            _empleadoService = empleadoService;
            _equipoService = equipoService;
        }

        public IActionResult Index(int pageIndex = 1, string? estado = null)
        {
            if (pageIndex < 1) pageIndex = 1;

            List<Solicitud> items;
            int totalCount;

            if (User.IsInRole("Admin") || User.IsInRole("Tecnico"))
            {
                (items, totalCount) = _solicitudService.ObtenerPaginado(pageIndex, PageSize, estado);
            }
            else // Rol Empleado
            {
                var empleadoId = ObtenerEmpleadoIdLogueado();
                if (empleadoId == null)
                {
                    TempData["ErrorMessage"] = "No se encontró el empleado asociado a su sesión. Vuelva a iniciar sesión.";
                    return RedirectToAction("Index", "Home");
                }

                (items, totalCount) = _solicitudService.ObtenerPaginadoPorEmpleado(pageIndex, PageSize, empleadoId.Value, estado);
            }

            var modelo = new PaginatedListViewModel<SolicitudViewModel>
            {
                Items = MapearAViewModel(items),
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            ViewBag.EstadoActual = estado;
            return View(modelo);
        }


        [Authorize(Roles = "Empleado")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categorias = _categoriaService.ObtenerTodos();
            return View();
        }

        [Authorize(Roles = "Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Solicitud solicitud)
        {
            var empleadoId = ObtenerEmpleadoIdLogueado();
            if (empleadoId == null) return RedirectToAction("Index", "Home");

            solicitud.EmpleadoId = empleadoId.Value;
            ModelState.Remove(nameof(solicitud.EmpleadoId));

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(solicitud);
            }

            try
            {
                _solicitudService.Crear(solicitud);
                TempData["SuccessMessage"] = "Solicitud enviada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(solicitud);
            }
        }

        [Authorize(Roles = "Empleado")]
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var solicitud = _solicitudService.ObtenerPorId(id);
            if (solicitud == null)
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            if (solicitud.Estado != "Pendiente")
            {
                TempData["ErrorMessage"] = "Solo se pueden editar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _categoriaService.ObtenerTodos();
            return View(solicitud);
        }

        [Authorize(Roles = "Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Solicitud solicitud)
        {
            if (id != solicitud.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(solicitud);
            }

            try
            {
                _solicitudService.Actualizar(solicitud);
                TempData["SuccessMessage"] = "Solicitud actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(solicitud);
            }
        }
        //para aprobar y rechazar solicitud
        [Authorize(Roles = "Admin,Tecnico")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            // El servicio se encarga de validar si el estado es válido y si la solicitud está pendiente
            _solicitudService.CambiarEstado(id, nuevoEstado);
            TempData["SuccessMessage"] = $"Estado cambiado a '{nuevoEstado}' correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            var empleadoId = ObtenerEmpleadoIdLogueado();
            if (empleadoId == null) return RedirectToAction("Index", "Home");

            // Delegamos toda la lógica de permisos y validación de estado al servicio
            _solicitudService.Eliminar(id, empleadoId.Value);
            TempData["SuccessMessage"] = "Solicitud eliminada correctamente.";
            
            return RedirectToAction(nameof(Index));
        }

        //desde lista de Empleados, ver solicitudes (siendo admin)
        [Authorize(Roles = "Admin")]
        public IActionResult PorEmpleado(int empleadoId, int pageIndex = 1, string? estado = null)
        {
            if (pageIndex < 1) pageIndex = 1;

            var (solicitudes, totalCount) = _solicitudService.ObtenerPaginadoPorEmpleado(pageIndex, PageSize, empleadoId, estado);

            var modelo = new PaginatedListViewModel<SolicitudViewModel>
            {
                Items = MapearAViewModel(solicitudes),
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            var empleado = _empleadoService.ObtenerPorId(empleadoId);
            ViewBag.EmpleadoNombre = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Empleado";
            ViewBag.EmpleadoId = empleadoId;
            ViewBag.EstadoActual = estado;

            return View(modelo);
        }

        // --- MÉTODOS PRIVADOS AUXILIARES ---

        private int? ObtenerEmpleadoIdLogueado()
        {
            var empleadoIdStr = User.FindFirstValue("EmpleadoId");
            return int.TryParse(empleadoIdStr, out var empleadoId) ? empleadoId : null;
        }

        private List<SolicitudViewModel> MapearAViewModel(List<Solicitud> solicitudes)
        {
            var listaViewModel = new List<SolicitudViewModel>();
            foreach (var solicitud in solicitudes)
            {
                listaViewModel.Add(new SolicitudViewModel
                {
                    Solicitud = solicitud,
                    Empleado = _empleadoService.ObtenerPorId(solicitud.EmpleadoId)!,
                    Categoria = _categoriaService.ObtenerPorId(solicitud.CategoriaId)!
                });
            }
            return listaViewModel;
        }
    }
}