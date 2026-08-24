using final_LAB2.Models;

namespace final_LAB2.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorUsername(string username);
        List<Usuario> ObtenerTodos();
        List<Usuario> ObtenerPaginado(int pageIndex, int pageSize);
        int ContarTotal();
        void Agregar(Usuario usuario);
        void Actualizar(Usuario usuario);
        void ActualizarPassword(int id, string nuevoHashPassword);
        void Desactivar(int id);
    }
}
