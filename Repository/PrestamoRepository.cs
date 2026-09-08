using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using MySqlConnector;

namespace final_LAB2.Repository
{
    public class PrestamoRepository : RepositorioBase, IPrestamoRepository
    {
        public PrestamoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Prestamo? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, EquipoId, EmpleadoId, FechaPrestamo, 
                                          FechaDevolucionEstimada, FechaDevolucionReal
                                   FROM PRESTAMO WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearPrestamo(reader) : null;
        }

        public List<Prestamo> ObtenerTodos()
        {
            var prestamos = new List<Prestamo>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, EquipoId, EmpleadoId, FechaPrestamo,
                                          FechaDevolucionEstimada, FechaDevolucionReal
                                   FROM PRESTAMO ORDER BY FechaPrestamo DESC";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
                prestamos.Add(MapearPrestamo(reader));

            return prestamos;
        }

        public List<Prestamo> ObtenerPaginado(int pageIndex, int pageSize, string? estado = null)
        {
            var prestamos = new List<Prestamo>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            // "Activo" = sin fecha de devolución real, "Devuelto" = con fecha de devolución real
            var whereEstado = estado == "Activo" ? "WHERE FechaDevolucionReal IS NULL"
                            : estado == "Devuelto" ? "WHERE FechaDevolucionReal IS NOT NULL"
                            : "";

            var query = $@"SELECT Id, EquipoId, EmpleadoId, FechaPrestamo,
                                  FechaDevolucionEstimada, FechaDevolucionReal
                           FROM PRESTAMO
                           {whereEstado}
                           ORDER BY FechaPrestamo DESC
                           LIMIT @Offset, @PageSize";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                prestamos.Add(MapearPrestamo(reader));

            return prestamos;
        }

        public int ContarTotal()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM PRESTAMO";
            using var command = new MySqlCommand(query, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Agregar(Prestamo prestamo)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"INSERT INTO PRESTAMO (EquipoId, EmpleadoId, FechaPrestamo, FechaDevolucionEstimada)
                                   VALUES (@EquipoId, @EmpleadoId, @FechaPrestamo, @FechaDevolucionEstimada)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EquipoId", prestamo.EquipoId);
            command.Parameters.AddWithValue("@EmpleadoId", prestamo.EmpleadoId);
            command.Parameters.AddWithValue("@FechaPrestamo", prestamo.FechaPrestamo);
            command.Parameters.AddWithValue("@FechaDevolucionEstimada", 
                (object?)prestamo.FechaDevolucionEstimada ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Actualizar(Prestamo prestamo)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"UPDATE PRESTAMO
                                   SET FechaDevolucionReal = @FechaDevolucionReal
                                   WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FechaDevolucionReal",
                (object?)prestamo.FechaDevolucionReal ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", prestamo.Id);

            command.ExecuteNonQuery();
        }

        private static Prestamo MapearPrestamo(MySqlDataReader reader)
        {
            return new Prestamo
            {
                Id = reader.GetInt32("Id"),
                EquipoId = reader.GetInt32("EquipoId"),
                EmpleadoId = reader.GetInt32("EmpleadoId"),
                FechaPrestamo = reader.GetDateTime("FechaPrestamo"),
                FechaDevolucionEstimada = reader.IsDBNull(reader.GetOrdinal("FechaDevolucionEstimada"))
                    ? null : reader.GetDateTime("FechaDevolucionEstimada"),
                FechaDevolucionReal = reader.IsDBNull(reader.GetOrdinal("FechaDevolucionReal"))
                    ? null : reader.GetDateTime("FechaDevolucionReal")
            };
        }

        public List<Prestamo> ObtenerPaginadoPorDni(int pageIndex, int pageSize, string dni)
        {
            var prestamos = new List<Prestamo>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT p.Id, p.EquipoId, p.EmpleadoId, p.FechaPrestamo,
                                        p.FechaDevolucionEstimada, p.FechaDevolucionReal
                                    FROM PRESTAMO p
                                    INNER JOIN EMPLEADO e ON e.Id = p.EmpleadoId
                                    WHERE e.DNI LIKE @DNI
                                    ORDER BY p.FechaPrestamo DESC
                                    LIMIT @Offset, @PageSize";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@DNI", $"%{dni}%");
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                prestamos.Add(MapearPrestamo(reader));

            return prestamos;
        }

        public int ContarTotalPorDni(string dni)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT COUNT(*) FROM PRESTAMO p
                                    INNER JOIN EMPLEADO e ON e.Id = p.EmpleadoId
                                    WHERE e.DNI LIKE @DNI";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@DNI", $"%{dni}%");
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<Prestamo> ObtenerPaginadoPorEmpleado(int pageIndex, int pageSize, int empleadoId)
        {
            var prestamos = new List<Prestamo>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, EquipoId, EmpleadoId, FechaPrestamo,
                                        FechaDevolucionEstimada, FechaDevolucionReal
                                    FROM PRESTAMO
                                    WHERE EmpleadoId = @EmpleadoId
                                    ORDER BY FechaPrestamo DESC
                                    LIMIT @Offset, @PageSize";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmpleadoId", empleadoId);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                prestamos.Add(MapearPrestamo(reader));

            return prestamos;
        }

        public int ContarTotalPorEmpleado(int empleadoId)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM PRESTAMO WHERE EmpleadoId = @EmpleadoId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmpleadoId", empleadoId);
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }
}