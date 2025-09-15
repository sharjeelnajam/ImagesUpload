using Microsoft.EntityFrameworkCore;
using ImageUploadTask.Models;

namespace ImageUploadTask.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerImage> CustomerImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure CustomerImage entity
            modelBuilder.Entity<CustomerImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).HasMaxLength(500); // Made optional since we're using Base64
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Base64Data).HasColumnType("NVARCHAR(MAX)"); // For large Base64 strings
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETUTCDATE()");

                // Configure relationship
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Images)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

        }
    }
}
