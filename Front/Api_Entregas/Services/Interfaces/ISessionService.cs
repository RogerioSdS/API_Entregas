using System;
using Api_Entregas.ViewModels;

namespace Api_Entregas.Services.Interfaces;

public interface ISessionService
{
    void SetUserData<T>(string context,T userData);
    public T? GetUserData<T>(string context);
    void ClearUserData(string context);
    bool IsLoggedIn(string context);
}

