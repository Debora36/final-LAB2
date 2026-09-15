using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using final_LAB2.Services.Interfaces;
using System.Transactions;

namespace final_LAB2.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IEquipoRepository _equipoRepository;
        private readonly ISolicitudService _solicitudService;

        public PrestamoService(IPrestamoRepository prestamoRepository, IEquipoRepository equipoRepository, ISolicitudService solicitudService)
        {
            _prestamoRepository = prestamoRepository;
            _equipoRepository = equipoRepository;
            _solicitudService = solicitudService;
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
            var equipo = _equipoRepository.ObtenerPorId(prestamo.EquipoId);
            if (equipo == null || equipo.Estado != "Disponible")
            {
                throw new InvalidOperationException("El equipo seleccionado no está disponible para préstamo.");
            }

            _prestamoRepository.Agregar(prestamo);

            equipo.Estado = "Prestado";
            _equipoRepository.Actualizar(equipo);
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

            // Cambio el estado del equipo a Disponible
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


        public void AprobarSolicitud(int solicitudId, int equipoId, DateTime? fechaDevolucionEstimada)
        {
            var solicitud = _solicitudService.ObtenerPorId(solicitudId);
            if (solicitud == null)
                throw new InvalidOperationException("Solicitud no encontrada.");

            if (solicitud.Estado != "Pendiente")
                throw new InvalidOperationException("Solo se pueden aprobar solicitudes pendientes.");

            // Envuelvo las operaciones en una transacción
            using (var scope = new TransactionScope())
            {
                var prestamo = new Prestamo
                {
                    EquipoId = equipoId,
                    EmpleadoId = solicitud.EmpleadoId,
                    FechaPrestamo = DateTime.Now,
                    FechaDevolucionEstimada = fechaDevolucionEstimada
                };

                Crear(prestamo); 
                
                _solicitudService.CambiarEstado(solicitudId, "Aprobada");

                //si no hubo errores confirmao la transacción
                scope.Complete();
            }
        }
    }
}