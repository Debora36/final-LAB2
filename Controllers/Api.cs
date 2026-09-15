using final_LAB2.Models;
using final_LAB2.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace final_LAB2.Controllers.Api
{
    [Route("api")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;

        public AuthApiController(IUsuarioService usuarioService, IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
        }

        // POST api/login
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginApiRequest request)
        {
            var usuario = _usuarioService.ValidarCredenciales(request.Username, request.Password);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });

            if (usuario.Rol != "Tecnico" && usuario.Rol != "Admin")
                return Forbid();

            var token = GenerarToken(usuario);
            return Ok(new { token });
        }

        private string GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(
            System.Text.Encoding.ASCII.GetBytes(
                _configuration["TokenAuthentication:SecretKey"]!));
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["TokenAuthentication:Issuer"],
            audience: _configuration["TokenAuthentication:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credenciales
        );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginApiRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


    [ApiController]
    [Route("api/prestamos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "TecnicoOAdmin")]
    public class PrestamosApiController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosApiController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        // GET api/prestamos?pageIndex=1&pageSize=20
        [HttpGet]
        public IActionResult GetPendientes(int pageIndex = 1, int pageSize = 20)
        {
            var (items, _) = _prestamoService.ObtenerPaginado(pageIndex, pageSize);
            var pendientes = items.Where(p => p.FechaDevolucionReal == null).ToList();
            return Ok(new { total = pendientes.Count, items = pendientes });
        }

        // POST api/prestamos/5/devolucion
        [HttpPost("{id}/devolucion")]
        public IActionResult RegistrarDevolucion(int id)
        {
            var prestamo = _prestamoService.ObtenerPorId(id);
            if (prestamo == null) return NotFound();

            try
            {
                _prestamoService.RegistrarDevolucion(id);
                return Ok(new { mensaje = "Devolución registrada" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET api/prestamos/vencidos
        [HttpGet("vencidos")]
        public IActionResult GetVencidos()
        {
            var vencidos = _prestamoService.ObtenerVencidos();
            return Ok(new { total = vencidos.Count, items = vencidos });
        }
    }


    [ApiController]
    [Route("api/solicitudes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "TecnicoOAdmin")]
    public class SolicitudesApiController : ControllerBase
    {
        private readonly ISolicitudService _solicitudService;

        public SolicitudesApiController(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        // GET api/solicitudes?pageIndex=1&pageSize=20
        [HttpGet]
        public IActionResult GetPendientes(int pageIndex = 1, int pageSize = 20)
        {
            var (items, total) = _solicitudService.ObtenerPaginado(pageIndex, pageSize, "Pendiente");
            return Ok(new { total, items });
        }

        [HttpPost("{id}/rechazar")]
        public IActionResult Rechazar(int id) => CambiarEstado(id, "Rechazada");

        private IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            var solicitud = _solicitudService.ObtenerPorId(id);
            if (solicitud == null) return NotFound();

            try
            {
                _solicitudService.CambiarEstado(id, nuevoEstado);
                return Ok(new { mensaje = $"Solicitud cambiada a '{nuevoEstado}'" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
    
}