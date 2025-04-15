using System;
using APIBackend.Application.DTOs;

namespace APIBackend.Application.Services.Interfaces;

public interface IAuthService
{
    public Task<UserDTO> AuthenticateUserAsync(string email, string password);
    public Task<string> GenerateJwtTokenAsync(UserDTO user); // Método para gerar o JWT
    public Task SaveRefreshTokenAsync(int userId, string refreshToken); // Método para salvar o refresh token
    public Task<int?> ValidateRefreshTokenAsync(string refreshToken); // Método para validar o refresh token
    public Task RevokeRefreshTokenAsync(string refreshToken); // Método para revogar o refresh token
}
