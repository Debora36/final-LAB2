using final_LAB2.Models;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_LAB2.Controllers
{
    [Authorize(Roles = "Admin,Tecnico")]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [Authorize(Roles = "Admin")]
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


        // Lista paginada para el filtro de Equipo
        [HttpGet]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
