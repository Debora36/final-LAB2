using Microsoft.Extensions.Configuration;
 
namespace final_LAB2.Repository
{
    public abstract class RepositorioBase
    {
        protected readonly string connectionString;
 
        protected RepositorioBase(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection' en appsettings.json.");
        }
    }
}