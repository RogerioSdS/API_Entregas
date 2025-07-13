using System;
using Api_Entregas.Services.Models;
using Api_Entregas.ViewModels;

namespace Api_Entregas.Services.Interfaces;

    public interface IAuthService
    {
        Task<ServiceResult<SignInViewModel>> LoginAsync(LoginViewModel model);
        Task<ServiceResult<string>> LogoutAsync();
        Task<ServiceResult<UserViewModel?>> GetUserAsync(LoginViewModel model);
        Task<ServiceResult<SignInViewModel>> RegisterAsync(RegisterViewModel model);
        Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<ServiceResult<object>> GetUserByEmailAsync(string email);
    }

