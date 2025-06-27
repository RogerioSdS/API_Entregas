using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IUserRepo
{
    /// <summary>
    /// Retorna a lista de todos os usuários cadastrados.
    /// </summary>
    /// <returns>Lista de usuários.</returns>
    Task<List<User>> GetUsersAsync();

    /// <summary>
    /// Busca um usuário pelo seu e-mail.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <returns>O usuário encontrado.</returns>
    Task<User>? GetUserByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário pelo seu ID.
    /// </summary>
    /// <param name="id">ID do usuário.</param>
    /// <returns>O usuário encontrado.</returns>
    Task<User>? GetUserByIdAsync(int id);

    /// <summary>
    /// Busca um usuário pelo seu nome.
    /// </summary>
    /// <param name="name">Nome do usuário.</param>
    /// <returns>O usuário encontrado ou null.</returns>
    Task<User?> GetUserByNameAsync(string name);

    /// <summary>
    /// Adiciona um novo usuário ao sistema com uma role específica.
    /// </summary>
    /// <param name="user">Usuário a ser adicionado.</param>
    /// <returns>O usuário criado.</returns>
    Task<User> AddUserAsync(User user);

    /// <summary>
    /// Atualiza os dados de um usuário no sistema.
    /// </summary>
    /// <param name="user">Usuário com os dados atualizados.</param>
    /// <returns>Usuário atualizado.</returns>
    Task<User> UpdateUserAsync(User user);

    /// <summary>
    /// Remove um usuário do sistema.
    /// </summary>
    /// <param name="user">Usuário a ser removido.</param>
    /// <returns>True se a remoção for bem-sucedida.</returns>
    Task<bool> DeleteUserAsync(User user);

    /// <summary>
    /// Obtém a lista de roles atribuídas a um usuário.
    /// </summary>
    /// <param name="user">Usuário para consulta de roles.</param>
    /// <returns>Lista de roles do usuário.</returns>
    Task<List<string>> GetRolesAsync(User user);

    /// <summary>
    /// Cria uma role no sistema se ela ainda não existir.
    /// </summary>
    /// <param name="role">Nome da role a ser criada.</param>
    Task InsertRoleToUserAsync(string role);
}
