using System.ComponentModel.DataAnnotations;
namespace final_LAB2.Models
{
    public class Equipo
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El modelo es requerido")]
        [StringLength(100, ErrorMessage = "El modelo no puede exceder los 100 caracteres")]
        public required string Modelo { get; set; }
        [Required(ErrorMessage = "El número de serie es requerido")]
        [StringLength(100, ErrorMessage = "El número de serie no puede exceder los 100 caracteres")]
        public required string NumeroSerie { get; set; }
        [Required(ErrorMessage = "El estado es requerido")]
        public required string Estado { get; set; }
        public string? RutaArchivoGarantia { get; set; }
        [Required(ErrorMessage = "La categoría es requerida")]
        public required int CategoriaId { get; set; }

        public override string ToString()
        {
            var equipo =  $"Categoria: {CategoriaId}, Modelo: {Modelo}, Número de Serie: {NumeroSerie}, Estado: {Estado}";
            if(!string.IsNullOrEmpty(RutaArchivoGarantia))
            {
                equipo += $", Ruta de Archivo de Garantía: {RutaArchivoGarantia}";
            }
            return equipo;
        }
    }
}