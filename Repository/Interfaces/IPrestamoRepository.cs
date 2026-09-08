using final_LAB2.Models;

namespace final_LAB2.Repository.Interfaces
{
    public interface IPrestamoRepository : IRepositorio<Prestamo>
    {
        List<Prestamo> ObtenerPaginado(int pageIndex, int pageSize, string? estado = null);
        int ContarTotal();
        List<Prestamo> ObtenerPaginadoPorDni(int pageIndex, int pageSize, string dni);
        int ContarTotalPorDni(string dni);
        List<Prestamo> ObtenerPaginadoPorEmpleado(int pageIndex, int pageSize, int empleadoId);
        int ContarTotalPorEmpleado(int empleadoId);
    }
}