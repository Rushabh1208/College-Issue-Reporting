using backend.Enums;

namespace backend.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; } 
    }
}

