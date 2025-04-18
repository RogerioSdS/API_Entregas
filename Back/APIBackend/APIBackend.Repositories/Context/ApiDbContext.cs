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

    public DbSet<User> Users { get; set; } //não seria necessário utiliza-lo, pois o IdentityDbContext já possui um DbSet de User, e esta sendo passado como parametro para o IdentityDbContext
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações para User
        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().Property(u => u.FirstName).HasMaxLength(50);
        modelBuilder.Entity<User>().Property(u => u.LastName).HasMaxLength(50);

        // Configurar o relacionamento muitos-para-muitos entre User e Role usando UserRole
        modelBuilder.Entity<UserRole>(entity =>
        {
            // Chave primária composta
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Relacionamento com User (sem propriedade de navegação)
            entity.HasOne<User>()
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com Role (sem propriedade de navegação)
            entity.HasOne<Role>()
                  .WithMany()
                  .HasForeignKey(ur => ur.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Configurar AssignmentDate com valor padrão dinâmico
            entity.Property(ur => ur.AssignmentDate)
                  .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%S', 'now')"); // SQL Server, ou NOW() para outros bancos
        });

        // Configurações para Role
        modelBuilder.Entity<Role>(
            entity =>
            {
                entity.Property(r => r.Description).HasMaxLength(200);
                entity.Property(r => r.IsActive).HasDefaultValue(true);
            });

        modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.UserId);
                entity.HasOne(rt => rt.User)
                    .WithMany() //não tem propriedade de navegação em User -- public ICollection<RefreshToken> RefreshTokens { get; set; }

                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.Property(rt => rt.CreatedAt)
                    .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%S', 'now')"); 
            });
    }
}