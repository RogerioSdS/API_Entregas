using System;
using APIBackend.Domain;

namespace APIBackend.Repositories.Interfaces;

public interface IUserRepo
{
    public Task<List<User>> GetUsersAsync();
    public Task<User>? GetUserAsync(int id);
    public Task<User> AddUserAsync(User user);
    public Task<User> AuthUser(User user);

}
