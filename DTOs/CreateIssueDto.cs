namespace backend.DTOs
{
    public class CreateIssueDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Block { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;


    }
}
