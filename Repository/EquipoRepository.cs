using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Text;
 
namespace final_LAB2.Repository
{
    public class EquipoRepository : RepositorioBase, IEquipoRepository
    {
        public EquipoRepository(IConfiguration configuration) : base(configuration)
        {
        }
 
        public Equipo? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"SELECT e.Id, e.Modelo, e.NumeroSerie, e.Estado, e.RutaArchivoGarantia, e.CategoriaId, c.Nombre AS CategoriaNombre
                                    FROM EQUIPO e
                                    JOIN CATEGORIA c ON c.Id = e.CategoriaId
                                    WHERE e.Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
 
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearEquipo(reader) : null;
        }
 
        public List<Equipo> ObtenerTodos()
        {
            var equipos = new List<Equipo>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"SELECT e.Id, e.Modelo, e.NumeroSerie, e.Estado, e.RutaArchivoGarantia, e.CategoriaId, c.Nombre AS CategoriaNombre
                                    FROM EQUIPO e
                                    JOIN CATEGORIA c ON c.Id = e.CategoriaId
                                    ORDER BY e.Modelo";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
 
            while (reader.Read())
            {
                equipos.Add(MapearEquipo(reader));
            }
            return equipos;
        }
 
        public List<Equipo> ObtenerPaginado(int pageIndex, int pageSize, string? estado, int? categoriaId)
        {
            var equipos = new List<Equipo>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            var (whereClause, parametros) = ArmarFiltro(estado, categoriaId);
 
            var query = new StringBuilder(@"SELECT e.Id, e.Modelo, e.NumeroSerie, e.Estado, e.RutaArchivoGarantia, e.CategoriaId, c.Nombre AS CategoriaNombre
                                              FROM EQUIPO e
                                              JOIN CATEGORIA c ON c.Id = e.CategoriaId");
            query.Append(whereClause);
            query.Append(" ORDER BY e.Modelo LIMIT @PageSize OFFSET @Offset");
 
            using var command = new MySqlCommand(query.ToString(), connection);
            foreach (var p in parametros)
            {
                command.Parameters.AddWithValue(p.Key, p.Value);
            }
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
 
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                equipos.Add(MapearEquipo(reader));
            }
            return equipos;
        }
 
        public int ContarTotal(string? estado, int? categoriaId)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            var (whereClause, parametros) = ArmarFiltro(estado, categoriaId);
 
            var query = new StringBuilder("SELECT COUNT(*) FROM EQUIPO e");
            query.Append(whereClause);
 
            using var command = new MySqlCommand(query.ToString(), connection);
            foreach (var p in parametros)
            {
                command.Parameters.AddWithValue(p.Key, p.Value);
            }
 
            return Convert.ToInt32(command.ExecuteScalar());
        }
 
        public bool NumeroSerieExiste(string numeroSerie, int idExcluir = 0)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = "SELECT COUNT(*) FROM EQUIPO WHERE NumeroSerie = @NumeroSerie AND Id <> @IdExcluir";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@NumeroSerie", numeroSerie);
            command.Parameters.AddWithValue("@IdExcluir", idExcluir);
 
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
 
        public void Agregar(Equipo equipo)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"INSERT INTO EQUIPO (Modelo, NumeroSerie, Estado, RutaArchivoGarantia, CategoriaId)
                                    VALUES (@Modelo, @NumeroSerie, @Estado, @RutaArchivoGarantia, @CategoriaId)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Modelo", equipo.Modelo);
            command.Parameters.AddWithValue("@NumeroSerie", equipo.NumeroSerie);
            command.Parameters.AddWithValue("@Estado", equipo.Estado);
            command.Parameters.AddWithValue("@RutaArchivoGarantia", (object?)equipo.RutaArchivoGarantia ?? DBNull.Value);
            command.Parameters.AddWithValue("@CategoriaId", equipo.CategoriaId);
 
            command.ExecuteNonQuery();
        }
 
        public void Actualizar(Equipo equipo)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = @"UPDATE EQUIPO
                                    SET Modelo = @Modelo, NumeroSerie = @NumeroSerie, Estado = @Estado,
                                        RutaArchivoGarantia = @RutaArchivoGarantia, CategoriaId = @CategoriaId
                                    WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Modelo", equipo.Modelo);
            command.Parameters.AddWithValue("@NumeroSerie", equipo.NumeroSerie);
            command.Parameters.AddWithValue("@Estado", equipo.Estado);
            command.Parameters.AddWithValue("@RutaArchivoGarantia", (object?)equipo.RutaArchivoGarantia ?? DBNull.Value);
            command.Parameters.AddWithValue("@CategoriaId", equipo.CategoriaId);
            command.Parameters.AddWithValue("@Id", equipo.Id);
 
            command.ExecuteNonQuery();
        }
 
        public void DarDeBaja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
 
            const string query = "UPDATE EQUIPO SET Estado = 'Baja' WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
 
        private static (string WhereClause, Dictionary<string, object> Parametros) ArmarFiltro(string? estado, int? categoriaId)
        {
            var condiciones = new List<string>();
            var parametros = new Dictionary<string, object>();
 
            if (!string.IsNullOrWhiteSpace(estado))
            {
                condiciones.Add("e.Estado = @Estado");
                parametros["@Estado"] = estado;
            }
 
            if (categoriaId.HasValue)
            {
                condiciones.Add("e.CategoriaId = @CategoriaId");
                parametros["@CategoriaId"] = categoriaId.Value;
            }
 
            var whereClause = condiciones.Count > 0 ? " WHERE " + string.Join(" AND ", condiciones) : "";
            return (whereClause, parametros);
        }
 
        private static Equipo MapearEquipo(MySqlDataReader reader)
        {
            return new Equipo
            {
                Id = reader.GetInt32("Id"),
                Modelo = reader.GetString("Modelo"),
                NumeroSerie = reader.GetString("NumeroSerie"),
                Estado = reader.GetString("Estado"),
                RutaArchivoGarantia = reader.IsDBNull(reader.GetOrdinal("RutaArchivoGarantia")) ? null : reader.GetString("RutaArchivoGarantia"),
                CategoriaId = reader.GetInt32("CategoriaId"),
                CategoriaNombre = reader.GetString("CategoriaNombre")
            };
        }
    }
}