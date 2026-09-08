using final_LAB2.Models.ViewModels;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using final_LAB2.Models;


namespace final_LAB2.Controllers
{

    [Authorize(Roles = "Admin,Tecnico")]
    public class PrestamoController : Controller
    {
        private const int PageSize = 10;
        private readonly IPrestamoService _prestamoService;
        private readonly IEmpleadoService _empleadoService;
        private readonly IEquipoService _equipoService;
        private readonly ISolicitudService _solicitudService;

        public PrestamoController(IPrestamoService prestamoService,
                                IEmpleadoService empleadoService,
                                IEquipoService equipoService,
                                ISolicitudService solicitudService)
        {
            _prestamoService = prestamoService;
            _empleadoService = empleadoService;
            _equipoService = equipoService;
            _solicitudService = solicitudService;
        }

        // Lista todos los préstamos — activos y devueltos
        public IActionResult Index(int pageIndex = 1, string? estado = null)
        {
            if (pageIndex < 1) pageIndex = 1;

            var (prestamos, totalCount) = _prestamoService.ObtenerPaginado(pageIndex, PageSize, estado);

            var listaViewModel = prestamos.Select(p => new PrestamoViewModel
            {
                Prestamo = p,
                Empleado = _empleadoService.ObtenerPorId(p.EmpleadoId)!,
                Equipo = _equipoService.ObtenerPorId(p.EquipoId)!
            }).ToList();

            var modelo = new PaginatedListViewModel<PrestamoViewModel>
            {
                Items = listaViewModel,
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            ViewBag.EstadoActual = estado;
            return View(modelo);
        }

        // Registra la devolución del equipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarDevolucion(int id)
        {
            try
            {
                _prestamoService.RegistrarDevolucion(id);
                TempData["SuccessMessage"] = "Devolución registrada correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin,Tecnico")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Aprobar(int id, int equipoId, DateTime? fechaDevolucionEstimada)
        {
            var solicitud = _solicitudService.ObtenerPorId(id);
            if (solicitud == null)
            {
                TempData["ErrorMessage"] = "Solicitud no encontrada.";
                return RedirectToAction("Index", "Solicitud");
            }

            try
            {
                var prestamo = new Prestamo
                {
                    EquipoId = equipoId,
                    EmpleadoId = solicitud.EmpleadoId,
                    FechaPrestamo = DateTime.Now,
                    FechaDevolucionEstimada = fechaDevolucionEstimada
                };
                _prestamoService.Crear(prestamo);

                _solicitudService.CambiarEstado(id, "Aprobada");

                TempData["SuccessMessage"] = "Solicitud aprobada y préstamo creado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index", "Solicitud");
        }
    }
}