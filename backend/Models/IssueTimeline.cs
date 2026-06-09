using backend.Enums;

namespace backend.Models
{
    public class IssueTimeline
    {
        public long Id { get; set; }

        public long IssueId { get; set; }
        public Issue Issue { get; set; } = null!;

        public TimelineAction Action { get; set; }

        public string PerformedBy { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
