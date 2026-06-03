namespace backend.Infrastructure
{
    using backend.Models;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Issue> Issues => Set<Issue>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Issue>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<Issue>()
                .HasIndex(i => i.Status);

            modelBuilder.Entity<Issue>()
                .Property(i => i.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Issue>()
                .Property(i => i.ImageStorageProvider)
                .HasMaxLength(32);

            modelBuilder.Entity<Issue>()
                .Property(i => i.ImageMimeType)
                .HasMaxLength(100);
        }
    }
}
