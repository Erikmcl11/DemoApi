using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Genera un token JWT enviando JSON.
        /// </summary>
        /// <param name="request">Credenciales de usuario.</param>
        /// <returns>Token JWT válido por 1 hora.</returns>
        [HttpPost("login")]
        [Consumes("application/json")]
        public IActionResult LoginJson([FromBody] LoginRequest request)
        {
            return Autenticar(request.Username, request.Password);
        }

        /// <summary>
        /// Genera un token JWT enviando form-data o urlencoded.
        /// </summary>
        [HttpPost("login")]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult LoginForm([FromForm] LoginRequest request)
        {
            return Autenticar(request.Username, request.Password);
        }

        private IActionResult Autenticar(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest("username y password son requeridos.");

            if (username != "admin" || password != "admin123")
                return Unauthorized("Credenciales incorrectas.");

            var token = GenerarToken(username);
            return Ok(new { token });
        }

        private string GenerarToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public record LoginRequest(string Username, string Password);
}
