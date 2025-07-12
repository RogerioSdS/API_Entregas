using Api_Entregas.Services.Interfaces;
using Api_Entregas.ViewModels;
using Newtonsoft.Json;

namespace Api_Entregas.Services.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetUserData(SignInViewModel userData)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("UserData", JsonConvert.SerializeObject(userData));
        }

        public SignInViewModel GetUserData()
        {
            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString("UserData");
            return string.IsNullOrEmpty(sessionData) ? null : JsonConvert.DeserializeObject<SignInViewModel>(sessionData);
        }

        public void ClearUserData()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("UserData");
        }
    }
}