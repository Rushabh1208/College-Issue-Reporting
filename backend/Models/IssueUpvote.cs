namespace backend.Models
{
    public class IssueUpvote
    {
        public long Id { get; set; }

        public long IssueId { get; set; }
        public Issue Issue { get; set; } = null!;

        public long StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
