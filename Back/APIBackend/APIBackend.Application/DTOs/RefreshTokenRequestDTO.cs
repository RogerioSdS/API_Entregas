using System;
using System.ComponentModel.DataAnnotations;

namespace APIBackend.Application.DTOs;

public class RefreshTokenRequestDTO
{
    [Required(ErrorMessage = "O refresh token é obrigatório.")] 
    public required string RefreshToken { get; set; } 
}
