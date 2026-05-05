namespace Backend.PriceComparison.Api.Middleware;

public class BearerTokenMiddleware(
    RequestDelegate next,
    ILogger<BearerTokenMiddleware> logger,
    IWebHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

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

        var publicEndpoints = new List<string>
        {
            "/api/v1/health",
            "/health",
            "/openapi",
            "/scalar",
            "/index.html",
            "/_framework",
            "/_vs",
            "/css",
            "/js"
        };

        if (environment.IsDevelopment())
        {
            publicEndpoints.Add("/api/v1/dev");
            publicEndpoints.Add("/"); // Allow root in development
        }

        return publicEndpoints.Any(endpoint => path.StartsWith(endpoint));
    }

    private static string? ExtractToken(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader))
            return null;

        return authHeader["Bearer ".Length..].Trim();
    }
}
