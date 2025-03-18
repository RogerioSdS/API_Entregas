using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Context;

public class ApiDbContext : IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }
    DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
        // Outras configurações, se necessário
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().Property(u => u.FirstName).HasMaxLength(50);
        modelBuilder.Entity<User>().Property(u => u.LastName).HasMaxLength(50);
    }
}