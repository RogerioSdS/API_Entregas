using Api_Entregas.Services.Interfaces;
using Api_Entregas.ViewModels;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;


namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISessionService _sessionService;
        private readonly ILogger<LoginController> _logger;

        public StudentController(IHttpClientFactory httpClientFactory, ISessionService sessionService, ILogger<LoginController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet("createStudent")]
        public async Task<IActionResult> CreateStudent()
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Login");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7188/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // Busca todos os usuários para popular o dropdown
            var users = await client
                .GetFromJsonAsync<List<UserViewModel>>("api/admin/getAllUsers")
                .ConfigureAwait(false)
                ?? new List<UserViewModel>();

            // Usa ViewBag para enviar a lista ao Razor
            ViewBag.Users = new SelectList(users, "Id", "FirstName");

            // Inicializa o VM vazio
            var model = new StudentViewModel
            {
                Responsibles = new List<UserViewModel>()
            };

            return View("StudentCreate", model);
        }

        [HttpPost("createStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(StudentViewModel model)
        {
            // Revalida dropdown em caso de erro
            void RepopularDropdown()
            {
                var t = Task.Run(async () =>
                {
                    var token = Request.Cookies["access_token"];
                    var c = _httpClientFactory.CreateClient();
                    c.BaseAddress = new Uri("https://localhost:7188/");
                    c.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                    var users = await c.GetFromJsonAsync<List<UserViewModel>>("api/admin/getAllUsers")
                               ?? new List<UserViewModel>();
                    ViewBag.Users = new SelectList(users, "Id", "FirstName");
                });
                t.Wait();
            }

            if (!ModelState.IsValid)
            {
                RepopularDropdown();
                return View("StudentCreate", model);
            }

            try
            {
                var token = Request.Cookies["access_token"];
                var apiClient = _httpClientFactory.CreateClient();
                apiClient.BaseAddress = new Uri("https://localhost:7188/");
                apiClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // *** AQUI: use a URL que realmente existe na API externa ***
                var endpoint = "api/student/createStudent"; // ou "api/student"

                _logger.LogInformation("Enviando POST para {Endpoint}: {@Model}", endpoint, model);

                var response = await apiClient
                    .PostAsJsonAsync(endpoint, model)
                    .ConfigureAwait(false);

                var body = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Resposta da API externa: {StatusCode} / {Body}",
                                       response.StatusCode, body);

                response.EnsureSuccessStatusCode();

                TempData["SuccessMessage"] = "Aluno criado com sucesso!";
                return RedirectToAction("getStudentsNames");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao chamar API externa");
                ModelState.AddModelError("", "Erro na API externa: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado");
                ModelState.AddModelError("", "Erro inesperado: " + ex.Message);
            }

            // se caiu aqui, houve erro – repopule e volte à view
            RepopularDropdown();
            return View("StudentCreate", model);
        }


        [HttpGet("getStudentsNames")]
        public async Task<IActionResult> GetStudentsNames()
        {
            // 0) Lê o JWT do cookie
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Login");

            // 1) Prepara o HttpClient
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7188/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // 2) Desserializa direto numa List<StudentDto>
            var studentDtos = await client
                .GetFromJsonAsync<List<StudentDto>>("api/student/getStudentsNames")
                .ConfigureAwait(false)
                // Se vier nulo, substitui por lista vazia
                ?? new List<StudentDto>();

            // 3) Cria o “wrapper” que sua ViewModel espera
            var model = new StudentNameIdDTO
            {
                Students = studentDtos
            };

            // 4) (Opcional) salva em sessão
            _sessionService.SetUserData("StudentData", model);

            // 5) Retorna a View com o ViewModel correto
            return View("Views/Student/StudentsNames.cshtml", model);
        }

        [HttpGet("getStudentDetails")]
        public async Task<IActionResult> GetStudentDetailsAsync(int studentId)
        {
            // 0) Tenta ler o JWT armazenado em cookie HttpOnly
            var accessToken = Request.Cookies["access_token"];
            // Se não existir token, redireciona para a página de login
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Login");

            // 1) Cria um HttpClient a partir da fábrica configurada
            var client = _httpClientFactory.CreateClient();
            //    Define a base da URL da API externa (apenas domínio e porta)
            client.BaseAddress = new Uri("https://localhost:7188/");
            //    Adiciona o header Authorization: Bearer <token>
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // 2) Chama o endpoint da API externa e desserializa o JSON em uma lista de DTOs
            var student = await client
                .GetFromJsonAsync<StudentViewModel>($"api/student/getStudentById?id={studentId}")
                .ConfigureAwait(false)
                // Se o retorno for nulo, inicializa uma lista vazia para evitar NullReference
                ?? new StudentViewModel();
            //    (Opcional) Salva o ViewModel na sessão para uso posterior
            _sessionService.SetUserData("StudentData", student);

            // 5) Retorna a View Razor já populada com o ViewModel
            return View("Views/Student/StudentDetails.cshtml", student);
        }

        [HttpPost("BeginEdit")]
        public IActionResult BeginEdit(StudentViewModel model)
        {
            void RepopularDropdown()
            {
                var t = Task.Run(async () =>
                {
                    var token = Request.Cookies["access_token"];
                    var c = _httpClientFactory.CreateClient();
                    c.BaseAddress = new Uri("https://localhost:7188/");
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var users = await c.GetFromJsonAsync<List<UserViewModel>>("api/admin/getAllUsers")
                                ?? new List<UserViewModel>();

                    // aqui: passa a lista de IDs marcados
                    ViewBag.Users = new MultiSelectList(
                        users,               // data source
                        "Id",                // value field
                        "FirstName",         // text field
                        model.ResponsibleId  // IEnumerable<int> com os IDs que já vieram
                    );
                });
                t.Wait();
            }

            if (!ModelState.IsValid)
                return View("StudentDetails", model);

            var accessToken = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Login");

            RepopularDropdown();

            model.ResponsibleId = model.Responsibles
                         .Select(r => r.Id)
                         .ToList();

            return View("Views/Student/StudentEdit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentEdit(StudentViewModel model)
        {
            // 1) Validação do ModelState
            if (!ModelState.IsValid)
            {
                // Log de informação com todos os erros de validação
                var erros = ModelState
                    .Where(kv => kv.Value.Errors.Any())
                    .Select(kv => new
                    {
                        Campo = kv.Key,
                        Mensagens = kv.Value.Errors.Select(e => e.ErrorMessage)
                    });

                foreach (var erro in erros)
                {
                    foreach (var msg in erro.Mensagens)
                    {
                        _logger.LogInformation(
                            "Falha de validação em '{Campo}': {Mensagem}",
                            erro.Campo,
                            msg
                        );
                    }
                }
            }

            // 2) Recupera token e monta HttpClient
                var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Login");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7188/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // 3) Ajusta a rota para bater com o atributo [HttpPut("{id}")] da API
            var relativeUrl = $"api/student/UpdateStudent";
            var fullUrl = new Uri(client.BaseAddress, relativeUrl);
            _logger.LogInformation("Enviando PUT para {Url} com payload {@Model}", fullUrl, model);

            // 4) Executa o PUT
            var response = await client.PutAsJsonAsync(relativeUrl, model);

            _logger.LogInformation("Resposta da API externa: {StatusCode}", response.StatusCode);

            // 5) Trata falha
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Falha ao atualizar aluno. Status: {StatusCode}, Body: {Body}",
                                 response.StatusCode,
                                 await response.Content.ReadAsStringAsync());
                ModelState.AddModelError("", "Falha ao atualizar.");
                await RepopularDropdown(model);
                return View("StudentEdit", model);
            }

            // 6) Sucesso: redireciona para detalhes
            return RedirectToAction("GetStudentDetails", new { studentId = model.Id });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string erro)
        {
            return View("Error", new ErrorViewModel { RequestId = erro });
        }

        private async Task RepopularDropdown(StudentViewModel model)
        {
            // Lê o token do cookie
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
                throw new InvalidOperationException("Token de acesso não encontrado.");

            // Prepara o HttpClient
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7188/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // Chama a API para obter todos os usuários
            var users = await client
                .GetFromJsonAsync<List<UserViewModel>>("api/admin/getAllUsers")
                .ConfigureAwait(false)
                ?? new List<UserViewModel>();

            // Popula o ViewBag com o MultiSelectList, já selecionando os IDs existentes
            ViewBag.Users = new MultiSelectList(
                users,                  // fonte de dados
                "Id",                   // valor de cada <option>
                "FirstName",            // texto exibido em cada <option>
                model.ResponsibleId     // IEnumerable<int> com os IDs que devem vir marcados
            );
        }
    }
}