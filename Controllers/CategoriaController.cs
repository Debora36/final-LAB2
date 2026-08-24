using final_LAB2.Models;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_LAB2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // Vista que hospeda la app de Vue. Categoria es chica: no necesita paginado.
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var categorias = _categoriaService.ObtenerTodos();
            return Json(categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear([FromBody] Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _categoriaService.Crear(categoria);
            return Ok(categoria);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest("El id no coincide.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _categoriaService.Actualizar(categoria);
            return Ok(categoria);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _categoriaService.Eliminar(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}