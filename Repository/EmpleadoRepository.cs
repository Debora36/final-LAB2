using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace final_LAB2.Repository
{
    public class EmpleadoRepository : RepositorioBase, IEmpleadoRepository
    {
        public EmpleadoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Empleado? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Nombre, Apellido, DNI, Telefono, UsuarioId, Activo
                                    FROM EMPLEADO WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearEmpleado(reader) : null;
        }

        public List<Empleado> ObtenerTodos()
        {
            var empleados = new List<Empleado>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Nombre, Apellido, DNI, Telefono, UsuarioId, Activo
                                    FROM EMPLEADO ORDER BY Apellido, Nombre";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                empleados.Add(MapearEmpleado(reader));
            }
            return empleados;
        }

        public List<Empleado> ObtenerPaginado(int pageIndex, int pageSize)
        {
            var empleados = new List<Empleado>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Nombre, Apellido, DNI, Telefono, UsuarioId, Activo
                                    FROM EMPLEADO
                                    ORDER BY Apellido, Nombre
                                    LIMIT @PageSize OFFSET @Offset";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                empleados.Add(MapearEmpleado(reader));
            }
            return empleados;
        }

        public int ContarTotal()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM EMPLEADO";
            using var command = new MySqlCommand(query, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<Usuario> ObtenerUsuariosElegibles(int usuarioIdActual = 0)
        {
            var usuarios = new List<Usuario>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Username, Password, Rol, AvatarUrl, Email, Activo
                                    FROM USUARIO
                                    WHERE Rol = 'Empleado'
                                      AND (Id NOT IN (SELECT UsuarioId FROM EMPLEADO WHERE UsuarioId IS NOT NULL)
                                           OR Id = @UsuarioIdActual)
                                    ORDER BY Username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UsuarioIdActual", usuarioIdActual);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    Rol = reader.GetString("Rol"),
                    AvatarUrl = reader.IsDBNull(reader.GetOrdinal("AvatarUrl")) ? null : reader.GetString("AvatarUrl"),
                    Email = reader.GetString("Email"),
                    Activo = reader.GetBoolean("Activo")
                });
            }
            return usuarios;
        }

        public void Agregar(Empleado empleado)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"INSERT INTO EMPLEADO (Nombre, Apellido, DNI, Telefono, UsuarioId, Activo)
                                    VALUES (@Nombre, @Apellido, @DNI, @Telefono, @UsuarioId, @Activo)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nombre", empleado.Nombre);
            command.Parameters.AddWithValue("@Apellido", empleado.Apellido);
            command.Parameters.AddWithValue("@DNI", empleado.DNI);
            command.Parameters.AddWithValue("@Telefono", (object?)empleado.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@UsuarioId", (object?)empleado.UsuarioId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Activo", empleado.Activo);

            command.ExecuteNonQuery();
        }

        // No toca Activo: eso se maneja únicamente vía Desactivar
        public void Actualizar(Empleado empleado)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"UPDATE EMPLEADO
                                    SET Nombre = @Nombre, Apellido = @Apellido, DNI = @DNI,
                                        Telefono = @Telefono, UsuarioId = @UsuarioId
                                    WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nombre", empleado.Nombre);
            command.Parameters.AddWithValue("@Apellido", empleado.Apellido);
            command.Parameters.AddWithValue("@DNI", empleado.DNI);
            command.Parameters.AddWithValue("@Telefono", (object?)empleado.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@UsuarioId", (object?)empleado.UsuarioId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", empleado.Id);

            command.ExecuteNonQuery();
        }

        public void Desactivar(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "UPDATE EMPLEADO SET Activo = FALSE WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        private static Empleado MapearEmpleado(MySqlDataReader reader)
        {
            return new Empleado
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                Apellido = reader.GetString("Apellido"),
                DNI = reader.GetString("DNI"),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                UsuarioId = reader.IsDBNull(reader.GetOrdinal("UsuarioId")) ? null : reader.GetInt32("UsuarioId"),
                Activo = reader.GetBoolean("Activo")
            };
        }
   
    }
}
