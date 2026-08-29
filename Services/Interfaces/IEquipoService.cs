using final_LAB2.Models;

namespace final_LAB2.Services.Interfaces
{
    public interface IEquipoService
    {
        Equipo? ObtenerPorId(int id);
        (List<Equipo> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize, string? estado, int? categoriaId);
        void Crear(Equipo equipo);   // lanza InvalidOperationException si el número de serie ya existe
        void Actualizar(Equipo equipo);
        void DarDeBaja(int id);
    }
}
