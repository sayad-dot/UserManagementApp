using Microsoft.EntityFrameworkCore;
using UserManagementApp.Core.Entities;

namespace UserManagementApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create unique index for email (FIRST REQUIREMENT)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.RegistrationTime).IsRequired();
                entity.Property(u => u.Status).IsRequired();
                
                // Remove the explicit column type configurations
                // Let Entity Framework handle the timestamp types automatically
            });
        }
    }
}