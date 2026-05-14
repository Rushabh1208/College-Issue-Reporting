using Serilog;

namespace backend.Infrastructure.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Log.Information($"Request: {context.Request.Method} {context.Request.Path}");
            await _next(context);
        }
    }
}
