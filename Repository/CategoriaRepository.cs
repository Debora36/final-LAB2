using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace final_LAB2.Repository
{
    public class CategoriaRepository : RepositorioBase, ICategoriaRepository
    {
        public CategoriaRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Categoria> ObtenerTodos()
        {
            var categorias = new List<Categoria>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "SELECT Id, Nombre, Descripcion FROM CATEGORIA ORDER BY Nombre";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                categorias.Add(MapearCategoria(reader));
            }
            return categorias;
        }

        public Categoria? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "SELECT Id, Nombre, Descripcion FROM CATEGORIA WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearCategoria(reader) : null;
        }

        public void Agregar(Categoria categoria)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "INSERT INTO CATEGORIA (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Actualizar(Categoria categoria)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "UPDATE CATEGORIA SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", categoria.Id);

            command.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "DELETE FROM CATEGORIA WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        public List<Categoria> ObtenerPaginado(int pageIndex, int pageSize)
        {
            var categorias = new List<Categoria>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Nombre, Descripcion FROM CATEGORIA
                                    ORDER BY Nombre
                                    LIMIT @PageSize OFFSET @Offset";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categorias.Add(MapearCategoria(reader));
            }
            return categorias;
        }

        public int ContarTotal()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM CATEGORIA";
            using var command = new MySqlCommand(query, connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<Categoria> Buscar(string termino, int maxResultados = 10)
        {
            var categorias = new List<Categoria>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT Id, Nombre, Descripcion FROM CATEGORIA
                                    WHERE Nombre LIKE @Termino
                                    ORDER BY Nombre
                                    LIMIT @Max";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Termino", $"%{termino}%");
            command.Parameters.AddWithValue("@Max", maxResultados);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categorias.Add(MapearCategoria(reader));
            }
            return categorias;
        }

        public bool EstaEnUso(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            const string query = @"SELECT
                                    (SELECT COUNT(*) FROM EQUIPO WHERE CategoriaId = @Id) +
                                    (SELECT COUNT(*) FROM SOLICITUD WHERE CategoriaId = @Id)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var total = Convert.ToInt32(command.ExecuteScalar());
            return total > 0;
        }

        private static Categoria MapearCategoria(MySqlDataReader reader)
        {
            return new Categoria
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString("Descripcion")
            };
        }
    }
}
