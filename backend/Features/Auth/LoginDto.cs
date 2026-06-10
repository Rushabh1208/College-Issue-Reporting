namespace backend.Features.Auth
{
    public class LoginDto
    {
        // accepts StudentId (e.g. "2025001") or email address
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
