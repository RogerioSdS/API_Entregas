using System;
using APIBackend.Domain.Identity;

namespace APIBackend.Repositories.Interfaces;

public interface IAuthRepo
{
    Task SaveAsync(int userId, string token, DateTime expiresAt);
    Task<RefreshToken> GetByTokenAsync(string token);
    Task RevokeAsync(string token);

}
