using backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace backend.Common
{
    public static class SecurityHelper
    {
        private const string DefaultKey = "THIS_IS_SUPER_SECRET_KEY_1236547890";

        public static string GenerateToken(User user, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? DefaultKey;
            
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()) 
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
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static long GetUserId(HttpContext ctx, AppDbContext db)
        {
            var email = ctx.User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                throw new Exception("User email missing from token");

            var user = db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                throw new Exception($"User not found in DB: {email}");

            return user.Id;
        }

        public static string GetUserRole(HttpContext ctx)
        {
            return ctx.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}
