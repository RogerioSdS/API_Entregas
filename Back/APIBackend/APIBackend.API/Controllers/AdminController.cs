using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace APIBackend.API.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly Logger _loggerNLog = LogManager.GetCurrentClassLogger();

        public AdminController(IUserService userService, IStudentService studentService)
        {
            _userService = userService;
            _studentService = studentService;
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
            if (!ModelState.IsValid)
            {
                return BadRequest("Todos os campos obrigatórios devem ser preenchidos.");
            }
            try
            {
                var result = await _userService.UpdateUserFromAdminAsync(model);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id do usuário inválido.");
            }

            try
            {
                var result = await _userService.DeleteUserAsync(id);

                return NoContent();
            }
            catch (NullReferenceException)
            {
                return NotFound("Usuário não encontrado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor ao deletar usuário.{ex.Message}");
            }
        }   

        [HttpDelete("DeleteStudent")]
        public async Task<IActionResult> DeleteStudent([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("O ID do estudante deve ser um número positivo.");
            }

            try
            {
                var result = await _studentService.DeleteStudentAsync(id);

                return NoContent();
            }
            catch  (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidOperationException)
            {
                _loggerNLog.Error($"Erro inesperado ao deletar estudante com ID {id}: {ex.Message}");

                return NotFound($"Estudante com ID {id} não encontrado.");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error($"Erro inesperado ao deletar estudante com ID {id}: {ex.Message}");

                return StatusCode(500, "Erro interno do servidor.");
            }
        }
                
    }
}

