using backend;
using backend.Extensions;
using Serilog;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Logging Configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 🔹 Add Services
builder.Services.AddAppServices();
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

app.Use(async (ctx, next) =>
{
    Log.Information($"Request: {ctx.Request.Method} {ctx.Request.Path}");
    await next();
});

// Custom Exception Handling Middleware
app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An unhandled exception occurred.");
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsync("Internal Server Error");
    }
});

app.UseAuthentication();
app.UseAuthorization();



// 🔹 Map Endpoints
app.MapAppEndpoints();

app.Run();
