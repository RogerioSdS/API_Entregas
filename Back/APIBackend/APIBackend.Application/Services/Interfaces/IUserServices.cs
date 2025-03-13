using APIBackend.Domain.Identity;

namespace APIBackend.Application.Services.Interfaces;

public interface IUserServices
{    
        public Task<List<User>> GetUsersAsync();
        public Task<User> GetUserAsync(int id); 
        public Task<User> AddUserAsync(User user);
}
