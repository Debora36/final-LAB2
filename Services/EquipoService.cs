using final_LAB2.Models;
using final_LAB2.Repository.Interfaces;
using final_LAB2.Services.Interfaces;

namespace final_LAB2.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IEquipoRepository _equipoRepository;

        public EquipoService(IEquipoRepository equipoRepository)
        {
            _equipoRepository = equipoRepository;
        }

        public Equipo? ObtenerPorId(int id) => _equipoRepository.ObtenerPorId(id);

        public (List<Equipo> Items, int TotalCount) ObtenerPaginado(int pageIndex, int pageSize, string? estado, int? categoriaId)
        {
            var items = _equipoRepository.ObtenerPaginado(pageIndex, pageSize, estado, categoriaId);
            var total = _equipoRepository.ContarTotal(estado, categoriaId);
            return (items, total);
        }

        public void Crear(Equipo equipo)
        {
            if (_equipoRepository.NumeroSerieExiste(equipo.NumeroSerie))
            {
                throw new InvalidOperationException("Ya existe un equipo con ese número de serie.");
            }
            _equipoRepository.Agregar(equipo);
        }

        public void Actualizar(Equipo equipo, string? nuevaRutaGarantia = null, bool eliminarGarantia = false)
        {
            var equipoActual = _equipoRepository.ObtenerPorId(equipo.Id);
            if (equipoActual == null)
            {
                throw new InvalidOperationException("Equipo no encontrado.");
            }

            if (_equipoRepository.NumeroSerieExiste(equipo.NumeroSerie, equipo.Id))
            {
                throw new InvalidOperationException("Ya existe otro equipo con ese número de serie.");
            }

            equipoActual.Modelo = equipo.Modelo;
            equipoActual.NumeroSerie = equipo.NumeroSerie;
            equipoActual.CategoriaId = equipo.CategoriaId;
            equipoActual.Estado = equipo.Estado;

            if (nuevaRutaGarantia != null)
            {
                equipoActual.RutaArchivoGarantia = nuevaRutaGarantia;
            }
            else if (eliminarGarantia)
            {
                equipoActual.RutaArchivoGarantia = null;
            }

            _equipoRepository.Actualizar(equipoActual);
        }

        public void DarDeBaja(int id) => _equipoRepository.DarDeBaja(id);
        public List<Equipo> ObtenerPorEstado(string estado)
        {
            return _equipoRepository.ObtenerPorEstado(estado);
        }

        public List<Equipo> BuscarDisponibles(string? termino, int categoriaId)
        {
            return _equipoRepository.BuscarDisponibles(termino, categoriaId);
        }
    }
}
