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

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _configuration = configuration;
            _authService = authService;
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

        [HttpPost("GenerateEmailConfirmationToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateEmailConfirmationToken([FromBody] EmailDTO model)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var urlToTokenConfirmedEmail = await _authService.CreatedEmailConfirmationAsync(model);
            if (urlToTokenConfirmedEmail == null)
            {
                return BadRequest("Erro ao gerar o token de confirmação de e-mail.");
            }
            if (urlToTokenConfirmedEmail.Contains("E-mail já estava confirmado"))
            {
               _loggerNLog.Info($"E-mail já estava confirmado: {model.Email}");
               return BadRequest("E-mail já estava confirmado.");
            }

            _loggerNLog.Info($"Token de confirmação de e-mail gerado com sucesso para: {model.Email} usando o tokenUrl: {urlToTokenConfirmedEmail}");

            return Ok(new { Token = urlToTokenConfirmedEmail });
        }

        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmedEmailToken([FromQuery] string email, string token)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            try
            {
                var result = await _authService.ConfirmEmailAsync(email, token);
                if (!result)
                {
                    Console.WriteLine($"Erro ao confirmar token: {token}");
                    _loggerNLog.Info($"Token invalido para confirmar o e-mail: {email}");
                    return BadRequest("Token inválido.");
                }

                Console.WriteLine($"Confirmando e-mail: {email}");

                return Ok("Email confirmado com sucesso.");
            }
            catch (System.Exception)
            {
                Console.WriteLine($"Erro ao confirmar e-mail: {email}");
                return BadRequest("Erro ao confirmar o e-mail.");
            }
        }

        [HttpPost("GenerateResetPasswordToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateResetPasswordToken([FromBody] EmailDTO model)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var urlToTokenConfirmedEmail = await _authService.CreatedResetPasswordTokenAsync(model);
            if (urlToTokenConfirmedEmail == null)
            {
                return BadRequest("Erro ao gerar o token de redefinição de senha.");
            }

            _loggerNLog.Info($"Token de redefinição de senha gerado com sucesso para: {model.Email} tokenUrl: {urlToTokenConfirmedEmail}");

            return Ok(new { Token = urlToTokenConfirmedEmail });
        }        

        [HttpPost("ConfirmResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] string email, string token, string newPassword)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            try
            {
                var result = await _authService.ConfirmResetPasswordAsync(email, token, newPassword);
                if (!result)
                {
                    Console.WriteLine($"Erro ao confirmar token: {token}");
                    _loggerNLog.Info($"Token invalido para redefinir a senha do e-mail: {email}");
                    return BadRequest("Token inválido.");
                }

                Console.WriteLine($"Redefinindo senha para o e-mail: {email}");

                return Ok("Senha redefinida com sucesso.");
            }
            catch (System.Exception)
            {
                Console.WriteLine($"Erro ao redefinir senha para o e-mail: {email}");
                return BadRequest("Erro ao redefinir a senha.");
            }
        }
    }
}
