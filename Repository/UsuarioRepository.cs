using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace final_LAB2.Repository
{
    public class UsuarioRepository : RepositorioBase, IUsuarioRepository
    {
        public UsuarioRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Usuario? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Username, Password, Rol, AvatarUrl, Email, Activo
                                    FROM USUARIO WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearUsuario(reader) : null;
        }

        public Usuario? ObtenerPorUsername(string username)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Username, Password, Rol, AvatarUrl, Email, Activo
                                    FROM USUARIO WHERE Username = @Username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearUsuario(reader) : null;
        }

        public List<Usuario> ObtenerTodos()
        {
            var usuarios = new List<Usuario>();

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Username, Password, Rol, AvatarUrl, Email, Activo
                                    FROM USUARIO ORDER BY Username";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                usuarios.Add(MapearUsuario(reader));
            }

            return usuarios;
        }

        public List<Usuario> ObtenerPaginado(int pageIndex, int pageSize)
        {
            var usuarios = new List<Usuario>();

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Username, Password, Rol, AvatarUrl, Email, Activo
                                    FROM USUARIO
                                    ORDER BY Username
                                    LIMIT @PageSize OFFSET @Offset";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(MapearUsuario(reader));
            }

            return usuarios;
        }

        public int ContarTotal()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM USUARIO";
            using var command = new MySqlCommand(query, connection);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Agregar(Usuario usuario)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"INSERT INTO USUARIO (Username, Password, Rol, AvatarUrl, Email, Activo)
                                    VALUES (@Username, @Password, @Rol, @AvatarUrl, @Email, @Activo)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", usuario.Username);
            command.Parameters.AddWithValue("@Password", usuario.Password);
            command.Parameters.AddWithValue("@Rol", usuario.Rol);
            command.Parameters.AddWithValue("@AvatarUrl", (object?)usuario.AvatarUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@Activo", usuario.Activo);

            command.ExecuteNonQuery();
        }

        public void Actualizar(Usuario usuario)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"UPDATE USUARIO
                                    SET Username = @Username, Rol = @Rol,
                                        AvatarUrl = @AvatarUrl, Email = @Email, Activo = @Activo
                                    WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", usuario.Username);
            command.Parameters.AddWithValue("@Rol", usuario.Rol);
            command.Parameters.AddWithValue("@AvatarUrl", (object?)usuario.AvatarUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@Activo", usuario.Activo);
            command.Parameters.AddWithValue("@Id", usuario.Id);

            command.ExecuteNonQuery();
        }

        public void ActualizarPassword(int id, string nuevoHashPassword)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "UPDATE USUARIO SET Password = @Password WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Password", nuevoHashPassword);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        public void Desactivar(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "UPDATE USUARIO SET Activo = FALSE WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        private static Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32("Id"),
                Username = reader.GetString("Username"),
                Password = reader.GetString("Password"),
                Rol = reader.GetString("Rol"),
                AvatarUrl = reader.IsDBNull(reader.GetOrdinal("AvatarUrl")) ? null : reader.GetString("AvatarUrl"),
                Email = reader.GetString("Email"),
                Activo = reader.GetBoolean("Activo")
            };
        }
    }
}