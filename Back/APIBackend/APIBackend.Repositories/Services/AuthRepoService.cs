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

    public async Task<RefreshToken> SaveTokenAsync(int userId, string token, DateTime expiresAt)
    {
        await RevokeOldTokensAsync(userId);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        
        return refreshToken;
    }

    public async Task<RefreshToken> GetTokenByIdAsync(int id)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.User.Id == id && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
    }

    public async Task RevokeAsync(int id)
    {
        // Revoke a specific token by Id (e.g., for logout)
        var refreshToken = await GetTokenByIdAsync(id);
        if (refreshToken == null)
        {
            throw new Exception("Refresh token not found or already invalid.");
        }

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    private async Task RevokeOldTokensAsync(int userId)
    {
        //Buscando todos os users com tokens vencidos
        var oldTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        //Marcando todos com a flag regovada como true
        foreach (var token in oldTokens)
        {
            token.IsRevoked = true;
        }

        if (oldTokens.Any())
        {
            await _context.SaveChangesAsync();
        }
    }
}
