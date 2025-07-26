using APIBackend.Domain.Enum;
using APIBackend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIBackend.Repositories.Context
{
    public class ApiDbContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        // DbSets adicionais
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ClassDetails> ClassDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── Configurações para User ──────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(c => c.Created)
                      .HasColumnType("date");

                entity.Property(c => c.Modified)
                      .HasColumnType("date");

                entity.Property(u => u.Email)
                      .IsRequired();

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.FirstName)
                      .HasMaxLength(50);

                entity.Property(u => u.LastName)
                      .HasMaxLength(50);
            });

            // ─── Configurações para UserRole (many-to-many User↔Role) ─────────────
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne<User>()
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Role>()
                      .WithMany()
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ur => ur.AssignmentDate)
                      .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%S', 'now')");
            });

            // ─── Configurações para Role ─────────────────────────────────────────
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.Description)
                      .HasMaxLength(200);

                entity.Property(r => r.IsActive)
                      .HasDefaultValue(true);
            });

            // ─── Configurações para RefreshToken ─────────────────────────────────
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Id)
                      .ValueGeneratedOnAdd();

                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshToken)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rt => rt.CreatedAt)
                      .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%S', 'now')");
            });

            // ─── Configurações para Student (N-N com User) ────────────────────────
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName)
                      .IsRequired();

                entity.Property(e => e.LastName)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .IsRequired();

                entity.Property(e => e.PhoneNumber)
                      .IsRequired();

                // N-N entre Student.Responsibles e User.Students
                entity.HasMany(s => s.Responsibles)
                      .WithMany(u => u.Students)
                      .UsingEntity<Dictionary<string, object>>(
                          "StudentResponsible",
                          join => join
                              .HasOne<User>()
                              .WithMany()
                              .HasForeignKey("ResponsibleId")
                              .OnDelete(DeleteBehavior.Restrict),
                          join => join
                              .HasOne<Student>()
                              .WithMany()
                              .HasForeignKey("StudentId")
                              .OnDelete(DeleteBehavior.Cascade),
                          join =>
                          {
                              join.HasKey("StudentId", "ResponsibleId");
                              join.ToTable("StudentResponsible");
                          });
            });

            // ─── Configurações para ClassDetails (1-N com Student) ───────────────
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
}
