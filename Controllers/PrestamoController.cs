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

        public PrestamoController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        // Registra la devolución del equipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarDevolucion(int id)
        {

            _prestamoService.RegistrarDevolucion(id);
            TempData["SuccessMessage"] = "Devolución registrada correctamente.";

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin,Tecnico")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Aprobar(int id, int equipoId, DateTime? fechaDevolucionEstimada)
        {
            _prestamoService.AprobarSolicitud(id, equipoId, fechaDevolucionEstimada);
            TempData["SuccessMessage"] = "Solicitud aprobada y préstamo creado correctamente.";

            return RedirectToAction("Index", "Solicitud");
        }
    }
}