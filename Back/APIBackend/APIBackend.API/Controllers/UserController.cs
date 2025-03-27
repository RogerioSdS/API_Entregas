using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIBackend.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
        {
            var result = await _userService.AddUserAsync(model);
            if (result == null)
            {
                return BadRequest($"Erro ao criar usuário.{model.FirstName} {model.LastName}");
            }
            // analisar se é necessário retornar o usuário.id somente quando for o admin que estiver criando o usuário
            return  Created("", result);
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
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsersAsync();

            if (users == null || users.Count == 0)
            {
                return NoContent(); // Retorna 204 se não houver usuários
            }

            return Ok(users);
        }


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