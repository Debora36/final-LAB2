using System.ComponentModel.DataAnnotations;
namespace final_LAB2.Models
{
public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "El rol es requerido")]
        public required string Rol { get; set; }
        public string? AvatarUrl { get; set; }
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres y no puede exceder los 255 caracteres")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public required string Email { get; set; }
        public bool Activo { get; set; } // true para activo, false para inactivo

        public override string ToString()
        {
            var usuario = $"Username: {Username}, Rol: {Rol}, Email: {Email}";
            if (!string.IsNullOrEmpty(AvatarUrl))
            {
                usuario += $", AvatarUrl: {AvatarUrl}";
            }
            return usuario;
        }
    }
}