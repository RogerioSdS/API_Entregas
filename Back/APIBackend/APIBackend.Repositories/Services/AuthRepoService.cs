using System;
using APIBackend.Domain.Models;
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

    public async Task<Auth> AuthLogin(Auth model)
    {
        var auth = await _context.Auth.FirstAsync(a => a.Email == model.Email && a.Password == model.Password);  

        return auth;      
    }
}
