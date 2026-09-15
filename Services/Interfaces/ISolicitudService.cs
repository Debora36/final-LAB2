using final_LAB2.Models;

namespace final_LAB2.Services.Interfaces
{
    public interface ISolicitudService
    {
        Solicitud? ObtenerPorId(int id);
        (List<Solicitud> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize, string? estado = null);
        void Crear(Solicitud solicitud);
        void Actualizar(Solicitud solicitud);
        void CambiarEstado(int id, string nuevoEstado);
        List<Solicitud> ObtenerTodas();
        (List<Solicitud> Items, int TotalCount) ObtenerPaginadoPorEmpleado(int pageIndex, int pageSize, int empleadoId, string? estado = null);
        void Eliminar(int id, int empleadoId);
    }
}
