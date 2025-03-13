using System;
using APIBackend.Domain;
using APIBackend.Domain.Identity;
using APIBackend.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Context;

public class ApiDbContext: IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
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