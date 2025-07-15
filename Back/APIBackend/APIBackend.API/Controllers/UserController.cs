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

            try
            {
                var result = await _userService.AddUserAsync(model);
                _loggerNLog.Info($"Usuario criado com sucesso: {result.FirstName + " " + result.LastName} - {result.Email}");

                if (result.Role.Contains("Admin"))
                {
                    return Created("", result);
                }
                else
                {
                    return Created("", new { result.FirstName, result.LastName, result.Email });
                }

            }
            catch (NullReferenceException ex)
            {
                _loggerNLog.Error(ex, $"Erro ao criar usuário: {model.FirstName} {model.LastName}");

                return BadRequest(ex.Message ?? "Erro interno ao criar usuário.");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error(ex, $"Erro ao criar usuário: {model.FirstName} {model.LastName}");

                return StatusCode(500, "Erro interno ao criar usuário.");
            }

        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID do usuário deve ser um número positivo.");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                return Ok(new { User = user, Roles = await _userService.GetRolesAsync(user) });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _loggerNLog.Error(ex, $"Erro ao buscar usuário com ID inválido: {id}");

                return BadRequest("ID do usuário deve ser um número positivo.");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error(ex, $"Erro ao buscar usuário com ID: {id}");

                return StatusCode(500, "Erro interno ao buscar usuário.");
            }
        }

        [HttpGet("getUserByName")]
        public async Task<IActionResult> GetUserByName([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Nome do usuário deve ser informado.");
            }

            var user = await _userService.GetUserByNameAsync(name);
            if (user == null)
            {
                return NotFound("Usuario não encontrado!");
            }

            return Ok(user);
        }

        [Authorize]
        [HttpGet("getUserByEmail")]
        public async Task<IActionResult> GetUserByEmail()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email do usuário deve ser informado.");
            }

            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                return Ok(user);
            }   
            catch (ArgumentNullException ex)
            {
                return BadRequest($"Erro ao buscar usuário, email inválido: {email}");
            }
            catch (Exception ex)
            {
                _loggerNLog.Error(ex, $"Erro ao buscar usuário com email: {email}");
                return StatusCode(500, "Erro interno ao buscar usuário.");
            }
        }

        [HttpPut("UpdateUserDetails")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UserUpdateFromUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.UpdateUserFromUserAsync(model);

                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest("usuario não encontrado ou campos inválidos: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao atualizar usuário." + ex.Message);
            }

        }
    }
}