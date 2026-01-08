namespace SAT.API.Middleware;

public class JwtValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtValidationMiddleware> _logger;

    public JwtValidationMiddleware(RequestDelegate next, ILogger<JwtValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Let the built-in JwtBearer handler run first (UseAuthentication before this middleware)

        if (context.User?.Identity is { IsAuthenticated: false })
        {
            // No token or invalid token on a protected endpoint
            if (IsApiEndpoint(context.Request.Path))
            {
                _logger.LogWarning("Unauthenticated request to {Path}", context.Request.Path);
            }
        }

        await _next(context);
    }

    private static bool IsApiEndpoint(PathString path)
    {
        return path.HasValue && path.Value!.StartsWith("/api", StringComparison.OrdinalIgnoreCase);
    }
}
