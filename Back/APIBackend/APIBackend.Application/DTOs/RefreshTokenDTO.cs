using System;

namespace APIBackend.Application.DTOs;

public class RefreshTokenDTO
{
    public string? Token { get; set; }= string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
