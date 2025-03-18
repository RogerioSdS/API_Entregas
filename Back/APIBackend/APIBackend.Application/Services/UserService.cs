using APIBackend.Application.DTOs;
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

    /// <summary>
    /// Adiciona um novo usuário ao sistema.
    /// </summary>
    /// <param name="userDTO">Os dados do usuário a serem cadastrados.</param>
    /// <param name="role">A função (role) que o usuário terá no sistema.</param>
    /// <param name="password">A senha do usuário.</param>
    /// <param name="signInAfterCreation">Indica se o usuário deve ser autenticado automaticamente após a criação.</param>
    /// <returns>Retorna um objeto <see cref="UserDTO"/> com os dados do usuário criado.</returns>
    /// <exception cref="Exception">Lançada quando ocorre um erro ao adicionar o usuário.</exception>
    public async Task<UserDTO> AddUserAsync(UserDTO userDTO, string role, string password, bool signInAfterCreation = false)
    {
        var user = _mapper.Map<User>(userDTO);
        try
        {
            var createdUser = await _userRepository.AddUserAsync(user, role, password);
            if (signInAfterCreation)
                await AuthAsync(createdUser);
            return _mapper.Map<UserDTO>(createdUser);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao adicionar usuário", ex);
        }
    }


    /// <summary>
    /// Retorna uma lista com todos os usuários do sistema.
    /// </summary>
    /// <returns>Retorna uma lista de <see cref="UserDTO"/>.</returns>
    public async Task<List<UserDTO>> GetUsersAsync()
    {
        var users = await _userRepository.GetUsersAsync();

        return _mapper.Map<List<UserDTO>>(users);
    }


    /// <summary>
    /// Retorna o usuário com o id especificado.
    /// </summary>
    /// <param name="id">O id do usuário a ser retornado.</param>
    /// <returns>Retorna o objeto <see cref="UserDTO"/> que representa o usuário se encontrado, caso contrário retorna null.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Lançada quando o id do usuário é menor ou igual a zero.</exception>
    public async Task<UserDTO> GetUserByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id do usuário inválido");

        var user = await _userRepository.GetUserByIdAsync(id);

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO> GetUserByNameAsync(string name)
    {
        return await _userRepository.GetUserByNameAsync(name);
    }
    public async Task AuthAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo");

        await _signInManager.SignInAsync(user, isPersistent: false);
    }

    public Task<UserDTO> UpdateUserAsync(UserDTO user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo");

        return _userRepository.UpdateUserAsync(user);
    }

    public Task<bool> DeleteUserAsync(UserDTO user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo");

        return _userRepository.DeleteUserAsync(user);
    }

    public async Task<List<string>> GetRolesAsync(User user)
    {
        return await _userRepository.GetRolesAsync(user);
    }

}
