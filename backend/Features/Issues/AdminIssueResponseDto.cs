using backend.Enums;

namespace backend.Features.Issues
{
    public class AdminIssueResponseDto : IssueResponseDto
    {
        public string ReporterName { get; set; } = string.Empty;
        public string ReporterStudentId { get; set; } = string.Empty;
    }
}
