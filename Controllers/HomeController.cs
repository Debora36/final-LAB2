using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using final_LAB2.Models;
using MySqlConnector;

namespace final_LAB2.Controllers;

public class HomeController : Controller
{
    // Interfaz que nos permite leer el appsettings.json
    private readonly IConfiguration _configuracion;

    // Inyectamos la configuración a través del constructor
        public HomeController(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        // Acción para probar la conexión
        public IActionResult ProbarConexion()
        {
            // 1. Leemos la cadena de conexión desde el appsettings.json
            string cadenaConexion = _configuracion.GetConnectionString("MySql");

            // 2. Creamos el objeto de conexión de ADO.NET
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    // 3. Intentamos abrir la conexión a la base de datos
                    conexion.Open();
                    
                    // Si llega a esta línea sin saltar al catch, ¡fue un éxito!
                    return Content("¡Conexión EXITOSA a la base de datos MySQL usando ADO.NET!");
                }
                catch (Exception ex)
                {
                    // Si ocurre algún error (contraseña mal, MySQL apagado, etc.), lo mostramos
                    return Content($" Falló la conexión. Error: {ex.Message}");
                }
            } // El bloque 'using' se encarga de hacer 'conexion.Close()' automáticamente al terminar.
        }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
