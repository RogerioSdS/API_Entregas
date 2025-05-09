using System;
using System.Reflection;
using APIBackend.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace APIBackend.Repositories;

public static class DbContextExtensions
{
    public static async Task InitializeDatabaseAsync(this ApiDbContext context, string sqlScriptsPath = null )
    {

        if (context == null) throw new ArgumentNullException(nameof(context));

        Logger _loggerNLog = LogManager.GetCurrentClassLogger();
        _loggerNLog.Info($"Inicializando o banco de dados...");

        // Garantir que o banco de dados foi criado
        await context.Database.EnsureCreatedAsync();

        // Definir o caminho padrão para os scripts SQL (pasta SqlScripts no projeto de infraestrutura)
        sqlScriptsPath ??= Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Scripts");

        if (!Directory.Exists(sqlScriptsPath))
        {
            throw new DirectoryNotFoundException($"A pasta de scripts SQL não foi encontrada: {sqlScriptsPath}");
        }

        // Executar todos os arquivos .sql na pasta
        foreach (var sqlFile in Directory.GetFiles(sqlScriptsPath, "*.sql"))
        {
            var sqlScript = await File.ReadAllTextAsync(sqlFile);
            await context.Database.ExecuteSqlRawAsync(sqlScript);
        }

        _loggerNLog.Info($"Finalizado o banco de dados...");
    }


}

