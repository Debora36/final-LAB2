using final_LAB2.Models;
namespace final_LAB2.Models.ViewModels
{
    public class SolicitudViewModel
    {
        public Solicitud Solicitud { get; set; } = null!;
        public Empleado Empleado { get; set; } = null!;
        public Categoria Categoria { get; set; } = null!;
    }
}