using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIBackend.Application.DTOs;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APIBackend.Application.Services;

public class AuthService(IConfiguration configuration, ApiDbContext refreshTokenRepository, IMapper mapper, UserManager<User> userManager, IAuthRepo authRepo) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ApiDbContext _apiDbContext = refreshTokenRepository; 
    private readonly IAuthRepo _authRepo = authRepo;
    private readonly UserManager<User> _userManager = userManager;
    public readonly IMapper _mapper = mapper;

/// 
    public async Task<UserDTO> AuthenticateUserAsync(string email, string password)
    {        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var user = await _apiDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return null; 
        }

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<string> GenerateJwtTokenAsync(UserDTO user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "user")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return await Task.Run (() => new JwtSecurityTokenHandler().WriteToken(token));
    }


    public Task RevokeRefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public async Task CreateOrSaveRefreshTokenAsync(int userId, string refreshToken)
    {
        await _authRepo.SaveAsync(userId, refreshToken, DateTime.UtcNow.AddDays(7));
    }

    public async Task<int?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var token = await _authRepo.GetByTokenAsync(refreshToken);
        if (token == null || token.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }
        return token.UserId;
    }
    
}
