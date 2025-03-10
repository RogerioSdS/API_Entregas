using ApiBackend_Entregas.Models;

namespace ApiBackend_Entregas.Application.Service.Interfaces
{
    public interface IUserServices
    {
        public Task<List<Users>> GetUsersAsync();
        public Task<Users> GetUserAsync(Users user); // Alinhado com o repositório
        public Task<Users> AddUserAsync(Users user);        
    }
}