using System;
using APIBackend.Application.DTOs;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIBackend.Application.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Autentica um usuário com email e senha.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="password">Senha do usuário.</param>
    /// <returns>Retorna um <see cref="UserDTO"/> autenticado, ou null se inválido.</returns>
    Task<UserDTO?> AuthenticateUserAsync(string email, string password);

    /// <summary>
    /// Gera um token JWT para o usuário.
    /// </summary>
    /// <param name="user">Usuário para gerar o token.</param>
    /// <returns>Token JWT como string.</returns>
    Task<string> GenerateJwtTokenAsync(UserDTO user);

    /// <summary>
    /// Salva um novo refresh token para o usuário.
    /// </summary>
    /// <param name="userId">Usuário para salvar o token.</param>
    /// <returns>Refresh token criado.</returns>
    Task<RefreshTokenDTO> SaveRefreshTokenAsync(UserDTO userId);

    /// <summary>
    /// Obtém um refresh token pelo ID.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <returns>Refresh token encontrado, ou null.</returns>
    Task<RefreshTokenDTO?> GetValidateRefreshTokenByIdAsync(int userId);

    /// <summary>
    /// Obtém um usuário com base em um refresh token.
    /// </summary>
    /// <param name="refreshToken">Token de atualização.</param>
    /// <returns>Usuário correspondente, ou null.</returns>
    Task<UserDTO?> GetUserByRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revoga um refresh token.
    /// </summary>
    /// <param name="Id">ID do token.</param>
    Task RevokeRefreshTokenAsync(int Id);

    /// <summary>
    /// Revoga todos os tokens associados a um usuário.
    /// </summary>
    /// <param name="id">ID do usuário.</param>
    Task RevokeTokensAsync(int id);

    /// <summary>
    /// Remove tokens antigos de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    Task RemoveOldTokensAsync(int userId);

    /// <summary>
    /// Obtém as roles (funções) do usuário.
    /// </summary>
    /// <param name="user">DTO do usuário.</param>
    /// <returns>DTO do usuário com as roles preenchidas.</returns>
    Task<UserDTO> GetUserRolesAsync(UserDTO user);

    /// <summary>
    /// Cria e envia o token de confirmação de e-mail.
    /// </summary>
    /// <param name="model">Modelo com e-mail de destino.</param>
    /// <returns>Link de confirmação gerado, ou null.</returns>
    Task<string?> CreatedEmailConfirmationAsync(EmailDTO model);

    /// <summary>
    /// Confirma o e-mail de um usuário.
    /// </summary>
    /// <param name="email">Email a ser confirmado.</param>
    /// <param name="token">Token de confirmação.</param>
    /// <returns>True se confirmado com sucesso.</returns>
    Task<bool> ConfirmEmailAsync(string email, string token);

    /// <summary>
    /// Cria um token para redefinição de senha e gera um link.
    /// </summary>
    /// <param name="model">Modelo com o e-mail do usuário.</param>
    /// <returns>URL com o token, ou null.</returns>
    Task<string?> CreatedResetPasswordTokenAsync(EmailDTO model);

    /// <summary>
    /// Confirma a redefinição de senha.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="token">Token de redefinição.</param>
    /// <param name="newPassword">Nova senha.</param>
    /// <returns>True se redefinido com sucesso.</returns>
    Task<bool> ConfirmResetPasswordAsync(string email, string token, string newPassword);

    /// <summary>
    /// Envia um e-mail genérico.
    /// </summary>
    /// <param name="email">Destinatário.</param>
    /// <param name="subject">Assunto.</param>
    /// <param name="message">Mensagem (HTML permitido).</param>
    Task SendEmailAsync(string email, string subject, string message);

    /// <summary>
    /// Valida um refresh token.
    /// </summary>
    /// <param name="token">Token de atualização.</param>
    /// <returns>True se dentro da validade.</returns>
    Task<bool> ValidateRefreshTokenAsync(string token);

    /// <summary>
    /// Obtém um refresh token pelo token de atualização.
    /// </summary>
    /// <param name="refreshToken">Token de atualização.</param>
    /// <returns>Refresh token encontrado, ou null.</returns>
    public Task<RefreshToken?> GetTokenByRefreshTokenAsync(string refreshToken);
}

