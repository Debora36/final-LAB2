using System.ComponentModel.DataAnnotations;
namespace final_LAB2.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }

        public override string ToString()
        {
            var categoria =  $"Categoria: {Nombre}";
            if(!string.IsNullOrEmpty(Descripcion))
            {
                categoria += $", Descripcion: {Descripcion}";
            }
            return categoria;
        }
    }
}