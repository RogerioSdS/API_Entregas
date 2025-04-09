using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.Services.Interfaces;

public interface IUserService
{    
    Task<UserDTO> AddUserAsync(UserDTO userDTO);
    Task<List<UserDTO>> GetUsersAsync();
    Task<UserDTO> GetUserByIdAsync(int id); 
    Task<List<object>> GetUserByNameAsync(string name); 
    Task<UserUpdateFromUserDTO> UpdateUserFromUserAsync(UserUpdateFromUserDTO userDTO);
    Task<UserUpdateFromAdminDTO> UpdateUserFromAdminAsync(UserUpdateFromAdminDTO userDTO);
    Task<bool> DeleteUserAsync(int id);
    Task<List<string>> GetRolesAsync(UserDTO userDTO);
}
