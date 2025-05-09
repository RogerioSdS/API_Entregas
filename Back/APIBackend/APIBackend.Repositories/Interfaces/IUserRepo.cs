using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IUserRepo
{
    public Task<List<User>> GetUsersAsync();
    public Task<User>? GetUserByEmailAsync(string email);
    public Task<User>? GetUserByIdAsync(int id);
    public Task<User?> GetUserByNameAsync(string name);
    public Task<User> AddUserAsync(User user);
    public Task<User> UpdateUserAsync(User user);
    public Task<bool> DeleteUserAsync(User user);
    public Task<List<string>> GetRolesAsync(User user);
    public Task InsertRoleToUserAsync(string role);
}
