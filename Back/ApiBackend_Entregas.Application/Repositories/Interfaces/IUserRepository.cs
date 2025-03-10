using ApiBackend_Entregas.Models;

namespace ApiBackend_Entregas.Application.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<Users>> GetUsersAsync();
        public Task<Users>? GetUserAsync(Users user);
        public Task<Users> AddUserAsync(Users user);
    }
}