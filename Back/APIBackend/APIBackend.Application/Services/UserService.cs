using APIBackend.Domain.Identity;
using APIBackend.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace APIBackend.Application.Services.Interfaces;

public class UserService : IUserService
{
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly IUserRepo _userRepository;

    public UserService(SignInManager<User> signInManager, IMapper mapper, IUserRepo userPersist)
    {
        _userRepository = userPersist;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }

    public async Task<User> GetUserAsync(int id)
    {
        return await _userRepository.GetUserAsync(id); // Exceção já tratada no repositório
    }

    public async Task<User> AddUserAsync(User user, string role, string password, bool signInAfterCreation = false)
    {
        try
        {
            var createdUser = await _userRepository.AddUserAsync(user, role, password);
            if (signInAfterCreation)
                await AuthAsync(createdUser);
            return createdUser;
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao adicionar usuário", ex);
        }
    }

    public async Task AuthAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo");
        
        await _signInManager.SignInAsync(user, isPersistent: false);
    }

    public async Task<List<string>> GetRolesAsync(User user)
    {
        return await _userRepository.GetRolesAsync(user);
    }
}
