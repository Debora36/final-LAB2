using final_LAB2.Models;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace final_LAB2.Controllers
{
    public class SolicitudController : Controller
    {
        private readonly ISolicitudService _solicitudService;

        public SolicitudController(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        // GET: /Solicitud
        public IActionResult Index(int pageIndex = 1, int pageSize = 10, string? estado = null)
        {
            var (items, totalCount) = _solicitudService.ObtenerPaginado(pageIndex, pageSize);

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.EstadoActual = estado;
            return View(items);
        }

        // GET: /Solicitud/Detalle/5
        public IActionResult Detalle(int id)
        {
            var solicitud = _solicitudService.ObtenerPorId(id);
            if (solicitud == null)
                return NotFound();

            return View(solicitud);
        }

        // GET: /Solicitud/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Solicitud/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Solicitud solicitud)
        {
            if (!ModelState.IsValid)
                return View(solicitud);

            try
            {
                _solicitudService.Crear(solicitud);
                TempData["Exito"] = "Solicitud creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(solicitud);
            }
        }

        // GET: /Solicitud/Editar/5
        public IActionResult Editar(int id)
        {
            var solicitud = _solicitudService.ObtenerPorId(id);
            if (solicitud == null)
                return NotFound();

            if (solicitud.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden editar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            return View(solicitud);
        }

        // POST: /Solicitud/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Solicitud solicitud)
        {
            if (id != solicitud.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(solicitud);

            try
            {
                _solicitudService.Actualizar(solicitud);
                TempData["Exito"] = "Solicitud actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Solicitud/CambiarEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            var estadosValidos = new[] { "Pendiente", "Aprobada", "Rechazada" };
            if (!estadosValidos.Contains(nuevoEstado))
            {
                TempData["Error"] = "Estado no válido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _solicitudService.CambiarEstado(id, nuevoEstado);
                TempData["Exito"] = $"Estado cambiado a '{nuevoEstado}' correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        
    }
}