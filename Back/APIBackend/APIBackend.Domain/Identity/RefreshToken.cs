using System;

namespace APIBackend.Domain.Identity;

public class RefreshToken
    {
        public int UserId { get; set; }
        public string? Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public User User { get; set; } 
    }