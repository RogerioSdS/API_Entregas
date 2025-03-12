using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain;
using Microsoft.AspNetCore.Mvc;

namespace APIBackend.Application.Controllers
{
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
        /// <param name="id">O usuário com e-mail e senha a ser buscado.</param>
        /// <returns>O usuário correspondente no formato JSON.</returns>
        /// <response code="200">Retorna o usuário com sucesso.</response>
        /// <response code="400">Se o usuário ou suas credenciais forem nulas.</response>
        /// <response code="404">Se o usuário não for encontrado.</response>
        [HttpGet("getUser/{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var foundUser = await _userService.GetUserAsync(id);
            if (foundUser == null)
            {
                return NotFound($"Usuário com ID: {id}, não foi encontrado");
            }
            return Ok(foundUser);
        }

        /// <summary>
        /// Adiciona um novo usuário.
        /// </summary>
        /// <param name="user">O usuário a ser adicionado.</param>
        /// <returns>O usuário adicionado no formato JSON.</returns>
        /// <response code="200">Retorna o usuário adicionado com sucesso.</response>
        /// <response code="400">Se o usuário for nulo.</response>
        [HttpPost("createUser", Name = "PostUsers")]
        public IActionResult PostUsers([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("O usuário não pode ser nulo.");
            }

            var searchUsers = _userService.GetUsersAsync();

            if (searchUsers.Result.Any(u => u.Email == user.Email))
            {
                return BadRequest("O e-mail já está em uso.");
            }

            _userService.AddUserAsync(user);
            return Ok(user);
        }
    }
}
