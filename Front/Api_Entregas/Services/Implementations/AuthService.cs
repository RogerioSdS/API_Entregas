using Api_Entregas.Services.Interfaces;
using Api_Entregas.Services.Models;
using Api_Entregas.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Api_Entregas.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ServiceResult<SignInViewModel>> LoginAsync(LoginViewModel model)
        {
            try
            {
                var urlPath = "/login";
                string apiUrl = $"{_configuration["ApiBackendSettings:AuthUrl"]}{urlPath}";
                var jsonBody = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Fazendo requisição para {apiUrl}");

                var response = await _httpClient.PostAsync(apiUrl, content);

                //Testando o Set-Cookie
                if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
                {
                    foreach (var cookie in setCookies)
                    {
                        Console.WriteLine("Cookie recebido da API: " + cookie);
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum Set-Cookie recebido da API.");
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userData = JsonConvert.DeserializeObject<SignInViewModel>(responseContent);

                    return ServiceResult<SignInViewModel>.SuccessResult(userData);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Erro na requisição para {apiUrl}. Status: {response.StatusCode}. Resposta: {errorContent}");
                return ServiceResult<SignInViewModel>.ErrorResult("E-mail ou senha inválidos.", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar o usuário.");
                return ServiceResult<SignInViewModel>.ErrorResult("Ocorreu um erro ao processar o login.");
            }
        }

        public async Task<ServiceResult<string>> LogoutAsync()
        {
            try
            {
                var urlPath = "/logout";
                string apiUrl = $"{_configuration["ApiBackendSettings:AuthUrl"]}{urlPath}";

                var response = await _httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult<string>.SuccessResult("Solicitação enviada com sucesso!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Falha na solicitação de reset. Status: {response.StatusCode}, Resposta: {response.Headers}");

                return ServiceResult<string>.ErrorResult(errorContent, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar esqueci minha senha.");

                return ServiceResult<string>.ErrorResult("Ocorreu um erro ao processar a solicitação.");
            }
        }

        public async Task<ServiceResult<string>> LoginByRefreshTokenAsync()
        {
            try
            {
                var urlPath = "/loginByRefreshToken";
                string apiUrl = $"{_configuration["ApiBackendSettings:AuthUrl"]}{urlPath}";

                // Recupera o cookie refresh_token do contexto HTTP
                var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return ServiceResult<string>.ErrorResult("Refresh token não encontrado no cookie.", 401);
                }

                // Cria a requisição manualmente para adicionar o cookie
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("Cookie", $"refresh_token={refreshToken}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult<string>.SuccessResult("Token renovado com sucesso!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Falha na solicitação de refresh. Status: {response.StatusCode}, Resposta: {errorContent}");

                return ServiceResult<string>.ErrorResult(errorContent, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar renovar o token.");
                return ServiceResult<string>.ErrorResult("Ocorreu um erro ao tentar renovar o token.");
            }
        }


        public Task<ServiceResult<UserViewModel?>> GetUserAsync(LoginViewModel model)
        {
            throw new NotImplementedException();
        }
        /*
                                public async Task<ServiceResult<UserViewModel?>> GetUserAsync(LoginViewModel model)
                                {
                                    try
                                    {
                                        string apiUrl = $"{_configuration["ApiBackendSettings:UserUrl"]}/login";
                                        var jsonBody = JsonConvert.SerializeObject(model);
                                        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                                        _logger.LogInformation($"Fazendo requisição para {apiUrl}");

                                        var response = await _httpClient.PostAsync(apiUrl, content);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var responseContent = await response.Content.ReadAsStringAsync();
                                            var userData = JsonConvert.DeserializeObject<SignInViewModel>(responseContent);

                                            return ServiceResult<SignInViewModel>.SuccessResult(userData);
                                        }

                                        var errorContent = await response.Content.ReadAsStringAsync();
                                        _logger.LogError($"Erro na requisição para {apiUrl}. Status: {response.StatusCode}. Resposta: {errorContent}");
                                        return ServiceResult<SignInViewModel>.ErrorResult("E-mail ou senha inválidos.", (int)response.StatusCode);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Erro ao autenticar o usuário.");
                                        return ServiceResult<SignInViewModel>.ErrorResult("Ocorreu um erro ao processar o login.");
                                    }
                                }
                        */
        public async Task<ServiceResult<SignInViewModel>> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                string apiUrl = $"{_configuration["ApiBackendSettings:UserUrl"]}/createUser";
                var jsonBody = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Fazendo requisição para {apiUrl}");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userData = JsonConvert.DeserializeObject<SignInViewModel>(responseContent);

                    _logger.LogInformation($"Resposta da API: {responseContent}");
                    return ServiceResult<SignInViewModel>.SuccessResult(userData);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Erro na requisição para {apiUrl}. Status: {response.StatusCode}. Resposta: {errorContent}");
                return ServiceResult<SignInViewModel>.ErrorResult("Erro ao registrar usuário.", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar o usuário.");
                return ServiceResult<SignInViewModel>.ErrorResult("Ocorreu um erro ao processar o registro. Tente novamente.");
            }
        }

        public async Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            try
            {
                string apiUrl = $"{_configuration["ApiBackendSettings:AuthUrl"]}/GenerateResetPasswordToken";
                var jsonBody = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult<string>.SuccessResult("Solicitação enviada com sucesso!");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Falha na solicitação de reset. Status: {response.StatusCode}, Resposta: {errorContent}");
                return ServiceResult<string>.ErrorResult("Email não encontrado.", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar esqueci minha senha.");
                return ServiceResult<string>.ErrorResult("Ocorreu um erro ao processar a solicitação.");
            }
        }

        public async Task<ServiceResult<object>> GetUserByEmailAsync(string email)
        {
            try
            {
                string apiUrl = $"{_configuration["ApiBackendSettings:ForgotPasswordUrl"]}/getUserByEmail?email={Uri.EscapeDataString(email)}";

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var userData = JsonConvert.DeserializeObject<object>(content);
                    return ServiceResult<object>.SuccessResult(userData);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Usuário não encontrado: {email}. Status: {response.StatusCode}, Resposta: {errorContent}");
                return ServiceResult<object>.ErrorResult("Usuário não encontrado.", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por email.");
                return ServiceResult<object>.ErrorResult("Erro interno do servidor.");
            }
        }
    }
}
