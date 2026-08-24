using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using final_LAB2.Services.Interfaces;

namespace final_LAB2.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;

        public EmpleadoService(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }

        public Empleado? ObtenerPorId(int id) => _empleadoRepository.ObtenerPorId(id);

        public (List<Empleado> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize)
        {
            var items = _empleadoRepository.ObtenerPaginado(pageIndex, pageSize);
            var total = _empleadoRepository.ContarTotal();
            return (items, total);
        }

        public List<Usuario> ObtenerUsuariosElegibles(int usuarioIdActual = 0)
            => _empleadoRepository.ObtenerUsuariosElegibles(usuarioIdActual);

        public void Crear(Empleado empleado)
        {
            empleado.Activo = true;
            _empleadoRepository.Agregar(empleado);
        }

        public void Actualizar(Empleado empleado) => _empleadoRepository.Actualizar(empleado);

        public void Desactivar(int id) => _empleadoRepository.Desactivar(id);
    }
}
