using final_LAB2.Services.Interfaces;

namespace final_LAB2.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GuardarArchivo(IFormFile archivo, string subCarpeta)
        {

            var carpeta = Path.Combine(_environment.WebRootPath, "Uploads", subCarpeta);
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            return $"/Uploads/{subCarpeta}/{nombreArchivo}";
        }

        public void EliminarArchivo(string? rutaRelativa) 
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa)) return;

            var rutaFisica = Path.Combine(_environment.WebRootPath,
                rutaRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                
            if (File.Exists(rutaFisica))
                File.Delete(rutaFisica);
        }
    }
}