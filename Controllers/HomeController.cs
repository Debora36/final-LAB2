using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using final_LAB2.Models;
using MySqlConnector;
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
                var empleadoIdStr = User.FindFirstValue("EmpleadoId");
                if (string.IsNullOrEmpty(empleadoIdStr))
                    return View(new PaginatedListViewModel<PrestamoViewModel>());

                var empleadoId = int.Parse(empleadoIdStr);
                (prestamos, totalCount) = _prestamoService.ObtenerPaginadoPorEmpleado(pageIndex, PageSize, empleadoId);
            }
            else
            {
                // Admin y Tecnico
                if (!string.IsNullOrWhiteSpace(dni))
                    (prestamos, totalCount) = _prestamoService.ObtenerPaginadoPorDni(pageIndex, PageSize, dni);
                else
                    (prestamos, totalCount) = _prestamoService.ObtenerPaginado(pageIndex, PageSize, estado);
            }

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
}
