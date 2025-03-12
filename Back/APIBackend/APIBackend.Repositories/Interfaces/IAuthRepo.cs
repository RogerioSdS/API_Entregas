using System;
using APIBackend.Domain.Models;

namespace APIBackend.Repositories.Interfaces;

public interface IAuthRepo
{
    public Task<Auth> AuthLogin(Auth model);
}
