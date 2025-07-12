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
    private readonly RoleManager<Role> _roleManager;
    private readonly ApiDbContext _userContext;

    public UserRepoService(ApiDbContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
    {
        _userContext = context;
        _roleManager = roleManager;
        _userManager = userManager;
        
    }

    public async Task<User> AddUserAsync(User user)
    {
        //analisar se essa regra de negocio não deve ser feita na service da Application
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo.");

        using (var transaction = await _userContext.Database.BeginTransactionAsync())
        {
            await InsertRoleToUserAsync(user.Role);

            try
            {
                // Criar o usuário
                var identityResult = await _userManager.CreateAsync(user, user.Password);
                if (!identityResult.Succeeded)
                    throw new Exception($"Erro ao criar usuário: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                

                // Associar o usuário à role 
                var roleAddResult = await _userManager.AddToRoleAsync(user, user.Role);
                if (!roleAddResult.Succeeded)
                    throw new Exception($"Erro ao atribuir role: {string.Join(", ", roleAddResult.Errors.Select(e => e.Description))}");

                // Se tudo deu certo, confirmar a transação
                await transaction.CommitAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao adicionar usuário: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro ao adicionar usuário: {ex.Message}");
            }
        }
    }

    public async Task InsertRoleToUserAsync(string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new Role(role));
            if (!roleResult.Succeeded)
                throw new Exception($"Erro ao criar role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        }        
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _userContext.Users.ToListAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception("Usuário não encontrado.");
    }
    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString()) ?? throw new Exception("Usuário não encontrado.");
    }

    public async Task<User?> GetUserByNameAsync(string name)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.FirstName == name) ?? null;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception($"Erro ao atualizar usuário: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user;
    }

    public async Task<bool> DeleteUserAsync(User user)
    {
        await _userManager.DeleteAsync(user);
        return true;
    }

    public async Task<List<string>> GetRolesAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}
