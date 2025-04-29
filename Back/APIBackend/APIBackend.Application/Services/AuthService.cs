using System.ComponentModel.DataAnnotations.Schema;
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
    public async Task<UserDTO?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _apiDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        var userDto = _mapper.Map<UserDTO>(user);

        var userDtoWithRoles = await GetUserRolesAsync(userDto);        

        return userDtoWithRoles;
    }

    public async Task<UserDTO> GetUserRolesAsync( UserDTO userDto)
    {
        var user = _mapper.Map<User>(userDto);

        var roles = await _userManager.GetRolesAsync(user);
        userDto.Role = roles.FirstOrDefault() ?? string.Empty;

        return userDto;
    }

    /* Arrumar os metodos comentados


        public async Task<bool> ValidateRefreshTokenByToken(string refreshToken)
        {
            var isValidRefreshToken =  await _context.RefreshTokens
            .Where(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

            if(isValidRefreshToken == null || isValidRefreshToken.Count > 0)
            {
                return false;
            }

            return true;
        }
    }

    */

    public async Task<string> GenerateJwtTokenAsync(UserDTO user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role )
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token));
    }

    public async Task RevokeRefreshTokenAsync(int tokenId)
    {
        await _authRepo.RevokeTokenAsync(tokenId);
    }

    public async Task RevokeOldTokensAsync(int userId)
    {
        var allTokens = await _authRepo.GetAllTokenByIdAsync(userId);

        if (allTokens == null || allTokens.Count == 0)
        {
            return;
        }

        var tokenReturn = new List<RefreshToken>(); 

        foreach (var token in allTokens)
        {
            if (token.ExpiresAt < DateTime.UtcNow && !token.IsRevoked)
            {
                token.IsRevoked = true;
            }

            tokenReturn.Add(token);
        }

        await _authRepo.UpdateTokenAsync(tokenReturn);
    }

    public async Task<RefreshTokenDTO> SaveRefreshTokenAsync(UserDTO user)
    {
        var randomBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
        var codHashToken = Convert.ToBase64String(randomBytes);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = codHashToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        //await _authRepo.DeleteTokenAsync(refreshToken);
        await _authRepo.SaveTokenAsync(refreshToken);
        var refreshTokenDto = _mapper.Map<RefreshTokenDTO>(refreshToken);

        return refreshTokenDto;
    }

    public async Task<RefreshTokenDTO?> GetRefreshTokenByIdAsync(int tokenId)
    {
        var token = await _authRepo.GetTokenByIdAsync(tokenId);
        if (token == null)
        {
            return null;
        }

        var refreshTokenDto = _mapper.Map<RefreshTokenDTO>(token);
        return refreshTokenDto;
    }

    public async Task<RefreshTokenDTO?> ValidateRefreshTokenAsync(int tokenId)
    {
        var token = await _authRepo.GetTokenByIdAsync(tokenId);
        if (token == null || token.ExpiresAt < DateTime.UtcNow || token.IsRevoked)
        {
            return null;
        }

        var refreshTokenDto = _mapper.Map<RefreshTokenDTO>(token);
        return refreshTokenDto;
    }

    public async Task<UserDTO?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var user = await _authRepo.GetUserByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return null;
        }

        var userDto = _mapper.Map<UserDTO>(user);

        return userDto;
    }

    public async Task RemoveOldTokensAsync(int userId)
    {
        var allTokens = await _authRepo.GetAllTokenByIdAsync(userId);

        if (allTokens == null || allTokens.Count == 0)
        {
            return;
        }

        var tokenToRemove = new List<RefreshToken>(); 

        foreach (var token in allTokens)
        {
            if (token.ExpiresAt > DateTime.UtcNow + TimeSpan.FromDays(60))
            {
                tokenToRemove.Add(token);
            }
        }

        if (tokenToRemove.Count == 0)
        {
            return;
        }

        await _authRepo.RemoveOldTokensAsync(tokenToRemove);
    }

    public async Task RevokeTokensAsync(int id)
    {
        await RemoveOldTokensAsync(id);
        await RevokeOldTokensAsync(id);
        await RevokeRefreshTokenAsync(id);
    }
}
