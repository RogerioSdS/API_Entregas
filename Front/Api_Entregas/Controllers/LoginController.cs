using Microsoft.AspNetCore.Mvc;
using Api_Entregas.ViewModels;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
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
        public IActionResult Login()
        {
            _logger.LogInformation("Método Login (GET) chamado. Renderizando a view Login.cshtml.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string apiUrl = _configuration["ApiBackendSettings:AuthUrl"] ?? string.Empty;
                    string jsonBody = JsonConvert.SerializeObject(model); 
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    _logger.LogInformation($"Fazendo requisição para {apiUrl + jsonBody}");

                    // Fazer a requisição Post para a API de usuários
                    var response = await _httpClient.PostAsync(apiUrl,content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var userData = JsonConvert.DeserializeObject<SignInViewModel>(responseContent);
                        userData.Email = model.Email;
                        _logger.LogInformation($"Resposta da API: {responseContent}");
                         // Armazenar como JSON na sessão
                        HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(userData));
                        _logger.LogInformation("Redirecionando para SignIn após login bem-sucedido.");
                        return RedirectToAction("SignIn");
                    }
                    else
                    {
                        // Logar o código de status e a resposta para depuração
                        var errorContent = await response.Content.ReadAsStringAsync();
                        //CRIAR PAGINA PARA INFORMAR QUE HOUVE ERRO NO LOGIN
                        _logger.LogError($"Erro na requisição para {apiUrl}. Status: {response.StatusCode}, Resposta: {errorContent}");
                        ModelState.AddModelError("", "E-mail ou senha inválidos.");
                        return View("Login", model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao autenticar o usuário.");
                    ModelState.AddModelError("", "Ocorreu um erro ao processar o login. Tente novamente.");
                    return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMsg = ex.Message });
                }
            }

            // Logar erros de validação para depuração
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            _logger.LogWarning($"Erros de validação no ModelState: {string.Join(", ", errors)}");
            return View("Login", model);
        }

        [HttpGet("SignIn")]
        public IActionResult SignIn()
        {
            // _logger.LogInformation("Método SignIn chamado com sucesso.");
            var userDataJson = HttpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userDataJson))
            {
                _logger.LogWarning("Usuário não encontrado na sessão. Redirecionando para Login.");
                return RedirectToAction("Login");
            }

            // _logger.LogInformation("Dados do Usuário na sessão: {} ", userDataJson);
            var userData = JsonConvert.DeserializeObject<SignInViewModel>(userDataJson);
            var model = new SignInViewModel
            {
                Email = userData.Email,
                Token = $"JWT: {userData.Token}",
                RefreshToken = $"RefreshToken: {userData.RefreshToken}",
                StartSession = userData.StartSession          
            };

            _logger.LogInformation($"Renderizando a view SignIn.cshtml com o modelo: Email={model.Email}, Name=");
            return View("SignIn", model);
        }

        [HttpGet("Error")]
        public IActionResult Error(ErrorViewModel model)
        {
            _logger.LogInformation("Método Error chamado. Renderizando a view Error.cshtml.");
            return View(model);
        }
    }
}