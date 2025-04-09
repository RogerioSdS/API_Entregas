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
    public async Task<UserDTO> AddUserAsync(UserDTO model)
    {
        var user = _mapper.Map<User>(model);
        try
        {
            var createdUser = await _userRepository.AddUserAsync(user);
            if (user.SignInAfterCreation)
                await AuthAsync(createdUser);
            //preciso criar um retorna diferente para quem for admin e quando for user
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
    /// <returns>Retorna uma lista de <see cref="UserDTO"/> com todos os usuários.</returns>
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

    /// <summary>
    /// Retorna o usuário com o nome especificado.
    /// </summary>
    /// <param name="name">O nome do usuário a ser retornado.</param>
    /// <returns>Retorna o objeto <see cref="UserDTO"/> que representa o usuário se encontrado, caso contrário retorna null.</returns>
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


    /// <summary>
    /// Realiza o login de um usuário no sistema.
    /// </summary>
    /// <param name="user">O usuário que será autenticado.</param>
    /// <returns>Retorna uma tarefa assíncrona que representa o processo de autenticação.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o usuário é nulo.</exception>
    public async Task AuthAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo");

        await _signInManager.SignInAsync(user, isPersistent: false);
    }

    /// <summary>
    /// Atualiza os dados de um usuário no sistema.
    /// </summary>
    /// <param name="userDTO">Os novos dados do usuário.</param>
    /// <returns>Retorna o objeto <see cref="UserDTO"/> com os dados do usuário atualizado.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o objeto <see cref="UserDTO"/> é nulo.</exception>
    public async Task<UserUpdateFromUserDTO> UpdateUserFromUserAsync(UserUpdateFromUserDTO userDTO)
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

    /// <summary>
    /// Exclui um usuário do sistema.
    /// </summary>
    /// <param name="userDTO">Os dados do usuário a ser excluído.</param>
    /// <returns>Retorna uma tarefa assíncrona que representa o processo de exclusão. Retorna <c>true</c> se o usuário foi excluído com sucesso.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o objeto <see cref="UserDTO"/> é nulo.</exception>
    public async Task<bool> DeleteUserAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id do usuário inválido", nameof(id));

        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Usuário não encontrado");

        return await _userRepository.DeleteUserAsync(user);
    }

    /// <summary>
    /// Retorna a lista de funções (roles) associadas a um usuário.
    /// </summary>
    /// <param name="userDTO">Os dados do usuário para o qual as funções serão retornadas.</param>
    /// <returns>Retorna uma lista de strings representando as funções associadas ao usuário.</returns>
    public async Task<List<string>> GetRolesAsync(UserDTO userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        return await _userRepository.GetRolesAsync(user);
    }
}
