using Api_Entregas.Services.Interfaces;
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

        public void SetUserData<T>(string context, T userData)
        {
            var json = JsonConvert.SerializeObject(userData);
            _httpContextAccessor.HttpContext?.Session.SetString(context, json);
        }

        public T? GetUserData<T>(string context)
        {
            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString(context);
            return sessionData != null ? JsonConvert.DeserializeObject<T>(sessionData) : default;
        }

        public void ClearUserData(string context)
        {
            _httpContextAccessor.HttpContext?.Session.Remove(context);
        }

        public bool IsLoggedIn(string context)
        {
            var sessionData = _httpContextAccessor.HttpContext?.Session.GetString(context);
            return !string.IsNullOrEmpty(sessionData);
        }
    }
}
