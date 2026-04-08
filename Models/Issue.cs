using backend.Enums;

namespace backend.Models
{
    public class Issue
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public IssueStatus? Status { get; set; } = IssueStatus.Open;

        public long UserId { get; set; }      // Student who created
        public User User { get; set; }

        public long? AssignedToId { get; set; } // Staff
        public User AssignedTo { get; set; }

        public string? ImagePath { get; set; }

        
        public string Block { get; set; }          // e.g., A, B, C
        public string RoomNumber { get; set; }     // e.g., 101, 202

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}

