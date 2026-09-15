using final_LAB2.Models;
using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_LAB2.Controllers
{
    [Authorize(Roles = "Admin,Tecnico")]
    public class EquipoController : Controller
    {
        private const int PageSize = 10;
        private readonly IEquipoService _equipoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IFileService _fileService;

        public EquipoController(IEquipoService equipoService, ICategoriaService categoriaService, IFileService fileService)
        {
            _equipoService = equipoService;
            _categoriaService = categoriaService;
            _fileService = fileService;
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
        public IActionResult Create(Equipo equipo, IFormFile? archivoGarantia)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }

            try
            {
                if (archivoGarantia != null && archivoGarantia.Length > 0)
                {
                    equipo.RutaArchivoGarantia = _fileService.GuardarArchivo(archivoGarantia, "Equipos");
                }

                _equipoService.Crear(equipo);
                
                TempData["SuccessMessage"] = "Equipo creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }
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
        public IActionResult Edit(int id, Equipo equipo, IFormFile? archivoGarantia, bool eliminarArchivoActual)
        {
            if (id != equipo.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }

            try
            {
                string? nuevaRutaGarantia = null;
                var equipoOriginal = _equipoService.ObtenerPorId(id);

                if (equipoOriginal == null)
                {
                    TempData["ErrorMessage"] = "Equipo no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                if (archivoGarantia != null && archivoGarantia.Length > 0)
                {
                    _fileService.EliminarArchivo(equipoOriginal.RutaArchivoGarantia);
                    nuevaRutaGarantia = _fileService.GuardarArchivo(archivoGarantia, "Equipos");
                }
                else if (eliminarArchivoActual)
                {
                    _fileService.EliminarArchivo(equipoOriginal.RutaArchivoGarantia);
                }

                _equipoService.Actualizar(equipo, nuevaRutaGarantia, eliminarArchivoActual);

                TempData["SuccessMessage"] = "Equipo actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Categorias = _categoriaService.ObtenerTodos();
                return View(equipo);
            }
        }

        //para el select2 cuando apruebo una solicitud
        [HttpGet]
        public IActionResult BuscarDisponibles(string? q, int categoriaId)
        {
            try
            {
                var equipos = _equipoService.BuscarDisponibles(q, categoriaId);
                var resultado = equipos.Select(e => new { id = e.Id, text = $"{e.Modelo} — {e.NumeroSerie}" });// Formato esperado por Select2
                return Json(new { results = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { results = new List<object>(), error = ex.Message });
            }
        }
    }
    
}
