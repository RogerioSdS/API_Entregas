using System.Reflection;
using APIBackend.Application.Services.Interfaces;
using APIBackend.Application.Services;
using APIBackend.Domain.Identity;
using APIBackend.Repositories.Context;
using APIBackend.Repositories.Interfaces;
using APIBackend.Repositories.Services;
using APIBackend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using APIBackend.Application.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o AutoMapper
builder.Services.AddAutoMapper(typeof(ProfilesDTO).Assembly);// Registra todos os profiles no assembly

// Adicione serviços para controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

//Configuração do banco de dados
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3); // Tokens válidos por 3 horas
});

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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepo, UserRepoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepo, AuthRepoService>();
builder.Services.AddScoped<IStudentRepo, StudentRepoService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IClassDetailsService, ClassDetailsService>();
builder.Services.AddScoped<IClassDetailsRepo, ClassDetailsRepoService>();

// Configurar o NLog como provedor de logging
builder.Logging.ClearProviders(); // Remover provedores padrão (ex.: Console)
builder.Logging.SetMinimumLevel(LogLevel.Information); // Definir o nível mínimo de log
builder.Host.UseNLog(); // Usa NLog no lugar dos logs padrões

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

    // 🔒 Configuração do Bearer Token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo. Ex: **Bearer {seu_token}**"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
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

// Configuração do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Ex.: "sua-api"
        ValidAudience = builder.Configuration["Jwt:Audience"], // Ex.: "sua-api-cliente"
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("A chave de assinatura JWT não foi configurada."))) // Chave secreta
    };
});

//Criando a política de autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdmin", policy =>
        policy.RequireClaim("Department", "SystemAdmin")); //Preciso inserir essa claim no token para o usuário que for admin, ainda não apliquei essa politica
});

var app = builder.Build();

// Configure o pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1"));
    app.UseDeveloperExceptionPage();
}

// Insere o seeders com os dados se a flag --seed for passada --dotnet run --seed
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiDbContext>();
    await context.InitializeDatabaseAsync(); // Inicializar o banco de dados com scripts SQL    

    return; // Para evitar que o restante do pipeline seja executado    
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Adiciona autenticação
app.UseAuthorization(); // Adiciona autorização
app.MapControllers();

app.Run();
