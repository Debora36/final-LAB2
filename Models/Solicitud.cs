using System.ComponentModel.DataAnnotations;
namespace final_LAB2.Models
{
    public class Solicitud
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El empleado es requerido")]
        public required int EmpleadoId { get; set; }
        [Required(ErrorMessage = "La categoría es requerida")]
        public required int CategoriaId { get; set; }
        public string? Motivo { get; set; }
        [Required(ErrorMessage = "El tiempo necesario es requerido")]
        public required string TiempoNecesario { get; set; }
        [Required(ErrorMessage = "La fecha de solicitud es requerida")]
        public required DateTime FechaSolicitud { get; set; }
        [Required(ErrorMessage = "El estado es requerido")]
        public required string Estado { get; set; }

        public override string ToString()
        {
            var solicitud = $"TiempoNecesario: {TiempoNecesario}, FechaSolicitud: {FechaSolicitud}";
            if (!string.IsNullOrEmpty(Motivo))
            {
                solicitud += $", Motivo: {Motivo}";
            }
            return solicitud;
        }
        
    }
}