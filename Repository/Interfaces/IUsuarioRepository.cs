using final_LAB2.Models;
 
namespace final_LAB2.Repository.Interfaces
{
    public interface IUsuarioRepository : IRepositorio<Usuario>
    {
        Usuario? ObtenerPorUsername(string username);
        List<Usuario> ObtenerPaginado(int pageIndex, int pageSize);
        int ContarTotal();
        void ActualizarPassword(int id, string nuevoHashPassword);
        void Desactivar(int id);
    }
}