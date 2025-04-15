using System;
using APIBackend.Domain.Enum;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace APIBackend.Repositories.Services;

public class AuthRepoService : IAuthRepo
{
    private readonly ApiDbContext _context;

    public AuthRepoService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(int userId, string token, DateTime expiresAt)
    {
        var existsRefreshToken = GetByTokenAsync(id);
        if (existsRefreshToken == null )
        {
            var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsRevoked = false};

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return;
        }
        
        Delete

    }

    public async Task<RefreshToken> DeleteRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens.Where(rt => rt. ); 
    }

    public async Task<RefreshToken> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked) ?? null;
    }

    public async Task RevokeAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}
