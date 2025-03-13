using System.Reflection;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using APIBackend.Repositories.Services;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços para controllers
builder.Services.AddControllers();

//Configuração do banco de dados
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); 
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireUppercase = true; // Pelo menos uma letra maiúscula
    options.Password.RequireDigit = true;     // Pelo menos um número
    options.Password.RequireNonAlphanumeric = true; // Pelo menos um caractere especial
    options.Password.RequiredLength = 8;      // Mínimo de 8 caracteres
    options.Password.RequiredUniqueChars = 1; // Pelo menos 1 caractere único
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApiDbContext>()
.AddDefaultTokenProviders();

// Adicionando a injeção de dependência
builder.Services.AddScoped<IUserServices, UserService>();
builder.Services.AddScoped<IUserRepo, UserRepoService>();

//Adicionando loggs na injecao de dependencia
// Configurar logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddFilter("APIBackend_Entregas", LogLevel.Information); // Garante que logs do namespace sejam exibidos a partir de Information
    logging.AddFilter("Microsoft", LogLevel.Warning); // Logs do Microsoft só a partir de Warning
    logging.AddFilter("System", LogLevel.Warning); // Logs do System só a partir de Warning
});

// Adicione serviços para Swagger
builder.Services.AddSwaggerGen(options =>
{
    // Configura as informações da API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minha API",
        Version = "v1",
        TermsOfService = new Uri("https://meu-dominio.com"),
        Description = "Bem-vindo à documentação da Minha API. Aqui você encontrará todos os serviços disponíveis para integrar nossa solução de forma simples e eficiente. Para utilizar a API, é necessário se registrar em nosso portal e obter uma chave de acesso. Acesse https://meu-portal.com e solicite sua chave na seção \"Configurações da Conta\".",
        Contact = new OpenApiContact
        {
            Name = "Suporte",
            Email = "suporte@meu-dominio.com"
        }
    });

    // Caminho dinâmico para o arquivo XML
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure o pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1"));
}

app.UseAuthorization(); // Opcional, só se for usar autenticação/autorização

app.MapControllers(); // Mapeia os endpoints dos controllers

app.Run();