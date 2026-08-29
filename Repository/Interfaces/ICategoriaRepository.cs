using final_LAB2.Models;

namespace final_LAB2.Repository.Interfaces
{
    public interface ICategoriaRepository : IRepositorio<Categoria>
    {
        void Eliminar(int id);

        // true si hay Equipos o Solicitudes que referencian esta categoría (bloquea el DELETE)
        bool EstaEnUso(int id);

        // Búsqueda ajax: solo trae coincidencias, nunca la tabla completa
        List<Categoria> Buscar(string termino, int maxResultados = 10);

        List<Categoria> ObtenerPaginado(int pageIndex, int pageSize);
        int ContarTotal();
    }
}
