using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;

namespace Backend.PriceComparison.Api.HealthChecks;

public sealed class ApplicationHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
          HealthCheckContext context,
         CancellationToken cancellationToken = default)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "Unknown";
            var buildDate = GetBuildDate(assembly);

            var data = new Dictionary<string, object>(capacity: 6)
            {
                { "Version", version },
                { "BuildDate", buildDate },
                { "Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown" },
                { "MachineName", Environment.MachineName },
                { "ProcessorCount", Environment.ProcessorCount },
                { "WorkingSet", Environment.WorkingSet }
            };

            return Task.FromResult(HealthCheckResult.Healthy("Application is running", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Application health check failed: {ex.Message}"));
        }
    }

    private static string GetBuildDate(Assembly assembly)
    {
        try
        {
            var location = assembly.Location;
            if (string.IsNullOrEmpty(location))
                return "Unknown";

            var buildDate = File.GetLastWriteTime(location);
            return buildDate.ToString("yyyy-MM-dd HH:mm:ss UTC");
        }
        catch
        {
            return "Unknown";
        }
    }
}