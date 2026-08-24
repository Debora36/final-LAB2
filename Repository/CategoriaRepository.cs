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