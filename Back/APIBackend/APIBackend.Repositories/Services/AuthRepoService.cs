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

    public async Task SaveTokenAsync(RefreshToken token)
    {        
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetTokenByIdAsync(int id)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.User.Id == id && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<RefreshToken?> GetTokenByRefreshTokenAsync(string refreshToken)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow); 
    }

    /// <summary>
    /// Retorna uma lista de todos os tokens de refresh associados ao usuário de id especificado.
    /// </summary>
    /// <param name="id">O id do usuário para o qual os tokens de refresh serão retornados.</param>
    /// <returns>Retorna uma lista de Refresh Tokens <see cref="RefreshToken"/> ou null se n o houver tokens de refresh associados ao usu rio.</returns> 
    public async Task<List<RefreshToken>?> GetAllTokenByIdAsync(int id)
    {
        return await _context.RefreshTokens
            .Where(u => u.UserId == id).ToListAsync();            
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken) ?? null;
    }

    public async Task UpdateTokenAsync(List<RefreshToken> tokens)
    {
            foreach(var token in tokens)
            {
                _context.RefreshTokens.Update(token);
            }

            await _context.SaveChangesAsync();
    }

    public async Task RevokeTokenAsync(int id)
    {
        var refreshToken = await GetTokenByIdAsync(id);
        if (refreshToken == null)
        {
            throw new Exception("Refresh token not found or already invalid.");
        }

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();
    }
}
