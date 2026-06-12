using backend.Enums;

namespace backend.Features.Issues
{
    public class IssueResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IssueStatus Status { get; set; }
        public string Block { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;

        public string AssignedStaffName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ImageObjectKey { get; set; }
        public string? ImageStorageProvider { get; set; }
        public string? ImageMimeType { get; set; }
        public long? ImageSizeBytes { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public IssuePriority Priority { get; set; }
        public bool IsAnonymous { get; set; }
        public int UpvoteCount { get; set; }
    }
}
