using System.ComponentModel.DataAnnotations;
namespace final_LAB2.Models
{
    public class Prestamo
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El equipo es requerido")]
        public required int EquipoId { get; set; }
        [Required(ErrorMessage = "El empleado es requerido")]
        public required int EmpleadoId { get; set; }
        [Required(ErrorMessage = "La fecha de préstamo es requerida")]
        public required DateTime FechaPrestamo { get; set; }
        public DateTime? FechaDevolucionEstimada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }

        public override string ToString()
        {
            var prestamo =  $"EquipoId: {EquipoId}, EmpleadoId: {EmpleadoId}, Fecha de Préstamo: {FechaPrestamo}";
            if(FechaDevolucionEstimada.HasValue)
            {
                prestamo += $", Fecha de Devolución Estimada: {FechaDevolucionEstimada.Value}";
            }
            if(FechaDevolucionReal.HasValue)
            {
                prestamo += $", Fecha de Devolución Real: {FechaDevolucionReal.Value}";
            }
            return prestamo;
        }
    }
}