using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Application.DTOs;

namespace APIBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService autService)
        {
            _authService = autService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar credenciais (ex.: e-mail e senha)
            var user = await _authService.AuthenticateUserAsync(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized("Credenciais inválidas.");
            }

            // Gerar o JWT
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        // Método auxiliar para gerar o JWT
        private string GenerateJwtToken(UserDTO user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role ?? "user") // Inclui a role (ex.: "admin" ou "user")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Access token expira em 30 minutos
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO model)
        {
            var userId = await _authService.ValidateRefreshTokenAsync(model.RefreshToken);
            if (userId == null)
            {
                return Unauthorized("Refresh token inválido.");
            }

            var user = await _authService.GetUserByIdAsync(userId.Value);
            var newToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            await _authService.SaveRefreshTokenAsync(user.Id, newRefreshToken);

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
        }
    }
}
