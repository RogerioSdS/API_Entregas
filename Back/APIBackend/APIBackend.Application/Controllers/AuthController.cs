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
       {
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // CREATE: Criar um usuário
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Address = dto.Address,
            ZipCode = dto.ZipCode,
            City = dto.City,
            CreditLimit = dto.CreditLimit
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Opcional: Associar um papel ao criar
        if (!string.IsNullOrEmpty(dto.RoleName))
        {
            if (await _roleManager.RoleExistsAsync(dto.RoleName))
            {
                await _userManager.AddToRoleAsync(user, dto.RoleName);
            }
            else
            {
                return BadRequest($"Role '{dto.RoleName}' does not exist.");
            }
        }

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // READ: Obter um usuário por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { User = user, Roles = roles });
    }

    // READ: Listar todos os usuários
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
}}}
