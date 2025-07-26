using System;
using Api_Entregas.Services.Interfaces;
using Api_Entregas.Services.Models;
using Api_Entregas.ViewModels;

namespace Api_Entregas.Services.Implementations;

public class ClassesDetailsService : IClassesService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClassesDetailsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;
    }
    
    public Task<ServiceResult<ClassesDetails>> CreateClassesDetailsAsync(ClassesDetails model)
    {
        throw new NotImplementedException();
    }
}
