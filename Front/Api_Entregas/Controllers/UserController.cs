using Api_Entregas.Services.Implementations;
using Api_Entregas.Services.Interfaces;
using Api_Entregas.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
         private readonly ILogger<UserController> _logger;
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;

        public UserController(ILogger<UserController> logger, IAuthService authService, ISessionService sessionService)
        {
            _logger = logger;
            _authService = authService;
            _sessionService = sessionService;
        }

         [HttpGet("Register")]
        public async Task<IActionResult> Register()
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

        [HttpGet("perfil")]
        public IActionResult Perfil()
        {
            var json = HttpContext.Session.GetString("UserData");
            if (json == null) return RedirectToAction("Error");

            var model = JsonConvert.DeserializeObject<UserViewModel>(json);
            return View("/Views/User/Perfil.cshtml", model); 
        }

        [HttpPost("perfil")]
        public IActionResult PerfilPost([FromBody] UserViewModel model)
        {            
            if (!ModelState.IsValid) return BadRequest();

            _sessionService.SetUserData("UserData", model); 
            return Ok(); // Apenas confirma
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string erro)
        {
            return View("Error", new ErrorViewModel { RequestId = erro });
        }
    }
}