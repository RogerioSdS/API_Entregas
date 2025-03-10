using ApiBackend_Entregas.Application.Repositories.Interfaces;
using ApiBackend_Entregas.Application.Service.Interfaces;
using ApiBackend_Entregas.Models;

namespace ApiBackend_Entregas.Application.Service
{
    public class UserService : IUserServices
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<Users>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<Users> GetUserAsync(Users user)
        {
            var foundUser = await _userRepository.GetUserAsync(user);
            if (foundUser == null)
            {
                throw new Exception($"Usuário com e-mail {user.Email} e senha fornecida não encontrado.");
            }
            return foundUser;
        }

        public async Task<Users> AddUserAsync(Users user)
        {
            return await _userRepository.AddUserAsync(user);
        }
    }
}