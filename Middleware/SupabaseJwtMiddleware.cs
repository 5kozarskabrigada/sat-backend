using System.Security.Claims;

namespace SAT.API.Middleware;

public class SupabaseJwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SupabaseJwtMiddleware> _logger;

    public SupabaseJwtMiddleware(RequestDelegate next, ILogger<SupabaseJwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var user = context.User;

        if (user?.Identity is { IsAuthenticated: true })
        {
            // Supabase typically puts the auth user id in "sub" and role in "role" (or inside "app_metadata").
            var roleClaim = user.FindFirst("role");
            var nameIdClaim = user.FindFirst("sub");

            var claims = new List<Claim>(user.Claims);

            // Mirror "role" -> ClaimTypes.Role so [Authorize(Roles = ...)] works
            if (roleClaim != null && !claims.Any(c => c.Type == ClaimTypes.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
            }

            // Mirror "sub" -> NameIdentifier for convenience in controllers
            if (nameIdClaim != null && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdClaim.Value));
            }

            if (claims.Count != user.Claims.Count())
            {
                var identity = new ClaimsIdentity(claims, user.Identity!.AuthenticationType, user.Identity.Name, ClaimTypes.Role);
                context.User = new ClaimsPrincipal(identity);

                _logger.LogDebug("Normalized Supabase JWT claims for user {Sub}", nameIdClaim?.Value);
            }
        }

        await _next(context);
    }
}
