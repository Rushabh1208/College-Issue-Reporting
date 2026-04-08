namespace backend.DTOs
{
    public class CreateIssueDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Block { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;

        // Image support can be added back if needed, but keeping it consistent with current logic
        // public IFormFile? Image { get; set; }
    }
}
