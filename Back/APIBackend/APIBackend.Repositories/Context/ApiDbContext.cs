using System;
using APIBackend.Domain;
using APIBackend.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Context;

public class ApiDbContext: DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        // Defina suas tabelas como DbSet
        public DbSet<User> Users { get; set; } 
        public DbSet<Auth> Auth { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Defina suas configurações de entidade
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired(); // Exemplo de configuração de propriedade obrigatória
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired(); // Exemplo de configuração de propriedade obrigatória
        }
    }