using APIBackend.Application.DTOs;

namespace APIBackend.Application.Services.Interfaces;

public interface IUserService
{    
    Task<UserDTO> AddUserAsync(UserDTO userDTO);
    Task<List<UserDTO>> GetUsersAsync();
    Task<UserDTO> GetUserByIdAsync(int id); 
    Task<List<object>> GetUserByNameAsync(string name); 
    Task<UserDTO> UpdateUserAsync(UserDTO userDTO);
    Task<bool> DeleteUserAsync(UserDTO userDTO);
    Task<List<string>> GetRolesAsync(UserDTO userDTO);
}
