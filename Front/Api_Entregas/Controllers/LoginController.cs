using Microsoft.AspNetCore.Mvc;
using Api_Entregas.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<LoginController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string actionDemanded = "")
        {
            ViewBag.ActionDemanded = actionDemanded;  // Passar o valor para a View
            _logger.LogInformation($"Método Login (GET) chamado. actionDemanded = {actionDemanded}");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var loginPath = "/login";
                string apiUrl = _configuration["ApiBackendSettings:AuthUrl"] + loginPath ?? string.Empty;
                string jsonBody = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                _logger.LogInformation($"Fazendo requisição para {apiUrl}");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userData = JsonConvert.DeserializeObject<SignInViewModel>(responseContent);
                    Console.WriteLine(responseContent);
                    // Armazenar na sessão, se necessário
                    HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(userData));

                    _logger.LogInformation("Login bem-sucedido.");

                    // Retornar JSON com o JWT
                    return Json(new { accessToken = userData.Token, RefreshTokenExpiresAt = userData.RefreshTokenExpiresAt, refreshToken = userData.RefreshToken }); // Ajuste "Token" para o nome real da propriedade
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Erro na requisição para {apiUrl}. Status: {response.StatusCode}, Resposta: {errorContent}");
                    return BadRequest(new { error = "E-mail ou senha inválidos." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar o usuário.");
                return StatusCode(500, new { error = "Ocorreu um erro ao processar o login." });
            }
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userPath = "/createUser";
                    string apiUrl = _configuration["ApiBackendSettings:UserUrl"] + userPath ?? string.Empty;
                    string jsonBody = JsonConvert.SerializeObject(model);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    _logger.LogInformation($"Fazendo requisição para {apiUrl + jsonBody}");

                    // Fazer a requisição Post para a API de usuários
                    var response = await _httpClient.PostAsync(apiUrl, content);
                    TempData["FormData"] = $"Dados recebidos: Nome={model.FirstName}, Sobrenome={model.LastName}, Email={model.Email}, Telefone={model.Phone}, Endereço={model.Address}, Complemento={model.Complement}, Cidade={model.City}, CEP={model.Post}";

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var userData = JsonConvert.DeserializeObject<SignInViewModel>(responseContent);
                        //userData.Email = model.Email;
                        _logger.LogInformation($"Resposta da API: {responseContent}");
                        // Armazenar como JSON na sessão
                        HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(userData));
                        _logger.LogInformation("Redirecionando para SignIn após registro bem-sucedido.");
                        return RedirectToAction("SignIn");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Erro na requisição para {apiUrl}. Status: {response.StatusCode}, Resposta: {errorContent}");
                        ModelState.AddModelError("", "Erro ao registrar usuário.");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao registrar o usuário.");
                    ModelState.AddModelError("", "Ocorreu um erro ao processar o registro. Tente novamente.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage ?? "Erro de validação.";

                //aqui estou retornado um JSON para o front-end para que o Ajax possa tratar a resposta
                return Json(new { success = false, error });
            }

            var foundUserByEmail = await GetUserByEmail(model.Email);

            if (foundUserByEmail == null)
            {
                return Json(new { success = false, error = "Email nao encontrado." });
            }

            var forgotPasswordPath = "/GenerateResetPasswordToken";
            string apiUrl = _configuration["ApiBackendSettings:AuthUrl"] + forgotPasswordPath;

            var jsonBody = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            //aqui estou retornado um JSON para o front-end para que o Ajax possa tratar a resposta
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Solicitação enviada com sucesso!",
                    redirectUrl = Url.Action("Index", "Home", null, Request.Scheme) // Redirecionar para a Home/Views/Home/Index.cshtml
                });
            }
            else
            {
                return Json(new { success = false, error = "Email não encontrado." });
            }
        }

        [HttpGet("SignIn")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpGet("Error")]
        public IActionResult Error(ErrorViewModel model)
        {
            _logger.LogInformation("Método Error chamado. Renderizando a view Error.cshtml.");
            return View(model);
        }

        private async Task<object?> GetUserByEmail(string email)
        {
            var forgotPasswordPath = $"/getUserByEmail?email={email}";
            string apiUrl = _configuration["ApiBackendSettings:ForgotPasswordUrl"] + forgotPasswordPath;

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response
                    .Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}