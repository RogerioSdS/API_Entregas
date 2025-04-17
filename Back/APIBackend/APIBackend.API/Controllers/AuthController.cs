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
            var valideLogin = await _authService.ValidateRefreshTokenAsync(user.Id);
            if(valideLogin == null)
            {
                if (valideLogin?.ExpiresAt < DateTime.UtcNow || string.IsNullOrEmpty(valideLogin?.Token))
                {
                    await _authService.SaveRefreshTokenAsync(user.Id);
                }
            }

            var userRefreshTokenDTO = await _authService.GetRefreshTokenByIdAsync(user.Id);

            if (userRefreshTokenDTO.IsRevoked)
            {
                return Unauthorized("Refresh token inválido. Está revogado.");
            }

            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new { Token = token, RefreshToken = userRefreshTokenDTO.Token });
        }
    }
}
