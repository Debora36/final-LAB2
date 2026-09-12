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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var categorias = _categoriaService.ObtenerTodos();
                return Json(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Búsqueda ajax reutilizada por el filtro de Equipo y por el selector de categoría en su ABM
        [HttpGet]
        [Authorize]
        public IActionResult Buscar(string termino = "", int max = 10)
        {
            try
            {
                var categorias = _categoriaService.Buscar(termino, max);
                return Json(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Lista paginada para el filtro de Equipo
        [HttpGet]
        [Authorize]
        public IActionResult ListarParaSeleccion(int pagina = 1)
        {
            const int tamPagina = 3;
            var (items, totalRegistros) = _categoriaService.ObtenerPaginado(pagina, tamPagina);
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamPagina);
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return PartialView("_ListaCategoriasSeleccion", items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear([FromBody] Categoria categoria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _categoriaService.Crear(categoria);
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
                return BadRequest("El id no coincide.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _categoriaService.Actualizar(categoria);
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
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
