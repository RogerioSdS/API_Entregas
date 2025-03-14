using APIBackend.Domain.Identity;

namespace APIBackend.Application.Services.Interfaces;

public interface IUserService
{    
    Task<List<User>> GetUsersAsync();
    Task<User> GetUserAsync(int id); 
    Task<User> AddUserAsync(User user, string role, string password, bool signInAfterCreation);
    Task<List<string>> GetRolesAsync(User user);
}
