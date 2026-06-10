using backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace backend.Infrastructure
{
    public static class SecurityHelper
    {
        public static string GenerateToken(User user, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is missing from configuration.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("userType", "staff")
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateToken(Student student, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is missing from configuration.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim(ClaimTypes.Name, student.Email),
                new Claim(ClaimTypes.Role, "Student"),
                new Claim("userType", "student"),
                new Claim("studentId", student.StudentId),
                new Claim("gender", student.Gender.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(hash)) return false;
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public static long GetUserId(HttpContext ctx)
        {
            var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var claimUserId))
            {
                return claimUserId;
            }

            throw new UnauthorizedAccessException("User identification missing from token");
        }

        public static string GetUserType(HttpContext ctx)
        {
            return ctx.User.FindFirst("userType")?.Value ?? "staff";
        }

        public static string GetStudentId(HttpContext ctx)
        {
            return ctx.User.FindFirst("studentId")?.Value ?? string.Empty;
        }
    }
}
