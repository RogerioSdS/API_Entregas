using System;
using APIBackend.Domain.Models;
using APIBackend.Repositories.Interfaces;

namespace APIBackend.Application.Services.Interfaces;

public class AuthService : IAuthServices
{
    private readonly IAuthRepo _authRepository;

    public AuthService(IAuthRepo authRepository)
    {
        _authRepository = authRepository;
    }
    public Task<Auth> AuthLogin(Auth model)
    {
        if (model == null)
        {
            throw new Exception("Modelo nulo.");
        }

        return _authRepository.AuthLogin(model);        
    }
}
