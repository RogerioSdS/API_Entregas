using APIBackend.Domain.Enum;
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
    public DbSet<Student> Students { get; set; }
    public DbSet<ClassDetails> ClassDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações para User
        modelBuilder.Entity<User>(
            entity =>
            {

                entity.Property(c => c.Created)
                      .HasColumnType("date");

                entity.Property(c => c.Modified)
                      .HasColumnType("date");

                entity.Property(u => u.Email).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).HasMaxLength(50);
                entity.Property(u => u.LastName).HasMaxLength(50);
            });

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

        modelBuilder.Entity<RefreshToken>(
            entity =>
            {
                entity.Property(rt => rt.Id).ValueGeneratedOnAdd();  // Gerar o Id do token automaticamente
                entity.HasKey(rt => rt.Id);  // Chave primária do token é o Id

                // Relacionamento entre RefreshToken e User
                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshToken)  // User pode ter vários RefreshTokens
                    .HasForeignKey(rt => rt.UserId)  // Use UserId como chave estrangeira
                    .OnDelete(DeleteBehavior.Cascade);  // Ao excluir um usuário, exclua seus tokens de atualização

                entity.Property(rt => rt.CreatedAt)
                    .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%S', 'now')");
            });

        modelBuilder.Entity<Student>(entity =>
  {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).ValueGeneratedOnAdd(); // 👈 gera o Id automaticamente

      entity.Property(e => e.FirstName).IsRequired();
      entity.Property(e => e.LastName).IsRequired();
      entity.Property(e => e.Email).IsRequired();
      entity.Property(e => e.PhoneNumber).IsRequired();

      entity.HasOne(e => e.Responsible)
            .WithMany(u => u.Students)
            .HasForeignKey(e => e.ResponsibleId)
            .OnDelete(DeleteBehavior.Restrict);
  });

        modelBuilder.Entity<ClassDetails>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Classes)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(c => c.DateOfClass)
                  .HasColumnType("date");
        });
    }
}