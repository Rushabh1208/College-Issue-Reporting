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
        public DateTime CreatedAt { get; set; }
    }
}
