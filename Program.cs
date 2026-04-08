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
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();



// 🔹 Configure Middleware



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
