using backend.Models;
using backend.Enums;
using backend.Common;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace backend.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 33))
                ));

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnection = configuration["Redis:ConnectionString"] ?? "localhost:6379";
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
            return services;
        }

        public static void SeedDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!db.Users.Any(u => u.Role == UserRole.Admin))
            {
                db.Users.Add(new User
                {
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = SecurityHelper.HashPassword("admin123"),
                    Role = UserRole.Admin
                });
                db.SaveChanges();
            }

            if (!db.Users.Any(u => u.Role == UserRole.Staff))
            {
                db.Users.Add(new User
                {
                    Name = "Staff1",
                    Email = "staff1@gmail.com",
                    PasswordHash = SecurityHelper.HashPassword("staff@123"),
                    Role = UserRole.Staff
                });

                db.Users.Add(new User
                {
                    Name = "Staff2",
                    Email = "staff2@gmail.com",
                    PasswordHash = SecurityHelper.HashPassword("staff@123"),
                    Role = UserRole.Staff
                });
                db.SaveChanges();
            }
        }
    }
}
