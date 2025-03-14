using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace APIBackend.API.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar usuários.
    /// </summary>
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model, string role, string password, bool signInAfterCreation = true)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email, 
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                ZipCode = model.ZipCode,
                City = model.City,
                CreditLimit = model.CreditLimit
            };

            var result = await _userService.AddUserAsync(user, role, password, signInAfterCreation );
            if (result == null)
            {
                return BadRequest($"Erro ao criar usuário.{model.UserName}");
            }
            // analisar se é necessário retornar o usuário.id somente quando for o admin que estiver criando o usuário
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { User = user, Roles = await _userService.GetRolesAsync(user) });
        }

        // Preciso realizar a validação que somente admin pode fazer essa consulta
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        // UPDATE: Atualizar um usuário
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Email = dto.Email ?? user.Email;
            user.Address = dto.Address ?? user.Address;
            user.ZipCode = dto.ZipCode != 0 ? dto.ZipCode : user.ZipCode;
            user.City = dto.City ?? user.City;
            user.CreditLimit = dto.CreditLimit != 0 ? dto.CreditLimit : user.CreditLimit;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(user);
        }

        // DELETE: Deletar um usuário
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }

    // DTOs
    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public string City { get; set; }
        public decimal CreditLimit { get; set; }
        public string RoleName { get; set; } // Opcional
    }

    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public string City { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
