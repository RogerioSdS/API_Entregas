using System.Security.Claims;
using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace APIBackend.API.Controllers
{
    //[Authorize] //Com o uso ASP.NET Identity com JWT (gerado no AuthService), o middleware de autenticação JWT (configurado em Program.cs) valida o token automaticamente.
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <param name="model">O objeto de transferência contendo os detalhes do usuário a ser criado.</param>
        /// <returns>
        /// Um <see cref="IActionResult"/> indicando o resultado da operação:
        /// <list type="bullet">
        /// <item><description>Retorna <see cref="BadRequestObjectResult"/> (400) se o estado do modelo for inválido ou se houver um erro ao criar o usuário.</description></item>
        /// <item><description>Retorna <see cref="CreatedResult"/> (201) com os detalhes do usuário criado se a operação for bem-sucedida.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Certifique-se de que o <paramref name="model"/> contém dados válidos antes de chamar este método.
        /// A resposta pode incluir lógica adicional para restringir certos detalhes com base na função do usuário (por exemplo, administrador).
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("createUser")]
        /*Antes de permitir utilizar o método precisa estabelecer o tipo de meio de criação, por exemplo será pelo APP, pela pagina web ou pelo admin, tipo se for o proprio user ou admin, para gerir a resposta*/
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
        {
            // Verifica se o modelo recebido (ex.: DTO com dados enviados pelo cliente) atende às regras de validação definidas
            // (ex.: [Required], [StringLength], [EmailAddress] em propriedades do DTO).
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.AddUserAsync(model);

            if (result == null)
            {
                return BadRequest($"Erro ao criar usuário.{model.FirstName} {model.LastName}");
            }

            _loggerNLog.Info($"Usuario criado com sucesso: {result.FirstName + " " + result.LastName} - {result.Email}");

            if(result.Role.Contains("Admin"))
            {
                return Created("", result);
            }
            else
            {
                return Created("", new { result.FirstName, result.LastName, result.Email });
            }
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { User = user, Roles = await _userService.GetRolesAsync(user) });
        }

        [HttpGet("getUserByName")]
        public async Task<IActionResult> GetUserByName([FromQuery] string name)
        {
            var user = await _userService.GetUserByNameAsync(name);
            if (user == null)
            {
                return NotFound("Usuario não encontrado!");
            }

            return Ok(user);
        }

        [Authorize (Roles = "Admin")]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            /*
            // DEBUG: visualizar claims do usuário
            var userRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            Console.WriteLine("Roles do usuário: " + string.Join(", ", userRoles));
            */

            var users = await _userService.GetUsersAsync();

            if (users == null || users.Count == 0)
            {
                return NoContent();
            }

            return Ok(users);
        }
        [HttpPut("UpdateUserDetails")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UserUpdateFromUserDTO model)
        {
            if (model == null)
            {
                return BadRequest("Usuário não pode ser nulo.");
            }

            if (model.Email == null || model.Email == string.Empty)
            {
                return BadRequest("Email do usuário inválido.");
            }

            var result = await _userService.UpdateUserFromUserAsync(model);
            if (result == null)
            {
                return BadRequest("Erro ao atualizar usuário.");
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateFromAdminDTO model)
        {
            if (model == null)
            {
                return BadRequest("Usuário não pode ser nulo.");
            }

            if (model.Email == null || model.Email == string.Empty)
            {
                return BadRequest("Email do usuário inválido.");
            }

            var result = await _userService.UpdateUserFromAdminAsync(model);
            if (result == null)
            {
                return BadRequest("Erro ao atualizar usuário.");
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id do usuário inválido.");
            }

            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound("Usuário não encontrado ou erro ao deletar.");
            }

            return NoContent();
        }
    }
}