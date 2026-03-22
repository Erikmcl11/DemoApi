using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoApi.Controllers
{
    [ApiController]
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
        /// Genera un token JWT. Acepta JSON, form-data o urlencoded.
        /// </summary>
        /// <returns>Token JWT válido por 1 hora.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            string? username, password;

            if (Request.ContentType?.Contains("application/json") == true)
            {
                var body = await Request.ReadFromJsonAsync<LoginRequest>();
                username = body?.Username;
                password = body?.Password;
            }
            else
            {
                username = Request.Form["username"];
                password = Request.Form["password"];
            }

            if (username != "admin" || password != "admin123")
                return Unauthorized("Credenciales incorrectas.");

            var token = GenerarToken(username!);
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
