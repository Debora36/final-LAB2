using final_LAB2.Models;
using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace final_LAB2.Controllers
{
    [Authorize(Roles = "Admin,Tecnico,Empleado")]// Admin/Tecnico ven todo — Empleado ve solo las suyas
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
            ViewBag.EquiposDisponibles = _equipoService.ObtenerPorEstado("Disponible");
        }
        if (User.IsInRole("Empleado"))
        {
            var empleadoIdStr = User.FindFirstValue("EmpleadoId");
            if (string.IsNullOrEmpty(empleadoIdStr))
            {
                TempData["ErrorMessage"] = "No se encontró el empleado asociado a su sesión. Vuelva a iniciar sesión.";
                return RedirectToAction("Index", "Home");
            }
            var empleadoId = int.Parse(User.FindFirstValue("EmpleadoId")!);
            (items, totalCount) = _solicitudService.ObtenerPaginadoPorEmpleado(pageIndex, PageSize, empleadoId, estado);
            ViewBag.Categorias = _categoriaService.ObtenerTodos();
        }
        else
        {
            (items, totalCount) = _solicitudService.ObtenerPaginado(pageIndex, PageSize, estado);
        }

        var listaViewModel = new List<SolicitudViewModel>();
        foreach (var solicitud in items)
        {
            listaViewModel.Add(new SolicitudViewModel
            {
                Solicitud = solicitud,
                Empleado = _empleadoService.ObtenerPorId(solicitud.EmpleadoId)!,
                Categoria = _categoriaService.ObtenerPorId(solicitud.CategoriaId)!
            });
        }
        var modelo = new PaginatedListViewModel<SolicitudViewModel>
        {
            Items = listaViewModel,
            PageIndex = pageIndex,
            PageSize = PageSize,
            TotalCount = totalCount
        };

        ViewBag.EstadoActual = estado;
        return View(modelo);
    }

        [HttpGet]
        public IActionResult Detalle(int id)
        {
            var solicitud = _solicitudService.ObtenerPorId(id);
            if (solicitud == null)
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(solicitud);
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
            var empleadoId = int.Parse(User.FindFirstValue("EmpleadoId")!);
            solicitud.EmpleadoId = empleadoId;
            ModelState.Remove(nameof(solicitud.EmpleadoId));

            if (!ModelState.IsValid)
            {
                // Ahora volvemos a la vista Create, no al Index
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
            if (id != solicitud.Id)
                return NotFound();

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
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin,Tecnico")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            var estadosValidos = new[] { "Pendiente", "Aprobada", "Rechazada" };
            if (!estadosValidos.Contains(nuevoEstado))
            {
                TempData["ErrorMessage"] = "Estado no válido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _solicitudService.CambiarEstado(id, nuevoEstado);
                TempData["SuccessMessage"] = $"Estado cambiado a '{nuevoEstado}' correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Empleado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            var empleadoId = int.Parse(User.FindFirstValue("EmpleadoId")!);
            var solicitud = _solicitudService.ObtenerPorId(id);

            if (solicitud == null)
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // Verificamos que la solicitud pertenezca al empleado logueado
            if (solicitud.EmpleadoId != empleadoId)
            {
                TempData["ErrorMessage"] = "No tenés permiso para eliminar esta solicitud.";
                return RedirectToAction(nameof(Index));
            }

            if (solicitud.Estado != "Pendiente")
            {
                TempData["ErrorMessage"] = "Solo se pueden eliminar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            _solicitudService.Eliminar(id);
            TempData["SuccessMessage"] = "Solicitud eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Tecnico")]
        public IActionResult PorEmpleado(int empleadoId, int pageIndex = 1, string? estado = null)
        {
            if (pageIndex < 1) pageIndex = 1;

            var (solicitudes, totalCount) = _solicitudService.ObtenerPaginadoPorEmpleado(pageIndex, PageSize, empleadoId, estado);

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

            var modelo = new PaginatedListViewModel<SolicitudViewModel>
            {
                Items = listaViewModel,
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            // Para mostrar el nombre del empleado en el título de la vista
            var empleado = _empleadoService.ObtenerPorId(empleadoId);
            ViewBag.EmpleadoNombre = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Empleado";
            ViewBag.EmpleadoId = empleadoId;
            ViewBag.EstadoActual = estado;

            return View(modelo);
        }
    }
}