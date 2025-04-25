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
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await _authService.AuthenticateUserAsync(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized("Credenciais inválidas.");
            }

            if (user.IsBlocked)
            {
                return Unauthorized("Usuário bloqueado.");
            } 

            await _authService.RevokeTokensAsync(user.Id);

            var refreshTokenDto = await _authService.SaveRefreshTokenAsync(user.Id);
            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new { Token = token, RefreshToken = refreshTokenDto.Token });
        }

        //esse metodo deve cria um novo refresh token a cada uso (RefreshToken) revogando o antigo
        [HttpPost("refreshtoken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO model)
        {
            var user = await _authService.GetUserByRefreshTokenAsync(model.RefreshToken);
            if (user == null)
            {
                return Unauthorized("Refresh token inválido.");
            }

            await _authService.RevokeRefreshTokenAsync(user.Id);

            // Revogar o refresh token atual
            var newRefreshToken = await _authService.SaveRefreshTokenAsync(user.Id);          

            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new { Token = token, RefreshToken = newRefreshToken });

        }     
    }
}
