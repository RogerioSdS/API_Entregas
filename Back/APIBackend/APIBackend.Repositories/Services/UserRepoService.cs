using System;
using APIBackend.Domain;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Services;

public class UserRepoService : IUserRepo
{
    private readonly ApiDbContext _context;

    public UserRepoService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User>? GetUserAsync(int id)    
    {
        var user = await _context.Users.FindAsync(id);
        
        return user;
    }

    public async Task<User> AddUserAsync(User user)
    {
        if (user == null)
        {
            throw new Exception("Usuário nulo.");
        }

        var entityEntry = await _context.Users.AddAsync(user); // Adiciona assincronamente

        if (entityEntry != null)
        {
            await _context.SaveChangesAsync(); // Persiste no banco
        }
        return entityEntry.Entity; // Retorna a entidade adicionada
    }

    public Task<User> AuthUser(User user)
    {
        throw new NotImplementedException();
    }
}