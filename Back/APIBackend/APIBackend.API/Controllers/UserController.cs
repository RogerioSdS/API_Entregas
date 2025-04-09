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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 com detalhes da validação
            }

            var result = await _userService.AddUserAsync(model);
            
            if (result == null)
            {
                return BadRequest($"Erro ao criar usuário.{model.FirstName} {model.LastName}");
            }
            // analisar se é necessário retornar o usuário.id somente quando for o admin que estiver criando o usuário
            return Created("", result);
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

        // Preciso realizar a validação que somente admin pode fazer essa consulta, utilizando o JWT
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsersAsync();

            if (users == null || users.Count == 0)
            {
                return NoContent(); // Retorna 204 se não houver usuários
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