
using APIBackend.Domain;
using APIBackend.Repositories.Interfaces;

namespace APIBackend.Application.Services.Interfaces;

public class UserService : IUserServices
{
    private readonly IUserRepo _userRepository;

    public UserService(IUserRepo userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }

    public async Task<User> GetUserAsync(int id)
    {
        var foundUser = await _userRepository.GetUserAsync(id);
        
        return foundUser;
    }

    public async Task<User> AddUserAsync(User user)
    {
        return await _userRepository.AddUserAsync(user);
    }

    public Task<User> GetUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}
