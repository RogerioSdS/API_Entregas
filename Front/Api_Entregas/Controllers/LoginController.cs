using Microsoft.AspNetCore.Mvc;
using Api_Entregas.ViewModels;
using Api_Entregas.Services.Interfaces;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;

namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;

        public LoginController(ILogger<LoginController> logger, IAuthService authService, ISessionService sessionService)
        {
            _logger = logger;
            _authService = authService;
            _sessionService = sessionService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login inválido: ModelState com erro.");
                return View(model); // volta para a mesma página com os erros
            }
            // Simula sucesso
            _sessionService.SetUserData("UserDataLogin",new SignInViewModel { SignIn = true });

            return Json(new { redirectUrl = Url.Action("Index", "Home") });
        }
       
        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage ?? "Erro de validação.";
                return Json(new { success = false, error });
            }

            var userResult = await _authService.GetUserByEmailAsync(model.Email);
            if (!userResult.Success)
            {
                return Json(new { success = false, error = "Email não encontrado." });
            }

            var forgotPasswordResult = await _authService.ForgotPasswordAsync(model);
            if (forgotPasswordResult.Success)
            {
                return Json(new
                {
                    success = true,
                    message = forgotPasswordResult.Data,
                    redirectUrl = Url.Action("Index", "Home", null, Request.Scheme)
                });
            }

            return Json(new { success = false, error = forgotPasswordResult.ErrorMessage });
        }
        

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            // Para JWT, o logout é feito no frontend removendo o token
            // Aqui você pode implementar blacklist do token se necessário
            return View();
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> LogOff()
        {
            _sessionService.ClearUserData("UserDataLogin");

            return Json(new { message = "Logout realizado com sucesso!" });
        }

        [HttpPost("loginByRefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginRefreshToken()
        {
            var result = await _authService.LoginByRefreshTokenAsync();

            if (!result.Success)
            {
                Response.StatusCode = result.StatusCode;

                if (result.StatusCode == 401)
                {
                    Response.Headers["Token-Error"] = "refresh-expired";
                }

                return Json(new { message = result.ErrorMessage });
            }

            return Json(new { message = "Token renovado com sucesso!" });
        }


        [HttpGet("Error")]
        public IActionResult Error(ErrorViewModel model)
        {
            _logger.LogInformation("Método Error chamado. Renderizando a view Error.cshtml.");
            return View(model);
        }
    }
}