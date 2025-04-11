using System;
using APIBackend.Application.DTOs;

namespace APIBackend.Application.Services.Interfaces;

public interface IAuthService
{    
    Task<UserDTO> AuthenticateUserAsync(string email, string password);
}
