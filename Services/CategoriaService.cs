using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using final_LAB2.Services.Interfaces;

namespace final_LAB2.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public List<Categoria> ObtenerTodos() => _categoriaRepository.ObtenerTodos();

        public Categoria? ObtenerPorId(int id) => _categoriaRepository.ObtenerPorId(id);

        public void Crear(Categoria categoria) => _categoriaRepository.Agregar(categoria);

        public void Actualizar(Categoria categoria) => _categoriaRepository.Actualizar(categoria);

        public List<Categoria> Buscar(string termino, int maxResultados = 10)
            => _categoriaRepository.Buscar(termino, maxResultados);

        public (List<Categoria> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize)
        {
            var items = _categoriaRepository.ObtenerPaginado(pageIndex, pageSize);
            var total = _categoriaRepository.ContarTotal();
            return (items, total);
        }

        public void Eliminar(int id)
        {
            if (_categoriaRepository.EstaEnUso(id))
            {
                throw new InvalidOperationException("No se puede eliminar: hay equipos o solicitudes que usan esta categoría.");
            }
            _categoriaRepository.Eliminar(id);
        }
    }
}
