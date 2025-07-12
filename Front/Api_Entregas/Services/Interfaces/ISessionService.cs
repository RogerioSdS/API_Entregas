using System;
using Api_Entregas.ViewModels;

namespace Api_Entregas.Services.Interfaces;

    public interface ISessionService
    {
        void SetUserData(SignInViewModel userData);
        SignInViewModel GetUserData();
        void ClearUserData();
    }

