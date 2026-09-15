using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using final_LAB2.Models;
using final_LAB2.Services.Interfaces;
using final_LAB2.Models.ViewModels;
using System.Security.Claims;

namespace final_LAB2.Controllers;

public class HomeController : Controller
{

    private const int PageSize = 10;
    private readonly IPrestamoService _prestamoService;
    private readonly IEmpleadoService _empleadoService;
    private readonly IEquipoService _equipoService;
    private readonly IConfiguration _configuracion;

    // Inyectamos la configuración a través del constructor
        public HomeController(IConfiguration configuracion,
                              IPrestamoService prestamoService,
                              IEmpleadoService empleadoService,
                              IEquipoService equipoService)
        {
            _configuracion = configuracion;
            _prestamoService = prestamoService;
            _empleadoService = empleadoService;
            _equipoService = equipoService;
        }
        public IActionResult Index(int pageIndex = 1, string? dni = null, string? estado = null)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return View(new PaginatedListViewModel<PrestamoViewModel>());

            if (pageIndex < 1) pageIndex = 1;

            List<Prestamo> prestamos;
            int totalCount;

            if (User.IsInRole("Empleado"))
            {
                var empleadoId = ObtenerEmpleadoIdLogueado();
                if (empleadoId == null)
                    return View(new PaginatedListViewModel<PrestamoViewModel>());

                (prestamos, totalCount) = _prestamoService.ObtenerPaginadoPorEmpleado(pageIndex, PageSize, empleadoId.Value);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dni))
                    (prestamos, totalCount) = _prestamoService.ObtenerPaginadoPorDni(pageIndex, PageSize, dni);
                else
                    (prestamos, totalCount) = _prestamoService.ObtenerPaginado(pageIndex, PageSize, estado);
            }

            var modelo = new PaginatedListViewModel<PrestamoViewModel>
            {
                Items = MapearAPrestamoViewModel(prestamos),
                PageIndex = pageIndex,
                PageSize = PageSize,
                TotalCount = totalCount
            };

            ViewBag.DNIActual = dni;
            ViewBag.EstadoActual = estado;
            return View(modelo);
        }

        // Endpoint AJAX para buscar por DNI
        [HttpGet]
        public IActionResult BuscarPorDni(string dni, int pageIndex = 1)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return Json(new { items = new List<object>(), totalCount = 0 });

            try
            {
                var (prestamos, totalCount) = _prestamoService.ObtenerPaginadoPorDni(pageIndex, PageSize, dni);

                var resultado = prestamos.Select(p => new
                {
                    id = p.Id,
                    empleado = _empleadoService.ObtenerPorId(p.EmpleadoId),
                    equipo = _equipoService.ObtenerPorId(p.EquipoId),
                    fechaPrestamo = p.FechaPrestamo.ToString("dd/MM/yyyy"),
                    fechaDevolucionEstimada = p.FechaDevolucionEstimada?.ToString("dd/MM/yyyy"),
                    fechaDevolucionReal = p.FechaDevolucionReal?.ToString("dd/MM/yyyy"),
                    devuelto = p.FechaDevolucionReal.HasValue
                });

                return Json(new { items = resultado, totalCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { items = new List<object>(), totalCount = 0, error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    //MÉTODOS PRIVADOS AUXILIARES

        private int? ObtenerEmpleadoIdLogueado()
        {
            var empleadoIdStr = User.FindFirstValue("EmpleadoId");
            return int.TryParse(empleadoIdStr, out var empleadoId) ? empleadoId : null;
        }

    private List<PrestamoViewModel> MapearAPrestamoViewModel(List<Prestamo> prestamos)
        {
            return prestamos.Select(p => new PrestamoViewModel
            {
                Prestamo = p,
                Empleado = _empleadoService.ObtenerPorId(p.EmpleadoId)!,
                Equipo = _equipoService.ObtenerPorId(p.EquipoId)!
            }).ToList();
        }
}
