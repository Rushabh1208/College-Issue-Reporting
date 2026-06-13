using backend.Enums;

namespace backend.Models
{
    public class Issue
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public IssueStatus? Status { get; set; } = IssueStatus.Open;



        public long? AssignedToId { get; set; }
        public User AssignedTo { get; set; } = null!;

        public string Block { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImagePath { get; set; }
        public string? ImageObjectKey { get; set; }
        public string? ImageStorageProvider { get; set; }
        public string? ImageMimeType { get; set; }
        public long? ImageSizeBytes { get; set; }

        public bool IsDeleted { get; set; } = false;

        // new properties
        public int? CategoryId { get; set; }
        public IssueCategory? Category { get; set; }

        public IssuePriority Priority { get; set; } = IssuePriority.Medium;

        public bool IsAnonymous { get; set; } = false;

        public int UpvoteCount { get; set; } = 0;

        public long? StudentId { get; set; }
        public Student? Student { get; set; }

        public ICollection<IssueUpvote> IssueUpvotes { get; set; } = new List<IssueUpvote>();
    }
}
