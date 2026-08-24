using final_LAB2.Models;

namespace final_LAB2.Services.Interfaces
{
    public interface IUsuarioService
    {
        Usuario? ValidarCredenciales(string username, string passwordPlano);
        Usuario? ObtenerPorId(int id);
        List<Usuario> ObtenerTodos();
        (List<Usuario> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize);
        void RegistrarUsuario(Usuario usuario, string passwordPlano);
        void ActualizarDatos(Usuario usuario);
        void CambiarPassword(int usuarioId, string nuevaPasswordPlano);
        void DesactivarUsuario(int id);
    }
}
