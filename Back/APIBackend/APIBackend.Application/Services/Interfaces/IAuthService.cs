using System;
using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;

namespace APIBackend.Application.Services.Interfaces;

public interface IAuthService
{
    public Task<UserDTO?> AuthenticateUserAsync(string email, string password);
    public Task<string> GenerateJwtTokenAsync(UserDTO user);
    public Task<RefreshTokenDTO> SaveRefreshTokenAsync(UserDTO userId); 
    public Task<RefreshTokenDTO?> ValidateRefreshTokenAsync(int userId); 
    public Task<RefreshTokenDTO?> GetRefreshTokenByIdAsync(int userId);
    public Task<UserDTO?> GetUserByRefreshTokenAsync(string refreshToken);
    public Task RevokeRefreshTokenAsync(int Id);
    public Task RevokeTokensAsync(int id);
    public Task RemoveOldTokensAsync(int userId);
    public Task<UserDTO> GetUserRolesAsync(UserDTO user);
}
