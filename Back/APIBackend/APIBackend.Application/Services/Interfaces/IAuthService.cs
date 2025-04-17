using System;
using APIBackend.Application.DTOs;

namespace APIBackend.Application.Services.Interfaces;

public interface IAuthService
{
    public Task<UserDTO> AuthenticateUserAsync(string email, string password);
    public Task<string> GenerateJwtTokenAsync(UserDTO user);
    public Task SaveRefreshTokenAsync(int userId); 
    public Task<RefreshTokenDTO?> ValidateRefreshTokenAsync(int userId); 
    public Task<RefreshTokenDTO> GetRefreshTokenByIdAsync(int userId);
    public Task RevokeRefreshTokenAsync(string refreshToken);
}
