using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIBackend.API.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

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

