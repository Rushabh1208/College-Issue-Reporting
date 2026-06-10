using backend.Infrastructure;
using backend.Infrastructure.Extensions;
using backend.Features.Auth;
using backend.Features.Issues;
using backend.Features.Users;
using backend.Features.Students;
using Serilog;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Render Port Binding
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// 🔹 Logging Configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 🔹 Add Services
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<AppDbContext>();

    int retries = 5;
    while (retries > 0)
    {
        try
        {
            logger.LogInformation("Attempting to run database migrations...");
            db.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");

            // 🔹 Seed Database within the same resilient scope
            logger.LogInformation("Seeding database...");
            app.SeedDatabase();
            logger.LogInformation("Database seeded successfully.");
            
            break;
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning($"Database not ready. Retrying in 5 seconds... ({retries} attempts left)");
            if (retries == 0)
            {
                logger.LogCritical(ex, "Could not connect to database after several attempts.");
                throw;
            }
            Thread.Sleep(5000);
        }
    }
}


// 🔹 Configure Middleware
app.UseSwaggerDocumentation();

app.UseCors("AllowAll");
app.UseRateLimiter();

app.UseSerilogRequestLogging();
app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// 🔹 Root Endpoint
app.MapGet("/", () => "Campus API is running and healthy! 🚀");

// 🔹 Map Feature Endpoints
app.MapAuthEndpoints();
app.MapIssueEndpoints();
app.MapUserEndpoints();
app.MapStudentEndpoints();

app.Run();
