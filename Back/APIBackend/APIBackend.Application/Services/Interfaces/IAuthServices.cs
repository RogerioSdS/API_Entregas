using System;
using APIBackend.Domain.Models;

namespace APIBackend.Application.Services.Interfaces;

public interface IAuthServices
{
    public Task<Auth> AuthLogin(Auth model);
}
