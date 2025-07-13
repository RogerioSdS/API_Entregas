using Api_Entregas.Services.Interfaces;
using Api_Entregas.ViewModels;
using Newtonsoft.Json;

namespace Api_Entregas.Services.Implementations
{
     public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string UserDataKey = "UserData";

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetUserData(SignInViewModel userData)
        {
            var json = JsonConvert.SerializeObject(userData);
            _httpContextAccessor.HttpContext?.Session.SetString(UserDataKey, json);
        }

        public SignInViewModel? GetUserData()
        {
            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString(UserDataKey);
            return string.IsNullOrWhiteSpace(sessionData) 
                ? null 
                : JsonConvert.DeserializeObject<SignInViewModel>(sessionData);
        }

        public bool IsLoggedIn()
        {
            return GetUserData() != null;
        }

        public void ClearUserData()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(UserDataKey);
        }
    }
}