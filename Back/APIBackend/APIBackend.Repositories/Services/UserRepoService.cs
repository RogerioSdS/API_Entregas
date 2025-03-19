using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace APIBackend.Repositories.Services;

public class UserRepoService : IUserRepo
{
    private readonly UserManager<User> _userManager;
    private readonly ApiDbContext _userContext;
    private readonly List<string> _validRoles;

    public UserRepoService(ApiDbContext context, UserManager<User> userManager, IConfiguration configuration)
    {
        _userContext = context;
        _userManager = userManager;
        _validRoles = configuration.GetSection("UserRoles:ValidRoles").Get<List<string>>() ?? new List<string>();
    }

    /// <summary>
    /// Adiciona um novo usuário ao sistema com uma role específica.
    /// </summary>
    /// <param name="user">Usuário a ser adicionado.</param>
    /// <param name="role">Papel do usuário.</param>
    /// <param name="password">Senha do usuário.</param>
    /// <returns>O usuário criado.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o usuário é nulo.</exception>
    /// <exception cref="ArgumentException">Lançada quando o papel é inválido.</exception>
    /// <exception cref="Exception">Lançada quando ocorre um erro ao criar ou atribuir o papel ao usuário.</exception>
    public async Task<User> AddUserAsync(User user, string role, string password)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo.");

        if (!_validRoles.Contains(role))
            throw new ArgumentException($"O papel '{role}' é inválido.", nameof(role));

        try
        {
            var identityResult = await _userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
                throw new Exception($"Erro ao criar usuário: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new Exception($"Erro ao atribuir role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

            return user;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao adicionar usuário: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retorna a lista de todos os usuários cadastrados.
    /// </summary>
    /// <returns>Lista de usuários.</returns>
    public async Task<List<User>> GetUsersAsync()
    {
        return await _userContext.Users.ToListAsync();
    }

    /// <summary>
    /// Busca um usuário pelo seu ID.
    /// </summary>
    /// <param name="id">ID do usuário.</param>
    /// <returns>O usuário encontrado.</returns>
    /// <exception cref="Exception">Lançada quando o usuário não é encontrado.</exception>
    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString()) ?? throw new Exception("Usuário não encontrado.");
    }

    /// <summary>
    /// Busca um usuário pelo seu nome.
    /// </summary>
    /// <param name="name">Nome do usuário.</param>
    /// <returns>O usuário encontrado.</returns>
    /// <exception cref="Exception">Lançada quando o usuário não é encontrado.</exception>
    public async Task<User> GetUserByNameAsync(string name)
    {
        var user = await _userManager.FindByNameAsync(name);
        return user ?? throw new Exception("Usuário não encontrado.");
    }

    /// <summary>
    /// Atualiza os dados de um usuário.
    /// </summary>
    /// <param name="user">Usuário a ser atualizado.</param>
    /// <returns>O usuário atualizado.</returns>
    /// <exception cref="Exception">Lançada quando o usuário não é encontrado ou quando ocorre um erro ao atualizar.</exception>
    public async Task<User> UpdateUserAsync(User user)
    {
        var userToUpdate = await _userManager.GetUserIdAsync(user);
        if (userToUpdate == null)
            throw new Exception("Usuário não encontrado.");

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception($"Erro ao atualizar usuário: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user;
    }

    /// <summary>
    /// Remove um usuário do sistema.
    /// </summary>
    /// <param name="user">Usuário a ser removido.</param>
    /// <returns>True se a remoção for bem-sucedida.</returns>
    /// <exception cref="Exception">Lançada quando o usuário não é encontrado.</exception>
    public async Task<bool> DeleteUserAsync(User user)
    {
        var userToDelete = await _userManager.GetUserIdAsync(user);
        if (userToDelete == null)
            throw new Exception("Usuário não encontrado.");

        await _userManager.DeleteAsync(user);

        return true;
    }

    /// <summary>
    /// Obtém a lista de roles atribuídas a um usuário.
    /// </summary>
    /// <param name="user">Usuário para consulta de roles.</param>
    /// <returns>Lista de roles do usuário.</returns>
    public async Task<List<string>> GetRolesAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}
