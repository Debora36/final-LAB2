using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using final_LAB2.Services.Interfaces;

namespace final_LAB2.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _solicitudRepository;

        public SolicitudService(ISolicitudRepository solicitudRepository)
        {
            _solicitudRepository = solicitudRepository;
        }

        public List<Solicitud> ObtenerTodas()
        {
            return _solicitudRepository.ObtenerTodos();
        }

        public (List<Solicitud> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize, string? estado = null)
        {
            var items = _solicitudRepository.ObtenerPaginado(pageIndex, pageSize, estado);
            var totalCount = _solicitudRepository.ContarTotal();
            return (items, totalCount);
        }

        public Solicitud? ObtenerPorId(int id)
        {
            if (id <= 0) return null;
            return _solicitudRepository.ObtenerPorId(id);
        }

        public void Crear(Solicitud solicitud)
        {
            // Validaciones de negocio
            if (solicitud.EmpleadoId <= 0)
                throw new ArgumentException("Debe seleccionar un empleado válido.");

            if (solicitud.CategoriaId <= 0)
                throw new ArgumentException("Debe seleccionar una categoría válida.");

            // Reglas de negocio automáticas al crear
            solicitud.FechaSolicitud = DateTime.Now;
            if (string.IsNullOrWhiteSpace(solicitud.Estado))
            {
                solicitud.Estado = "Pendiente"; // Estado por defecto
            }

            _solicitudRepository.Agregar(solicitud);
        }

        public void Actualizar(Solicitud solicitud)
        {
            var solicitudExistente = _solicitudRepository.ObtenerPorId(solicitud.Id);
            if (solicitudExistente == null)
                throw new InvalidOperationException("La solicitud a actualizar no existe.");

            // No permitimos editar una solicitud ya procesada (Aprobada/Rechazada)
            if (solicitudExistente.Estado != "Pendiente")
                throw new InvalidOperationException("Solo se pueden modificar solicitudes en estado Pendiente.");

            _solicitudRepository.Actualizar(solicitud);
        }

        public void CambiarEstado(int id, string nuevoEstado)
        {
            var solicitud = _solicitudRepository.ObtenerPorId(id);
            if (solicitud == null)
                throw new InvalidOperationException("La solicitud especificada no existe.");

            solicitud.Estado = nuevoEstado;
            _solicitudRepository.Actualizar(solicitud);
        }
    }
}