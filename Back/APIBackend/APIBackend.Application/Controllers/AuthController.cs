using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain;
using APIBackend.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIBackend.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;
       
        [HttpPost("login")]
        public async Task<IActionResult> Login(Auth model)
        {
            try
            {
                var foundUser = await _authService.AuthLogin(model);
                if (foundUser == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                return Ok(foundUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
