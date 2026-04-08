namespace backend.DTOs
{
    public class IssueResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        // public string? ImageUrl { get; set; }
        public string Block { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;

        public string AssignedStaffName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
