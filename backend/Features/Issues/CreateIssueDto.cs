namespace backend.Features.Issues
{
    public class CreateIssueDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Block { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public backend.Enums.IssuePriority Priority { get; set; } = backend.Enums.IssuePriority.Medium;

        public bool IsAnonymous { get; set; } = false;    }
}
