using final_LAB2.Models;
using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_LAB2.Controllers
{
    // Solo Admin y Tecnico gestionan equipos. El Empleado tiene su propia vista
    // (solicitar préstamo / devolver) que se arma aparte, no reutiliza este controller.
    [Authorize(Roles = "Admin,Tecnico")]
    public class EquipoController : Controller
    {
        private const int PageSize = 10;
        private readonly IEquipoService _equipoService;
        private readonly ICategoriaService _categoriaService;

        public EquipoController(IEquipoService equipoService, ICategoriaService categoriaService)
        {
            _equipoService = equipoService;
            _categoriaService = categoriaService;
        }

        public IActionResult Index(int pageIndex = 1, string? estado = null, int? categoriaId = null)
        {
            if (pageIndex < 1) pageIndex = 1;

            var (items, totalCount) = _equipoService.ObtenerPaginado(pageIndex, PageSize, estado, categoriaId);

            var modelo = new PaginatedListViewModel<Equipo>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            ViewBag.EstadoActual = estado;
            ViewBag.CategoriaIdActual = categoriaId;
            ViewBag.CategoriaNombreActual = categoriaId.HasValue
                ? _categoriaService.ObtenerPorId(categoriaId.Value)?.Nombre
                : null;

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DarDeBaja(int id)
        {
            _equipoService.DarDeBaja(id);
            TempData["SuccessMessage"] = "Equipo dado de baja correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categorias = _categoriaService.ObtenerTodos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Equipo equipo, IFormFile? archivoGarantia, [FromServices] IWebHostEnvironment environment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }

            if (archivoGarantia != null && archivoGarantia.Length > 0)
            {
                var carpeta = Path.Combine(environment.WebRootPath, "Uploads", "Equipos");
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoGarantia.FileName);
                var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    archivoGarantia.CopyTo(stream);
                }

                equipo.RutaArchivoGarantia = $"/Uploads/Equipos/{nombreArchivo}";
            }

            try
            {
                _equipoService.Crear(equipo);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }

            TempData["SuccessMessage"] = "Equipo creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var equipo = _equipoService.ObtenerPorId(id);
            if (equipo == null)
            {
                TempData["ErrorMessage"] = "Equipo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _categoriaService.ObtenerTodos();
            return View(equipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Equipo equipo, IFormFile? archivoGarantia, bool eliminarArchivoActual,
                                   [FromServices] IWebHostEnvironment environment)
        {
            if (id != equipo.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }

            var equipoActual = _equipoService.ObtenerPorId(id);
            if (equipoActual == null)
            {
                TempData["ErrorMessage"] = "Equipo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            if (archivoGarantia != null && archivoGarantia.Length > 0)
            {
                // Subir el nuevo y borrar el físico anterior (si había uno)
                var carpeta = Path.Combine(environment.WebRootPath, "Uploads", "Equipos");
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoGarantia.FileName);
                var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    archivoGarantia.CopyTo(stream);
                }

                EliminarArchivoFisico(equipoActual.RutaArchivoGarantia, environment);
                equipo.RutaArchivoGarantia = $"/Uploads/Equipos/{nombreArchivo}";
            }
            else if (eliminarArchivoActual)
            {
                EliminarArchivoFisico(equipoActual.RutaArchivoGarantia, environment);
                equipo.RutaArchivoGarantia = null;
            }
            else
            {
                // Ni subieron uno nuevo ni marcaron eliminar: se mantiene el que ya había
                equipo.RutaArchivoGarantia = equipoActual.RutaArchivoGarantia;
            }

            try
            {
                _equipoService.Actualizar(equipo);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }

            TempData["SuccessMessage"] = "Equipo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private static void EliminarArchivoFisico(string? rutaRelativa, IWebHostEnvironment environment)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa)) return;

            var rutaFisica = Path.Combine(environment.WebRootPath, rutaRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(rutaFisica))
            {
                System.IO.File.Delete(rutaFisica);
            }
        }
    }
}
