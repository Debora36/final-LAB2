using final_LAB2.Models;

namespace final_LAB2.Services.Interfaces
{
    public interface IEmpleadoService
    {
        Empleado? ObtenerPorId(int id);
        (List<Empleado> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize);
        List<Usuario> ObtenerUsuariosElegibles(int usuarioIdActual = 0);
        void Crear(Empleado empleado);
        void Actualizar(Empleado empleado);
        void Desactivar(int id);  
        List<Empleado> ObtenerTodos();
    }
}
