using System.ComponentModel.DataAnnotations;
namespace final_LAB2.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
        public required string Apellido { get; set; }
        [Required(ErrorMessage = "El DNI es requerido")]
        [StringLength(10, ErrorMessage = "El DNI no puede exceder los 10 caracteres")]
        public required string DNI { get; set; }
        [Phone(ErrorMessage = "El número de teléfono no es válido")]
        public string? Telefono { get; set; }
        public int? UsuarioId { get; set; }
        public bool Activo { get; set; } = true;
        public override string ToString()
        {
            var empleado =  $"Empleado: {Nombre} {Apellido}, DNI: {DNI}";
            if(!string.IsNullOrEmpty(Telefono))
            {
                empleado += $", Teléfono: {Telefono}";
            }
            return empleado;
        }
    }
}