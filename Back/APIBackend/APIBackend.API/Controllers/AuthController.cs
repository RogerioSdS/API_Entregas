using Microsoft.AspNetCore.Mvc;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using NLog;

namespace APIBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();

        public AuthController(IAuthService autService, IConfiguration configuration, ILogger<AuthController> nlog)
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

            _loggerNLog.Info($"Usuario logado com sucesso: {user.FirstName + " " + user.LastName} - {user.Email}");

            await _authService.RevokeTokensAsync(user.Id);

            var refreshTokenDto = await _authService.SaveRefreshTokenAsync(user);
            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new { Token = token, RefreshToken = refreshTokenDto.Token });
        }

        //esse metodo deve criar um novo refresh token a cada uso (RefreshToken) revogando o antigo
        [HttpPost("refreshtoken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO model)
        {
            var user = await _authService.GetUserByRefreshTokenAsync(model.RefreshToken);
            if (user == null)
            {
                return Unauthorized("Refresh token inválido.");
            }

            var userWithRoles = await _authService.GetUserRolesAsync(user);
            if (userWithRoles == null)
            {
                return Unauthorized("Problema na validação do usuario atraves do Refresh Token");
            }

           await _authService.RevokeTokensAsync(userWithRoles.Id);

            var refreshTokenDto = await _authService.SaveRefreshTokenAsync(userWithRoles);
            var token = await _authService.GenerateJwtTokenAsync(userWithRoles);

            return Ok(new { Token = token, RefreshToken = refreshTokenDto.Token });
        }     
    }
}
