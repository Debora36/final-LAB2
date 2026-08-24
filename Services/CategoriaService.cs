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
