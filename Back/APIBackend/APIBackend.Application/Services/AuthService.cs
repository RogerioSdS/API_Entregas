using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
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
using NLog;
using System.Net.Mail;
using System.Net;

namespace APIBackend.Application.Services;

public class AuthService(IConfiguration configuration, ApiDbContext refreshTokenRepository, IMapper mapper, UserManager<User> userManager, IAuthRepo authRepo) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ApiDbContext _apiDbContext = refreshTokenRepository;
    private readonly IAuthRepo _authRepo = authRepo;
    private readonly UserManager<User> _userManager = userManager;
    public readonly IMapper _mapper = mapper;
    protected Logger _loggerNLog = LogManager.GetCurrentClassLogger();

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

    public async Task<UserDTO> GetUserRolesAsync(UserDTO userDto)
    {
        var user = _mapper.Map<User>(userDto);

        var roles = await _userManager.GetRolesAsync(user);
        userDto.Role = roles.FirstOrDefault() ?? string.Empty;

        return userDto;
    }

    public async Task<string> GenerateJwtTokenAsync(UserDTO user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role )
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key está faltando na configuração.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(0.2),
            signingCredentials: creds);

        Console.WriteLine("Token gerado: " + DateTime.UtcNow + " Token vence: " + token.ValidTo);

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

    public async Task<RefreshTokenDTO?> GetValidateRefreshTokenByIdAsync(int tokenId)
    {
        var token = await _authRepo.GetValideTokenByIdAsync(tokenId);
        if (token == null)
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

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"] ?? "587"), // Padrão 587 para TLS
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"] ?? ""),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            _loggerNLog.Info($"Tentando enviar e-mail para: {email}, Assunto: {subject}");
            await smtpClient.SendMailAsync(mailMessage);
            _loggerNLog.Info($"E-mail enviado com sucesso para: {email}");
        }
        catch (SmtpException ex)
        {
            _loggerNLog.Error(ex, $"Erro SMTP ao enviar e-mail para: {email}. Código de erro: {ex.StatusCode}, Mensagem: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _loggerNLog.Error(ex, $"Erro geral ao enviar e-mail para: {email}. Mensagem: {ex.Message}");
            throw;
        }
    }

    public async Task<string?> CreatedEmailConfirmationAsync(EmailDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            _loggerNLog.Info($"Usuário não encontrado para confirmação de e-mail: {model.Email}");
            return null;
        }
        if (user.EmailConfirmed)
        {
            _loggerNLog.Info($"E-mail já confirmado: {model.Email}");
            return $"E-mail já estava confirmado: {model.Email}";
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        if (token == null)
        {
            _loggerNLog.Info($"Erro ao gerar token de confirmação para: {model.Email}");
            return null;
        }

        var encodedToken = HttpUtility.UrlEncode(token); // Codificar para URL
        var urlToTokenConfirmedEmail = $"{_configuration["Url:ApiUrl"]}/api/Auth/ConfirmEmail?email={HttpUtility.UrlEncode(model.Email)}&token={encodedToken}";

        try
        {
            // Enviar e-mail
            var message = $@"<h3>Confirme seu e-mail</h3>
                        <p>Por favor, confirme seu e-mail clicando no link abaixo:</p>
                        <p><a href='{urlToTokenConfirmedEmail}'>Confirmar e-mail</a></p>";
            await SendEmailAsync(model.Email, "Confirme seu e-mail", message);
            _loggerNLog.Info($"E-mail de confirmação enviado para: {model.Email}");
        }
        catch (Exception ex)
        {
            _loggerNLog.Error(ex, $"Erro ao enviar e-mail de confirmação para: {model.Email}");
            return null;
        }

        return urlToTokenConfirmedEmail;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _loggerNLog.Info($"Usuário não encontrado para confirmação: {email}");
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                _loggerNLog.Info($"Erro ao confirmar e-mail: {email}. Erros: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return false;
            }

            _loggerNLog.Info($"E-mail confirmado com sucesso: {email}");
            return true;
        }
        catch (Exception ex)
        {
            _loggerNLog.Error(ex, $"Erro inesperado ao confirmar e-mail: {email}. Detalhes: {ex.Message}");
            return false;
        }
    }

    public async Task<string?> CreatedResetPasswordTokenAsync(EmailDTO model)
    {
        var token = await _authRepo.CreateResetPasswordTokenAsync(model.Email);

        if (token == null)
        {
            return null;
        }

        var urlToResetPassword = $"{_configuration["Url:ApiUrl"]}/api/Auth/ConfirmEmail?email={HttpUtility.UrlEncode(model.Email)}&token={HttpUtility.UrlEncode(token)}";

        try
        {
            var message = $@"<h3>Redefinir sua senha</h3>
                    <p>Você solicitou a redefinição de senha. Clique no link abaixo para redefinir:</p>
                    <p><a href='{urlToResetPassword}'>Redefinir senha</a></p>";

            await SendEmailAsync(model.Email, "Redefinir senha", message);
            _loggerNLog.Info($"E-mail de redefinição de senha enviado para: {model.Email}");
        }
        catch (Exception ex)
        {
            _loggerNLog.Error(ex, $"Erro ao enviar e-mail de redefinição de senha para: {model.Email}");
            return null;
        }

        return urlToResetPassword;
    }

    public async Task<bool> ConfirmResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            return await _authRepo.ResetPasswordAsync(email, token, newPassword);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        try
        {
            var refreshToken = await _authRepo.GetRefreshTokenByRefreshTokenAsync(token);
            if (refreshToken != null)
            {
                return true;
            }
            return false;
        }
        catch (System.Exception ex)
        {
            throw new Exception("Erro ao validar refresh token: " + ex.Message);
        }
    }

    public Task<RefreshToken?> GetTokenByRefreshTokenAsync(string refreshToken)
    {
        return _authRepo.GetTokenByRefreshTokenAsync(refreshToken);
    }
    
}
