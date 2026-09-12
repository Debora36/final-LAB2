using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using final_LAB2.Services.Interfaces;

namespace final_LAB2.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IEquipoRepository _equipoRepository;

        public PrestamoService(IPrestamoRepository prestamoRepository, IEquipoRepository equipoRepository)
        {
            _prestamoRepository = prestamoRepository;
            _equipoRepository = equipoRepository;
        }

        public Prestamo? ObtenerPorId(int id) => _prestamoRepository.ObtenerPorId(id);

        public (List<Prestamo> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize, string? estado = null)
        {
            var items = _prestamoRepository.ObtenerPaginado(pageIndex, pageSize, estado);
            var totalCount = _prestamoRepository.ContarTotal(estado);
            return (items, totalCount);
        }

        public void Crear(Prestamo prestamo)
        {
            prestamo.FechaPrestamo = DateTime.Now;
            _prestamoRepository.Agregar(prestamo);

            // Cambia el estado del equipo a Prestado automáticamente
            var equipo = _equipoRepository.ObtenerPorId(prestamo.EquipoId);
            if (equipo != null)
            {
                equipo.Estado = "Prestado";
                _equipoRepository.Actualizar(equipo);
            }
        }

        public void RegistrarDevolucion(int id)
        {
            var prestamo = _prestamoRepository.ObtenerPorId(id);
            if (prestamo == null)
                throw new InvalidOperationException("El préstamo no existe.");

            if (prestamo.FechaDevolucionReal.HasValue)
                throw new InvalidOperationException("Este préstamo ya fue devuelto.");

            prestamo.FechaDevolucionReal = DateTime.Now;
            _prestamoRepository.Actualizar(prestamo);

            // Cambia el estado del equipo a Disponible automáticamente
            var equipo = _equipoRepository.ObtenerPorId(prestamo.EquipoId);
            if (equipo != null)
            {
                equipo.Estado = "Disponible";
                _equipoRepository.Actualizar(equipo);
            }
        }

        public (List<Prestamo> Items, int TotalCount) ObtenerPaginadoPorDni(int pageIndex, int pageSize, string dni)
        {
            var items = _prestamoRepository.ObtenerPaginadoPorDni(pageIndex, pageSize, dni);
            var totalCount = _prestamoRepository.ContarTotalPorDni(dni);
            return (items, totalCount);
        }

        public (List<Prestamo> Items, int TotalCount) ObtenerPaginadoPorEmpleado(int pageIndex, int pageSize, int empleadoId)
        {
            var items = _prestamoRepository.ObtenerPaginadoPorEmpleado(pageIndex, pageSize, empleadoId);
            var totalCount = _prestamoRepository.ContarTotalPorEmpleado(empleadoId);
            return (items, totalCount);
        }

        public List<Prestamo> ObtenerVencidos()
        {
            return _prestamoRepository.ObtenerVencidos();
        }
    }
}