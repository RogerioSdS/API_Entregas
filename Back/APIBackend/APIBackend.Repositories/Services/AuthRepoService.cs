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
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Agora informa explicitamente que o novo token é 'Added'
            _context.Entry(token).State = EntityState.Added;

            // Salva
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteTokenAsync(RefreshToken token)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Deleta o token anterior diretamente no banco
            await _context.RefreshTokens
                .Where(t => t.Id == token.Id)
                .ExecuteDeleteAsync();
            // Salva
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
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
            .Where(u => u.Id == id).ToListAsync();
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .Where(u => u.RefreshToken.Any(rt => rt.Token == refreshToken))
            .FirstOrDefaultAsync();
    }

    public async Task UpdateTokenAsync(List<RefreshToken> tokens)
    {
        foreach (var token in tokens)
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
            return;
        }

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        // Solução: detach cada objeto deletado
        foreach (var entry in _context.ChangeTracker.Entries<RefreshToken>())
        {
            entry.State = EntityState.Detached;
        }
    }

    public async Task RemoveOldTokensAsync(List<RefreshToken> listTokens)
    {
        _context.RefreshTokens.RemoveRange(listTokens);
        await _context.SaveChangesAsync();

        // Solução: detach cada objeto deletado
        foreach (var token in listTokens)
        {
            var entry = _context.Entry(token);
            if (entry != null)
                entry.State = EntityState.Detached;
        }
    }
}
