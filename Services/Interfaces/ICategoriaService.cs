using final_LAB2.Models;

namespace final_LAB2.Services.Interfaces
{
    public interface ICategoriaService
    {
        List<Categoria> ObtenerTodos();
        Categoria? ObtenerPorId(int id);
        void Crear(Categoria categoria);
        void Actualizar(Categoria categoria);
        void Eliminar(int id); // lanza InvalidOperationException si está en uso
        (List<Categoria> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize);
    }
}
