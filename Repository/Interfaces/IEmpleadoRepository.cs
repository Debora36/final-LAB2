using final_LAB2.Models;

namespace final_LAB2.Repository.Interfaces
{
    public interface IEmpleadoRepository
    {
        Empleado? ObtenerPorId(int id);
        List<Empleado> ObtenerTodos();
        List<Empleado> ObtenerPaginado(int pageIndex, int pageSize);
        int ContarTotal();

        // Cuentas con Rol='Empleado' que todavía no están vinculadas a ningún Empleado.
        // usuarioIdActual permite incluir la cuenta ya asignada al editar (para no "perderla" del combo).
        List<Usuario> ObtenerUsuariosElegibles(int usuarioIdActual = 0);

        void Agregar(Empleado empleado);
        void Actualizar(Empleado empleado);
        void Desactivar(int id);
    }
}
