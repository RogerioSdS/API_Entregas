using System.Threading.Tasks;
using ApiBackend_Entregas.Application.Repositories;
using ApiBackend_Entregas.Application.Service;
using ApiBackend_Entregas.Application.Service.Interfaces;
using ApiBackend_Entregas.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiBackend_Entregas.Controllers;

/// <summary>
/// Controlador responsável por gerenciar usuários.
/// </summary>
[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly IUserServices _userService;
    public UsersController(IUserServices userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtém a lista de usuários cadastrados.
    /// </summary>
    /// <returns>Lista de usuários no formato JSON.</returns>
    /// <response code="200">Retorna a lista de usuários com sucesso.</response>
    [HttpGet("getUsers", Name = "GetUsers")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();

        return Ok(users);
    }

    /// <summary>
    /// Obtém um usuário específico com base nas credenciais fornecidas.
    /// </summary>
    /// <param name="user">O usuário com e-mail e senha a ser buscado.</param>
    /// <returns>O usuário correspondente no formato JSON.</returns>
    /// <response code="200">Retorna o usuário com sucesso.</response>
    /// <response code="400">Se o usuário ou suas credenciais forem nulas.</response>
    /// <response code="404">Se o usuário não for encontrado.</response>
    [HttpGet("getUser", Name = "GetUser")]
    public async Task<IActionResult> GetUser([FromQuery] Users user)
    {
        var foundUser = await _userService.GetUserAsync(user); // Usar await para obter o Users
        return Ok(foundUser);
    }

    /// <summary>
    /// Adiciona um novo usuário.
    /// </summary>
    /// <param name="user">O usuário a ser adicionado.</param>
    /// <returns>O usuário adicionado no formato JSON.</returns>
    /// <response code="200">Retorna o usuário adicionado com sucesso.</response>
    /// <response code="400">Se o usuário for nulo.</response>
    [HttpPost(Name = "PostUsers")]
    public IActionResult PostUsers([FromBody] Users user)
    {
        if (user == null)
        {
            return BadRequest("O usuário não pode ser nulo.");
        }

        _userService.AddUserAsync(user);
        return Ok(user);
    }
}