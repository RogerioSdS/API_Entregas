using Api_Entregas.Services.Interfaces;
using Api_Entregas.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar HttpClient
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Serviços MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Opcional, se usar páginas Razor

// Registrar serviços personalizados da camada de serviço   
builder.Services.AddScoped<IAuthService, AuthService>();

// O HttpContextAccessor é um serviço que permite acessar o contexto HTTP da requisição
// em qualquer lugar da aplicação, sem precisar passar o contexto como parâmetro.
// Ele é necessário para que o serviço de sessão possa recuperar a sessão atual.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// O serviço de sessão é responsável por armazenar e recuperar os dados da sessão.
// Ele é implementado como um singleton, pois precisa ser acessado de qualquer lugar
// da aplicação, e precisa manter os dados da sessão entre as requisições.
builder.Services.AddScoped<ISessionService, SessionService>();

// ✅ Adicionar suporte a Session
builder.Services.AddDistributedMemoryCache(); // Armazena sessões na memória
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessão
    options.Cookie.HttpOnly = true; // Impede acesso via JS
    options.Cookie.IsEssential = true; // Necessário para funcionar mesmo com consentimento de cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Garante uso via HTTPS
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;
                return Task.CompletedTask;
            }
        };
    });


// Configurar CORS se necessário para chamadas AJAX
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5218/") // Ajuste para sua URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configurar logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// CORS (se configurado)
app.UseCors("AllowSpecificOrigin");

// ✅ Ativar sessão antes da autenticação/autorização
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Configurar rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Se estiver usando Razor Pages

app.Run();
