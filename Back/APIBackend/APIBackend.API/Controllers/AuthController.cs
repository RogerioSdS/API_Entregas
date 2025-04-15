using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace APIBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService autService, IConfiguration configuration)
        {
            _configuration = configuration;
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
            
            if (user.IsBlocked)
            {
                return Unauthorized("Usuário bloqueado.");
            }

            //iniciar o fluxo de gerar tokenJWT e refresh token

            if(await ValidateRefreshToken())
            // Gerar o JWT
            var token = await _authService.GenerateJwtTokenAsync(user);
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
        public async Task<IActionResult> ValidateRefreshToken([FromBody] RefreshTokenDTO model)
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

        // Método para gerar o refresh token (adicione isso ao AuthService)
        private string GenerateRefreshToken()
        {
            // Cria uma string única e segura usando GUID (alternativa: use System.Security.Cryptography para bytes aleatórios)
            var randomBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
            var refreshToken = Convert.ToBase64String(randomBytes);
            return refreshToken;

        }

        // Exemplo de uso no login (em AuthenticateUserAsync ou endpoint de login)
        public async Task<(string JwtToken, string RefreshToken)> CreateRefreshToken(string email, string password)
        {
            // Valida o usuário (como já faz em AuthenticateUserAsync)
            var userDTO = await AuthenticateUserAsync(email, password);
            if (userDTO == null)
            {
                return (null, null); // Falha na autenticação
            }

            // Gera o JWT
            var jwtToken = await GenerateJwtTokenAsync(userDTO);

            // Gera o refresh token
            var refreshToken = GenerateRefreshToken();

            // Salva o refresh token no banco com userId e expiração (usa seu método existente)
            await SaveRefreshTokenAsync(userDTO.Id, refreshToken);

            // Retorna ambos para o cliente
            return (jwtToken, refreshToken);
        }
    }
}
