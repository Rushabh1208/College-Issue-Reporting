namespace backend
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
        }
    }
}
