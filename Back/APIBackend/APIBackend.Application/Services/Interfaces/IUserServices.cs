using APIBackend.Domain;
using APIBackend.Domain.Models;

namespace APIBackend.Application.Services.Interfaces;

public interface IUserServices
{    
        public Task<List<User>> GetUsersAsync();
        public Task<User> GetUserAsync(int id); 
        public Task<User> AddUserAsync(User user);
}
