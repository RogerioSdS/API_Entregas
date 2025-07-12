using Api_Entregas.Services.Interfaces;
using Api_Entregas.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Configurar HttpClient
builder.Services.AddHttpClient();

// Serviços MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Opcional, se usar páginas Razor

// Registrar serviços personalizados da camada de serviço
builder.Services.AddScoped<IAuthService, AuthService>();

// ✅ Adicionar suporte a Session
builder.Services.AddDistributedMemoryCache(); // Armazena sessões na memória
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessão
    options.Cookie.HttpOnly = true; // Impede acesso via JS
    options.Cookie.IsEssential = true; // Necessário para funcionar mesmo com consentimento de cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Garante uso via HTTPS
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
