using Microsoft.AspNetCore.Mvc;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using NLog;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            
            RefreshTokenDTO? foundRefreshToken = await _authService.GetValidateRefreshTokenByIdAsync(user.Id);

            if (foundRefreshToken == null)
            {
                await _authService.RevokeTokensAsync(user.Id);
                foundRefreshToken = await _authService.SaveRefreshTokenAsync(user);
            }
            
            var token = await _authService.GenerateJwtTokenAsync(user);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, // 👈 Obrigatório para cross-site
                Expires = DateTime.UtcNow.AddMinutes(15)
            };
            Response.Cookies.Append("access_token", token, cookieOptions);

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, // 👈 Também precisa ser None
                Expires = foundRefreshToken.ExpiresAt
            };
            Response.Cookies.Append("refresh_token", foundRefreshToken.Token, refreshCookieOptions);

            return Ok(new { message = "Login realizado com sucesso" });
        }

        [HttpPost("loginByRefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginRefreshToken([FromBody] RefreshTokenRequestDTO model)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (!await _authService.ValidateRefreshTokenAsync(model.RefreshToken))
            {
                return Unauthorized("Refresh token inválido ou expirado.");
            }

            var foundUser = await _authService.GetUserByRefreshTokenAsync(model.RefreshToken);
            if (foundUser == null)
            {
                return Unauthorized("Refresh token inválido.");
            }

            var userWithRoles = await _authService.GetUserRolesAsync(foundUser);
            var token = await _authService.GenerateJwtTokenAsync(userWithRoles);

            return Ok(new { message = "Login realizado com sucesso" });
        }

        [HttpPost("generateRefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] RefreshTokenRequestDTO model)
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

            // return Ok(new { Token = token, RefreshToken = refreshTokenDto.Token });
            return Ok(new { message = "Login realizado com sucesso" });
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

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Deleta os cookies
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");            

            // Lê o refresh token diretamente do cookie
            var refreshToken = Request.Cookies["refresh_token"];

            Console.WriteLine("Refresh token: " + refreshToken);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Nenhum refresh token encontrado.");
            }

            var foundRefreshToken = await _authService.GetTokenByRefreshTokenAsync(refreshToken);
            Console.WriteLine("Found refresh token: " + foundRefreshToken.Id);
            if (foundRefreshToken == null)
            {
                return BadRequest("Refresh token inválido.");
            }

            await _authService.RevokeRefreshTokenAsync(foundRefreshToken.Id);
            Console.WriteLine("Refresh token revogado: " + foundRefreshToken.Id);

            return Ok(new { message = "Logout realizado com sucesso" });
        }
    }
}
