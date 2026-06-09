namespace backend.Infrastructure
{
    using backend.Models;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Issue> Issues => Set<Issue>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<IssueCategory> IssueCategories => Set<IssueCategory>();
        public DbSet<IssueTimeline> IssueTimelines => Set<IssueTimeline>();
        public DbSet<IssueUpvote> IssueUpvotes => Set<IssueUpvote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // existing Issue config — do not change
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

            // existing Student config — do not change
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentId)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .Property(s => s.Gender)
                .HasConversion<string>();

            modelBuilder.Entity<Student>()
                .Property(s => s.StudentId)
                .HasMaxLength(20);

            modelBuilder.Entity<Student>()
                .Property(s => s.Email)
                .HasMaxLength(100);

            modelBuilder.Entity<Student>()
                .Property(s => s.Name)
                .HasMaxLength(100);

            // new IssueCategory config
            modelBuilder.Entity<IssueCategory>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<IssueCategory>()
                .Property(c => c.ParentCategory)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<IssueCategory>()
                .HasIndex(c => c.ParentCategory);

            // seed all categories
            modelBuilder.Entity<IssueCategory>().HasData(
                // Infrastructure
                new IssueCategory { Id = 1, Name = "Electrical", ParentCategory = Enums.ParentCategory.Infrastructure, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 2, Name = "Plumbing", ParentCategory = Enums.ParentCategory.Infrastructure, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 3, Name = "Internet", ParentCategory = Enums.ParentCategory.Infrastructure, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 4, Name = "Furniture", ParentCategory = Enums.ParentCategory.Infrastructure, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 5, Name = "Cleaning", ParentCategory = Enums.ParentCategory.Infrastructure, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 6, Name = "Security", ParentCategory = Enums.ParentCategory.Infrastructure, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 7, Name = "Water Supply", ParentCategory = Enums.ParentCategory.Infrastructure, IsWomenWelfare = false, IsActive = true },

                // Hostel
                new IssueCategory { Id = 8, Name = "Room Maintenance", ParentCategory = Enums.ParentCategory.Hostel, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 9, Name = "Cleanliness", ParentCategory = Enums.ParentCategory.Hostel, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 10, Name = "Hostel Facilities", ParentCategory = Enums.ParentCategory.Hostel, IsWomenWelfare = false, IsActive = true },

                // Academic
                new IssueCategory { Id = 11, Name = "Classroom Issue", ParentCategory = Enums.ParentCategory.Academic, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 12, Name = "Faculty Issue", ParentCategory = Enums.ParentCategory.Academic, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 13, Name = "Academic Facility Issue", ParentCategory = Enums.ParentCategory.Academic, IsWomenWelfare = false, IsActive = true },

                // Campus Safety
                new IssueCategory { Id = 14, Name = "Security Concern", ParentCategory = Enums.ParentCategory.CampusSafety, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 15, Name = "Unsafe Area", ParentCategory = Enums.ParentCategory.CampusSafety, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 16, Name = "Broken CCTV", ParentCategory = Enums.ParentCategory.CampusSafety, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 17, Name = "Poor Lighting", ParentCategory = Enums.ParentCategory.CampusSafety, IsWomenWelfare = false, IsActive = true },

                // Student Welfare
                new IssueCategory { Id = 18, Name = "Ragging", ParentCategory = Enums.ParentCategory.StudentWelfare, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 19, Name = "Bullying", ParentCategory = Enums.ParentCategory.StudentWelfare, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 20, Name = "Discrimination", ParentCategory = Enums.ParentCategory.StudentWelfare, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 21, Name = "Mental Wellbeing", ParentCategory = Enums.ParentCategory.StudentWelfare, IsWomenWelfare = false, IsActive = true },
                new IssueCategory { Id = 22, Name = "Counseling Request", ParentCategory = Enums.ParentCategory.StudentWelfare, IsWomenWelfare = false, IsActive = true },

                // Women Welfare
                new IssueCategory { Id = 23, Name = "Harassment", ParentCategory = Enums.ParentCategory.WomenWelfare, IsWomenWelfare = true, IsActive = true },
                new IssueCategory { Id = 24, Name = "Safety & Privacy Concern", ParentCategory = Enums.ParentCategory.WomenWelfare, IsWomenWelfare = true, IsActive = true },
                new IssueCategory { Id = 25, Name = "Complaint Against Student", ParentCategory = Enums.ParentCategory.WomenWelfare, IsWomenWelfare = true, IsActive = true },
                new IssueCategory { Id = 26, Name = "Complaint Against Faculty", ParentCategory = Enums.ParentCategory.WomenWelfare, IsWomenWelfare = true, IsActive = true }
            );

            // new Issue additions
            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Category)
                .WithMany()
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Issue>()
                .Property(i => i.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Issue>()
                .HasIndex(i => i.Priority);

            modelBuilder.Entity<Issue>()
                .Property(i => i.UpvoteCount)
                .HasDefaultValue(0);

            // IssueTimeline config
            modelBuilder.Entity<IssueTimeline>()
                .HasOne(t => t.Issue)
                .WithMany()
                .HasForeignKey(t => t.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueTimeline>()
                .Property(t => t.Action)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<IssueTimeline>()
                .Property(t => t.PerformedBy)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<IssueTimeline>()
                .Property(t => t.Notes)
                .HasMaxLength(500);

            modelBuilder.Entity<IssueTimeline>()
                .HasIndex(t => t.IssueId);

            modelBuilder.Entity<IssueTimeline>()
                .HasIndex(t => t.CreatedAt);

            // IssueUpvote config
            modelBuilder.Entity<IssueUpvote>()
                .HasOne(u => u.Issue)
                .WithMany()
                .HasForeignKey(u => u.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueUpvote>()
                .HasOne(u => u.Student)
                .WithMany()
                .HasForeignKey(u => u.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // one student can only upvote an issue once
            modelBuilder.Entity<IssueUpvote>()
                .HasIndex(u => new { u.IssueId, u.StudentId })
                .IsUnique();
        }
    }
}
