using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace final_LAB2.Services.Interfaces
{
    public interface IFileService
    {
        string GuardarArchivo(IFormFile archivo, string subCarpeta); 
        void EliminarArchivo(string? rutaRelativa);
    }
}