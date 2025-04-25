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
}
