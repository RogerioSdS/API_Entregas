using System;

namespace APIBackend.Domain.Identity;

public class RefreshToken
{
    public int Id { get; set; }  // Chave primária do token
    public string Token { get; set; }  // O token gerado
    public DateTime CreatedAt { get; set; }  // Quando o token foi criado
    public DateTime ExpiresAt { get; set; }  // Quando o token expira
    public bool IsRevoked { get; set; } = false;  // Se o token foi revogado ou não

    // Chave estrangeira para User
    public int UserId { get; set; }  // O UserId é necessário como chave estrangeira
    public User User { get; set; }  // Propriedade de navegação para o usuário
}