using System.ComponentModel.DataAnnotations;
namespace final_LAB2.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres")]
        public string Username { get; set; } = string.Empty;
 
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
 
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}