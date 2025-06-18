using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IAuthRepo
{
    public Task SaveTokenAsync(RefreshToken token);
    public Task<RefreshToken?> GetTokenByIdAsync(int id);
    public Task<RefreshToken?> GetTokenByRefreshTokenAsync(string refreshToken);
    public Task<List<RefreshToken>?> GetAllTokenByIdAsync(int id);
    public Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    public Task RevokeTokenAsync(int id);
    public Task UpdateTokenAsync(List<RefreshToken> tokens);
    public Task RemoveOldTokensAsync(List<RefreshToken> listTokens);
    public Task DeleteTokenAsync(RefreshToken token);
    public Task<string> CreateEmailConfirmationTokenAsync(string email);
    public Task<bool> ConfirmEmailAsync(string email, string token);
    public Task<string> CreateResetPasswordTokenAsync(string email);
    public Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

}
