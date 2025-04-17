using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IAuthRepo
{
    Task<RefreshToken> SaveTokenAsync(int userId, string token, DateTime expiresAt);
    Task<RefreshToken> GetTokenByIdAsync(int id);
    Task RevokeAsync(int id);

}
