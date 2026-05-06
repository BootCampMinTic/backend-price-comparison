namespace Backend.PriceComparison.Api.Middleware;

public sealed class BearerTokenMiddleware(
    RequestDelegate next,
    ILogger<BearerTokenMiddleware> logger,
    IWebHostEnvironment environment)
{
    private static readonly string[] PublicEndpoints =
    [
        "/api/v1/health",
        "/health",
        "/openapi",
        "/scalar",
        "/index.html",
        "/_framework",
        "/_vs",
        "/css",
        "/js"
    ];

    private static readonly string[] DevOnlyEndpoints =
    [
        "/api/v1/dev",
        "/"
    ];

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (IsPublicEndpoint(path))
        {
            await next(context);
            return;
        }

        var token = ExtractToken(context);

        if (string.IsNullOrEmpty(token))
        {
            logger.LogWarning("Missing authorization token for path: {Path}", path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Authorization token is required",
                data = (object?)null
            });
            return;
        }

        await next(context);
    }

    private bool IsPublicEndpoint(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        foreach (var endpoint in PublicEndpoints)
        {
            if (path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        if (environment.IsDevelopment())
        {
            foreach (var endpoint in DevOnlyEndpoints)
            {
                if (path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return false;
    }

    private static string? ExtractToken(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader))
            return null;

        return authHeader["Bearer ".Length..].Trim();
    }
}
