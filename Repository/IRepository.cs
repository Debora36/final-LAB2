
namespace final_LAB2.Repository.Interfaces
{
    // Solo lo que TODAS las entidades comparten de verdad.
    // Paginado, filtros y "baja" quedan afuera a propósito: no todas las entidades
    // los necesitan igual (ver Categoria y Equipo), y forzarlos acá rompe el contrato.
    public interface IRepositorio<T>
    {
        T? ObtenerPorId(int id);
        List<T> ObtenerTodos();
        void Agregar(T entidad);
        void Actualizar(T entidad);
    }
}
 