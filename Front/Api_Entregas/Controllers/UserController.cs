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
        private readonly ISessionService _sessionService;

        public UserController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // GET: /User/Perfil
        [HttpGet("perfil")]
        public IActionResult Perfil()
        {
            var json = HttpContext.Session.GetString("UserData");
            if (json == null) return RedirectToAction("Error");

            var model = JsonConvert.DeserializeObject<UserViewModel>(json);
            return View("/Views/User/Perfil.cshtml", model); // View está em Views/User/Perfil.cshtml
        }

        // POST: /User/PerfilPost
        [HttpPost("perfil")]
        public IActionResult PerfilPost([FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            _sessionService.SetUserData("UserData", model); 
            return Ok(); // Apenas confirma
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}