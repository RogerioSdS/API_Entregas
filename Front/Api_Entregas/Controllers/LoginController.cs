using Microsoft.AspNetCore.Mvc;
using Api_Entregas.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly HttpClient _httpClient;

        public LoginController(ILogger<LoginController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
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
                    // Configurar a URL da API de usuários com parâmetros de consulta
                    string apiUrl = "http://localhost:5088/api/user/getUser";
                    string parameters = $"?Email={Uri.EscapeDataString(model.Email)}&Password={Uri.EscapeDataString(model.Password)}";
                    var requestUri = $"{apiUrl}{parameters}";

                    _logger.LogInformation($"Fazendo requisição para {requestUri}");

                    // Fazer a requisição GET para a API de usuários
                    var response = await _httpClient.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        // Desserializar a resposta da API como Users
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var userData = JsonConvert.DeserializeObject<SignInViewModel>(responseContent);
                        userData.Name = $"Usuário {DateTime.Now}"; // Ou use um campo de nome de Users, se existir

                        // Armazenar dados do usuário (ex.: em sessão) para autenticação
                        HttpContext.Session.SetString("UserEmail", model.Email);
                        _logger.LogInformation("Redirecionando para SignIn após login bem-sucedido.");
                        return RedirectToAction("SignIn");
                    }
                    else
                    {
                        // Logar o código de status e a resposta para depuração
                        var errorContent = await response.Content.ReadAsStringAsync();
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
            _logger.LogInformation("Método SignIn chamado com sucesso.");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            _logger.LogInformation("UserEmail: {UserEmail}", userEmail);

            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("Nenhum usuário autenticado na sessão. Redirecionando para Login.");
                return RedirectToAction("Login");
            }

            var time = DateTime.Now;
            var model = new SignInViewModel
            {
                Email = userEmail,
                Name = $"Usuário {time}"
            };

            _logger.LogInformation($"Renderizando a view SignIn.cshtml com o modelo: Email={model.Email}, Name={model.Name}");
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