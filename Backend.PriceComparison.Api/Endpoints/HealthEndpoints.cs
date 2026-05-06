using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backend.PriceComparison.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthApiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/health")
            .WithTags("Health");

        group.MapGet("", GetHealthStatus)
            .WithName("GetHealthStatus")
            .WithSummary("Get application health status")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        group.MapGet("ready", GetReadyStatus)
            .WithName("GetReadyStatus")
            .WithSummary("Get ready status")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        group.MapGet("live", GetLiveStatus)
            .WithName("GetLiveStatus")
            .WithSummary("Get live status")
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetHealthStatus(HealthCheckService healthCheckService)
    {
        var result = await healthCheckService.CheckHealthAsync();

        var response = new
        {
            Status = result.Status.ToString(),
            result.TotalDuration,
            Entries = result.Entries.Select(e => new
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                e.Value.Duration,
                e.Value.Description,
                e.Value.Tags
            })
        };

        return result.Status == HealthStatus.Healthy
            ? TypedResults.Ok(response)
            : TypedResults.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<IResult> GetReadyStatus(HealthCheckService healthCheckService)
    {
        var result = await healthCheckService.CheckHealthAsync(healthCheck => healthCheck.Tags.Contains("ready"));

        return result.Status == HealthStatus.Healthy
            ? TypedResults.Ok(new { Status = "Ready", Timestamp = DateTime.UtcNow })
            : TypedResults.Json(new { Status = "Not Ready", Timestamp = DateTime.UtcNow }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private static IResult GetLiveStatus()
    {
        return TypedResults.Ok(new { Status = "Alive", Timestamp = DateTime.UtcNow });
    }
}
