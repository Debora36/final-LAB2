using final_LAB2.Models;
 
namespace final_LAB2.Repository.Interfaces
{
    public interface ICategoriaRepository
    {
        List<Categoria> ObtenerTodos();
        Categoria? ObtenerPorId(int id);
        void Agregar(Categoria categoria);
        void Actualizar(Categoria categoria);
        void Eliminar(int id);
 
        // true si hay Equipos o Solicitudes que referencian esta categoría (bloquea el DELETE)
        bool EstaEnUso(int id);
    }
}