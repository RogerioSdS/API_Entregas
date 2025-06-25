using System;
using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

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
    public Task<string?> CreatedEmailConfirmationAsync(EmailDTO model);
    public Task<bool> ConfirmEmailAsync(string email, string token);
    public Task<string?> CreatedResetPasswordTokenAsync(EmailDTO model);
    public Task<bool> ConfirmResetPasswordAsync(string email, string token, string newPassword);
    public Task SendEmailAsync(string email, string subject, string message);
}
