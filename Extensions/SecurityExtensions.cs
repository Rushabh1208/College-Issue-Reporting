using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace backend.Extensions
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is missing from configuration.");
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
                options.AddPolicy("StudentOnly", p => p.RequireRole("Student"));
                options.AddPolicy("StaffOrAdmin", p => p.RequireRole("Admin", "Staff"));
            });

            return services;
        }
    }
}
