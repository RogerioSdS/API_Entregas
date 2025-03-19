using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Context
{
    public class ApiDbContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações existentes para User
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.FirstName).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.LastName).HasMaxLength(50);

            // Configurar o relacionamento muitos-para-muitos entre User e Role usando UserRole
            modelBuilder.Entity<UserRole>(entity =>
            {
                // Definir a chave primária composta (UserId e RoleId)
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                // Relacionamento com User
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Se o usuário for deletado, deleta os UserRoles associados

                // Relacionamento com Role
                entity.HasOne(ur => ur.Role)
                      .WithMany() // Role não tem uma coleção de UserRoles, então deixamos vazio
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade); // Se a role for deletada, deleta os UserRoles associados

                // Configurar a propriedade AssignmentDate, se necessário
                entity.Property(ur => ur.AssignmentDate)
                      .HasMaxLength(50); // Ajuste conforme necessário
            });

            // Configurações adicionais para Role, se necessário
            modelBuilder.Entity<Role>().Property(r => r.Description).HasMaxLength(200);
        }
    }
}