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
}