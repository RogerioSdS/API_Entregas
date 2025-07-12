using Microsoft.AspNetCore.Mvc;
using Api_Entregas.ViewModels;
using Api_Entregas.Services.Interfaces;

namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IAuthService _authService;

        public LoginController(ILogger<LoginController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login inválido: ModelState com erro.");
                return View(model); // volta para a mesma página com os erros
            }

            var result = await _authService.LoginAsync(model);

            if (result.Success)
            {
                HttpContext.Session.SetString("Logado", "true");
                // Redirecionar para uma página segura após login
                return RedirectToAction("Index", "Home");
            }

            // Adiciona erro no ModelState para exibir na View
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Erro ao fazer login.");
            return View(model); // volta para o form com mensagem de erro
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState });
            }

            var result = await _authService.RegisterAsync(model);

            if (result.Success)
            {
                _logger.LogInformation("Registro bem-sucedido.");
                return Json(new 
                { 
                    success = true,
                    message = "Usuário registrado com sucesso!",
                    accessToken = result.Data.Token, 
                    refreshToken = result.Data.RefreshToken,
                    refreshTokenExpiresAt = result.Data.RefreshTokenExpiresAt
                });
            }

            return Json(new { success = false, error = result.ErrorMessage });
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

        [HttpGet("SignIn")]
        public IActionResult SignIn()
        {
            return View();
        }
/*
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel model)
        {
            // Implementar lógica de refresh token se necessário
            // Por enquanto, apenas um placeholder
            return Json(new { success = false, error = "RefreshToken não implementado ainda." });
        }
        */ 

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Para JWT, o logout é feito no frontend removendo o token
            // Aqui você pode implementar blacklist do token se necessário
            return Json(new { success = true, message = "Logout realizado com sucesso." });
        }

        [HttpGet("Error")]
        public IActionResult Error(ErrorViewModel model)
        {
            _logger.LogInformation("Método Error chamado. Renderizando a view Error.cshtml.");
            return View(model);
        }
    }
}