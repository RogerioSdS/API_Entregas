using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIBackend.API.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar usuários.
    /// </summary>
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model, string role, string password, bool signInAfterCreation = true)
        {

            var result = await _userService.AddUserAsync(model, role, password, signInAfterCreation);
            if (result == null)
            {
                return BadRequest($"Erro ao criar usuário.{model.FirstName} {model.LastName}");
            }
            // analisar se é necessário retornar o usuário.id somente quando for o admin que estiver criando o usuário
            return CreatedAtAction(nameof(model), model);
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { User = user, Roles = await _userService.GetRolesAsync(user) });
        }

        [HttpGet("getUserByName/{name}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var user = await _userService.GetUserByNameAsync(name);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { User = user, Roles = await _userService.GetRolesAsync(user) });
        }

        // Preciso realizar a validação que somente admin pode fazer essa consulta, utilizando o JWT
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetUsersAsync();
            return Ok(users);
        }

        // UPDATE: Atualizar um usuário
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO user)
        {
            var result = await _userService.UpdateUserAsync(user);
            if (result == null)
            {
                return BadRequest(result);
            }

            return Ok(user);
        }

        // DELETE: Deletar um usuário
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var result = await _userService.DeleteUserAsync(user);
            if (!result)
            {
                return BadRequest("Erro ao deletar usuário.");
            }

            return NoContent();
        }
    }
}