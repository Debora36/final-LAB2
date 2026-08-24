using final_LAB2.Repository.Interfaces;
using final_LAB2.Models;
using final_LAB2.Services.Interfaces;

namespace final_LAB2.Services
{
    public class UsuarioService : IUsuarioService
    {
        private const int BCryptWorkFactor = 12;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public Usuario? ValidarCredenciales(string username, string passwordPlano)
        {
            var usuario = _usuarioRepository.ObtenerPorUsername(username);
            if (usuario == null || !usuario.Activo)
            {
                return null;
            }

            bool esValido = BCrypt.Net.BCrypt.Verify(passwordPlano, usuario.Password);
            return esValido ? usuario : null;
        }

        public Usuario? ObtenerPorId(int id) => _usuarioRepository.ObtenerPorId(id);

        public List<Usuario> ObtenerTodos() => _usuarioRepository.ObtenerTodos();

        public (List<Usuario> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize)
        {
            var items = _usuarioRepository.ObtenerPaginado(pageIndex, pageSize);
            var total = _usuarioRepository.ContarTotal();
            return (items, total);
        }

        public void RegistrarUsuario(Usuario usuario, string passwordPlano)
        {
            if (_usuarioRepository.ObtenerPorUsername(usuario.Username) != null)
            {
                throw new InvalidOperationException("Ese nombre de usuario ya existe.");
            }

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(passwordPlano, workFactor: BCryptWorkFactor);
            usuario.Activo = true;
            _usuarioRepository.Agregar(usuario);
        }

        public void ActualizarDatos(Usuario usuario) => _usuarioRepository.Actualizar(usuario);

        public void CambiarPassword(int usuarioId, string nuevaPasswordPlano)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(nuevaPasswordPlano, workFactor: BCryptWorkFactor);
            _usuarioRepository.ActualizarPassword(usuarioId, hash);
        }

        public void DesactivarUsuario(int id) => _usuarioRepository.Desactivar(id);
    }
}
