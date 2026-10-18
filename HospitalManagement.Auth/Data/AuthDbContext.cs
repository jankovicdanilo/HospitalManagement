using HospitalManagement.Auth.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HospitalManagement.Auth.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ServiceToken> ServiceTokens => Set<ServiceToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var utcConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__User__3214EC076709E5A4");
                entity.ToTable("User");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime")
                    .HasConversion(utcConverter);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.PasswordHash).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(50).HasConversion<string>();
                entity.Property(e => e.Username).HasMaxLength(100);
            });

            modelBuilder.Entity<ServiceToken>(entity =>
            {
                entity.HasIndex(t => t.Jti).IsUnique();
                entity.Property(t => t.Jti).HasMaxLength(64);
                entity.Property(t => t.Name).HasMaxLength(50);
                entity.Property(t => t.Role).HasMaxLength(30);
                entity.Property(t => t.CreatedBy).HasMaxLength(100);
            });
        }
    }
}
