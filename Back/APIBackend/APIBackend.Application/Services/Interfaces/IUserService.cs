using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Adiciona um novo usuário ao sistema.
    /// </summary>
    /// <param name="userDTO">Os dados do usuário a serem cadastrados.</param>
    /// <returns>Retorna um objeto <see cref="UserDTO"/> com os dados do usuário criado.</returns>
    /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar o usuário.</exception>
    Task<UserDTO> AddUserAsync(UserDTO userDTO);

    /// <summary>
    /// Retorna uma lista com todos os usuários do sistema.
    /// </summary>
    /// <returns>Retorna uma lista de <see cref="UserDTO"/> com todos os usuários.</returns>
    Task<List<UserDTO>> GetUsersAsync();

    /// <summary>
    /// Retorna o usuário com o id especificado.
    /// </summary>
    /// <param name="id">O id do usuário a ser retornado.</param>
    /// <returns>Retorna o objeto <see cref="UserDTO"/> que representa o usuário se encontrado, caso contrário retorna null.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Lançada quando o id do usuário é menor ou igual a zero.</exception>
    Task<UserDTO> GetUserByIdAsync(int id);

    /// <summary>
    /// Retorna o usuário com o nome especificado.
    /// </summary>
    /// <param name="name">O nome do usuário a ser retornado.</param>
    /// <returns>Retorna o objeto <see cref="UserDTO"/> que representa o usuário se encontrado, caso contrário retorna null.</returns>
    Task<List<object>> GetUserByNameAsync(string name);

    /// <summary>
    /// Retorna o usuário com o e-mail especificado.
    /// </summary>
    /// <param name="name">O e-mail do usuário.</param>
    /// <returns>Retorna o objeto <see cref="UserDTO"/> se encontrado, ou null.</returns>
    Task<UserDTO> GetUserByEmailAsync(string name);

    Task<LoginDTO> GetUserByEmailToLoginAsync(string email);

    /// <summary>
    /// Atualiza os dados de um usuário no sistema.
    /// </summary>
    /// <param name="userDTO">Os novos dados do usuário.</param>
    /// <returns>Retorna o objeto <see cref="UserUpdateFromUserDTO"/> com os dados do usuário atualizado.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o objeto <see cref="UserDTO"/> é nulo.</exception>
    Task<UserUpdateFromUserDTO> UpdateUserFromUserAsync(UserUpdateFromUserDTO userDTO);

    Task<UserUpdateFromAdminDTO> UpdateUserFromAdminAsync(UserUpdateFromAdminDTO userDTO);

    /// <summary>
    /// Exclui um usuário do sistema.
    /// </summary>
    /// <param name="id">Id do usuário a ser excluído.</param>
    /// <returns>Retorna <c>true</c> se o usuário foi excluído com sucesso.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o usuário não é encontrado.</exception>
    Task<bool> DeleteUserAsync(int id);

    /// <summary>
    /// Retorna a lista de funções (roles) associadas a um usuário.
    /// </summary>
    /// <param name="userDTO">Os dados do usuário para o qual as funções serão retornadas.</param>
    /// <returns>Lista de strings com os nomes das funções.</returns>
    Task<List<string>> GetRolesAsync(UserDTO userDTO);

    /// <summary>
    /// Realiza o login de um usuário no sistema.
    /// </summary>
    /// <param name="user">O usuário que será autenticado.</param>
    /// <returns>Tarefa assíncrona que representa o processo de autenticação.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o usuário é nulo.</exception>
    Task AuthAsync(User user);

    Task VerifyRoleIsPermitedAsync(string role);
}
