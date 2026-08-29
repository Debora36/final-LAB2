using final_LAB2.Models;
 
namespace final_LAB2.Repository.Interfaces
{
    public interface IEquipoRepository : IRepositorio<Equipo>
    {
        List<Equipo> ObtenerPaginado(int pageIndex, int pageSize, string? estado, int? categoriaId);
        int ContarTotal(string? estado, int? categoriaId);
        bool NumeroSerieExiste(string numeroSerie, int idExcluir = 0);
        void DarDeBaja(int id);
    }
}
 