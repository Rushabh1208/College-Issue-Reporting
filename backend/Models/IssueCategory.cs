using backend.Enums;

namespace backend.Models
{
    public class IssueCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ParentCategory ParentCategory { get; set; }
        public bool IsWomenWelfare { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
