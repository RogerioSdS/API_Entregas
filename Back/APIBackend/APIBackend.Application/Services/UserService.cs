using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace APIBackend.Application.Services;

public class UserService : IUserService
{
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly IUserRepo _userRepository;
    private readonly List<string> _validRoles;

    public UserService(SignInManager<User> signInManager, IMapper mapper, IUserRepo userPersist, IConfiguration configuration)
    {
        _userRepository = userPersist;
        _signInManager = signInManager;
        _mapper = mapper;
        _validRoles = configuration.GetSection("UserRoles:ValidRoles").Get<List<string>>() ?? new List<string>();
    }

    public async Task<UserDTO> AddUserAsync(UserDTO model)
    {
        var user = _mapper.Map<User>(model);
        try
        {
            await VerifyRoleIsPermitedAsync(user.Role);
            string nome = (user.FirstName + user.LastName)
                .Replace(" ", "")  // remove TODOS os espaços
                .ToLowerInvariant(); // opcional: tudo minúsculo

            user.UserName = nome + DateTime.Now.Millisecond;      
            var createdUser = await _userRepository.AddUserAsync(user);

            if (user.SignInAfterCreation) //logar o cliente quando realizar o cadastro
                await AuthAsync(createdUser);//preciso criar um retorna diferente para quem for admin e quando for user                

            return _mapper.Map<UserDTO>(createdUser);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao adicionar usuário", ex);
        }
    }

    public async Task<List<UserDTO>> GetUsersAsync()
    {
        var users = await _userRepository.GetUsersAsync();

        var userDTOs = _mapper.Map<List<UserDTO>>(users);
        foreach (var user in userDTOs)
        {
            user.Id = users.Find(u => u.Email == user.Email)?.Id ?? 0;
        }

        return _mapper.Map<List<UserDTO>>(users);
    }

    public async Task<UserDTO> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<LoginDTO> GetUserByEmailToLoginAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        return _mapper.Map<LoginDTO>(user);
    }

    public async Task<UserDTO> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<List<object>> GetUserByNameAsync(string name)
    {
        var allUsers = await _userRepository.GetUsersAsync();
        var userFound = allUsers.FindAll(u => u.FirstName == name);

        if (!userFound.Any())
        {
            return new List<object>(); // Retorna uma lista vazia para indicar que nenhum usuário foi encontrado
        }

        var usersWithRoles = new List<object>();

        foreach (var user in userFound)
        {
            var userConvert = _mapper.Map<UserDTO>(user);
            var roles = await GetRolesAsync(userConvert);
            usersWithRoles.Add(new { User = _mapper.Map<UserDTO>(user), Roles = roles });
        }

        return usersWithRoles;
    }

    public async Task AuthAsync(User user)
    {
        // apontar para auth controller
        throw new NotImplementedException();
    }

    public async Task<UserUpdateFromUserDTO> UpdateUserFromUserAsync(UserUpdateFromUserDTO userDTO)
    {
        var userToUpdate = await _userRepository.GetUserByEmailAsync(userDTO.Email);

        if (userToUpdate == null)
            throw new ArgumentNullException(nameof(userToUpdate), "Usuário não encontrado");

        _mapper.Map(userDTO, userToUpdate);

        var updatedUser = await _userRepository.UpdateUserAsync(userToUpdate);

        if (updatedUser == null)
            throw new Exception("Usuário não atualizado");

        return _mapper.Map<UserUpdateFromUserDTO>(updatedUser);
    }

    public async Task<UserUpdateFromAdminDTO> UpdateUserFromAdminAsync(UserUpdateFromAdminDTO userDTO)
    {
        // Buscar o usuário existente do banco
        var userToUpdate = await _userRepository.GetUserByEmailAsync(userDTO.Email);
        if (userToUpdate == null)
            throw new ArgumentNullException(nameof(userToUpdate), "Usuário não encontrado");

        // Atualizar apenas os campos permitidos do DTO no usuário existente
        _mapper.Map(userDTO, userToUpdate);

        // Chamar o repositório para atualizar o usuário
        var updatedUser = await _userRepository.UpdateUserAsync(userToUpdate);

        // Retornar o DTO mapeado do usuário atualizado
        return _mapper.Map<UserUpdateFromAdminDTO>(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não encontrado");

        return await _userRepository.DeleteUserAsync(user);
    }

    public async Task<List<string>> GetRolesAsync(UserDTO userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        return await _userRepository.GetRolesAsync(user);
    }

    public async Task VerifyRoleIsPermitedAsync(string role)
    {
        await Task.Run(() =>
        {
            if (!_validRoles.Contains(role))
            {
                throw new ArgumentException(
                    $"O papel '{role}' é inválido.", nameof(role));
            }
        });
    }
}
