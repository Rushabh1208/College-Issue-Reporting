using backend.Enums;

namespace backend.Models
{
    public class Issue
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public IssueStatus? Status { get; set; } = IssueStatus.Open;

        public long UserId { get; set; }      // Student who created
        public User User { get; set; } = null!;

        public long? AssignedToId { get; set; } // Staff
        public User AssignedTo { get; set; } = null!;


        
        public string Block { get; set; } = string.Empty;          // e.g., A, B, C
        public string RoomNumber { get; set; } = string.Empty;     // e.g., 101, 202

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImagePath { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}

