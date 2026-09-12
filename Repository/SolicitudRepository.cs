using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
 
namespace final_LAB2.Repository
{
    public class SolicitudRepository : RepositorioBase, ISolicitudRepository
    {
        public SolicitudRepository(IConfiguration configuration) : base(configuration)
        {
        }
 
        public Solicitud? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"SELECT Id, EmpleadoId, CategoriaId, Motivo, TiempoNecesario, FechaSolicitud, Estado
                                    FROM SOLICITUD WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
 
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearSolicitud(reader) : null;
        }
 
        public List<Solicitud> ObtenerTodos()
        {
            var solicitudes = new List<Solicitud>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"SELECT Id, EmpleadoId, CategoriaId, Motivo, TiempoNecesario, FechaSolicitud, Estado
                                    FROM SOLICITUD ORDER BY FechaSolicitud DESC";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
 
            while (reader.Read())
            {
                solicitudes.Add(MapearSolicitud(reader));
            }
            return solicitudes;
        }
 
        public void Agregar(Solicitud solicitud)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"INSERT INTO SOLICITUD (EmpleadoId, CategoriaId, Motivo, TiempoNecesario, FechaSolicitud, Estado)
                                    VALUES (@EmpleadoId, @CategoriaId, @Motivo, @TiempoNecesario, @FechaSolicitud, @Estado)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmpleadoId", solicitud.EmpleadoId);
            command.Parameters.AddWithValue("@CategoriaId", solicitud.CategoriaId);
            command.Parameters.AddWithValue("@Motivo", (object?)solicitud.Motivo ?? DBNull.Value);
            command.Parameters.AddWithValue("@TiempoNecesario", solicitud.TiempoNecesario);
            command.Parameters.AddWithValue("@FechaSolicitud", solicitud.FechaSolicitud);
            command.Parameters.AddWithValue("@Estado", solicitud.Estado);
 
            command.ExecuteNonQuery();
        }
 
        public void Actualizar(Solicitud solicitud)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"UPDATE SOLICITUD
                                    SET EmpleadoId = @EmpleadoId, CategoriaId = @CategoriaId, Motivo = @Motivo,
                                        TiempoNecesario = @TiempoNecesario, Estado = @Estado
                                    WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmpleadoId", solicitud.EmpleadoId);
            command.Parameters.AddWithValue("@CategoriaId", solicitud.CategoriaId);
            command.Parameters.AddWithValue("@Motivo", (object?)solicitud.Motivo ?? DBNull.Value);
            command.Parameters.AddWithValue("@TiempoNecesario", solicitud.TiempoNecesario);
            command.Parameters.AddWithValue("@Estado", solicitud.Estado);
            command.Parameters.AddWithValue("@Id", solicitud.Id);
 
            command.ExecuteNonQuery();
        }
 
        private static Solicitud MapearSolicitud(MySqlDataReader reader)
        {
            return new Solicitud
            {
                Id = reader.GetInt32(nameof(Solicitud.Id)),
                EmpleadoId = reader.GetInt32(nameof(Solicitud.EmpleadoId)),
                CategoriaId = reader.GetInt32(nameof(Solicitud.CategoriaId)),
                Motivo = reader.IsDBNull(reader.GetOrdinal(nameof(Solicitud.Motivo))) ? null : reader.GetString(nameof(Solicitud.Motivo)),
                TiempoNecesario = reader.GetString(nameof(Solicitud.TiempoNecesario)),
                FechaSolicitud = reader.GetDateTime(nameof(Solicitud.FechaSolicitud)),
                Estado = reader.GetString(nameof(Solicitud.Estado))
            };
        }

       public List<Solicitud> ObtenerPaginado(int pageIndex, int pageSize, string? estado = null)
        {
            var solicitudes = new List<Solicitud>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var whereEstado = estado != null ? "WHERE Estado = @Estado" : "";
            var query = $@"SELECT Id, EmpleadoId, CategoriaId, Motivo, TiempoNecesario, FechaSolicitud, Estado
                        FROM SOLICITUD
                        {whereEstado}
                        ORDER BY FechaSolicitud DESC
                        LIMIT @Offset, @PageSize";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            if (estado != null)
                command.Parameters.AddWithValue("@Estado", estado);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                solicitudes.Add(MapearSolicitud(reader));

            return solicitudes;
        }


        public int ContarTotal(string? estado = null)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        var whereEstado = estado != null ? "WHERE Estado = @Estado" : "";
        var query = $"SELECT COUNT(*) FROM SOLICITUD {whereEstado}";

        using var command = new MySqlCommand(query, connection);
        if (estado != null)
            command.Parameters.AddWithValue("@Estado", estado);

        return Convert.ToInt32(command.ExecuteScalar());
    }

       public List<Solicitud> ObtenerPaginadoPorEmpleado(int pageIndex, int pageSize, int empleadoId, string? estado = null)
        {
            var solicitudes = new List<Solicitud>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            // Armamos el WHERE dinámicamente según si hay filtro de estado o no
            var whereEstado = estado != null ? "AND Estado = @Estado" : "";

            var query = $@"SELECT Id, EmpleadoId, CategoriaId, Motivo, TiempoNecesario, FechaSolicitud, Estado
                        FROM SOLICITUD
                        WHERE EmpleadoId = @EmpleadoId
                        {whereEstado}
                        ORDER BY FechaSolicitud DESC
                        LIMIT @Offset, @PageSize";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmpleadoId", empleadoId);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            if (estado != null)
                command.Parameters.AddWithValue("@Estado", estado);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                solicitudes.Add(MapearSolicitud(reader));

            return solicitudes;
        }

        public int ContarTotalPorEmpleado(int empleadoId, string? estado = null)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var whereEstado = estado != null ? "AND Estado = @Estado" : "";
            var query = $"SELECT COUNT(*) FROM SOLICITUD WHERE EmpleadoId = @EmpleadoId {whereEstado}";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmpleadoId", empleadoId);
            if (estado != null)
                command.Parameters.AddWithValue("@Estado", estado);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Eliminar(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "DELETE FROM SOLICITUD WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }
}