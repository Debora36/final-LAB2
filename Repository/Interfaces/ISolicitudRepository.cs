using final_LAB2.Models;

namespace final_LAB2.Repository.Interfaces
{
    public interface ISolicitudRepository : IRepositorio<Solicitud>
    {
        List<Solicitud> ObtenerPaginado(int pageIndex, int pageSize, string? estado = null);
        int ContarTotal(string? estado = null);

        List<Solicitud> ObtenerPaginadoPorEmpleado(int pageIndex, int pageSize, int empleadoId, string? estado = null);
        int ContarTotalPorEmpleado(int empleadoId, string? estado = null);

        void Eliminar(int id);

    }
}
