using Api_Entregas.Services.Interfaces;
using Api_Entregas.ViewModels;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;


namespace Api_Entregas.Controllers
{
    [Route("[controller]")]
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISessionService _sessionService;

        public StudentController(IHttpClientFactory httpClientFactory, ISessionService sessionService)
        {
            _httpClientFactory = httpClientFactory;
            _sessionService = sessionService;
        }

        [HttpGet("beginCreateStudent")]
        public async Task<IActionResult> BeginCreateStudentAsync()
        {
            return View("Views/Student/StudentCreate.cshtml");
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
            return View("Views/Student/StudentEdit.cshtml", model);
        }

        [HttpPost("editStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(StudentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Views/Student/EditStudent.cshtml", model);

            var accessToken = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Login");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7188/");
            //    Adiciona o header Authorization: Bearer <token>
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client
                .PutAsJsonAsync("api/student/UpdateStudent", model)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Falha ao atualizar.");
                return View("Views/Student/StudentEdit.cshtml", model);
            }

            return RedirectToAction(actionName: "getStudentDetails", controllerName: "Student", routeValues: new { studentId = model.Id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string erro)
        {
            return View("Error", new ErrorViewModel { RequestId = erro });
        }
    }
}