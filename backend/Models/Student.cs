using backend.Enums;

namespace backend.Models
{
    public class Student
    {
        public long Id { get; set; }
        public string StudentId { get; set; } = string.Empty;  // e.g. "2025001"
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}