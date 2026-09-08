using final_LAB2.Models;

namespace final_LAB2.Services.Interfaces
{
    public interface IPrestamoService
    {
        Prestamo? ObtenerPorId(int id);
        (List<Prestamo> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize, string? estado = null);
        void Crear(Prestamo prestamo);
        void RegistrarDevolucion(int id);
        (List<Prestamo> Items, int TotalCount) ObtenerPaginadoPorDni(int pageIndex, int pageSize, string dni);
        (List<Prestamo> Items, int TotalCount) ObtenerPaginadoPorEmpleado(int pageIndex, int pageSize, int empleadoId);
    }
}