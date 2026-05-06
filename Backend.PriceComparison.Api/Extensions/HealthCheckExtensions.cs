using Microsoft.Extensions.Diagnostics.HealthChecks;
using Backend.PriceComparison.Api.HealthChecks;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Api.Extensions
{
    public static class HealthCheckExtensions
 {
     public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
      {
       services.AddHealthChecks()
         // Application basic health check
 .AddCheck<ApplicationHealthCheck>(
     "application",
      HealthStatus.Unhealthy,
      tags: ["ready", "live"])
   
   // Database connectivity check
      .AddCheck<DatabaseHealthCheck>(
  "database",
 HealthStatus.Unhealthy,
    tags: ["ready"])
       
 // Entity Framework specific health check
    .AddDbContextCheck<ClientDbContext>(
       "ef-database",
    HealthStatus.Unhealthy,
   tags: ["ready"]);

         return services;
      }

  public static WebApplication UseHealthCheckEndpoints(this WebApplication app)
   {
      // Detailed health check endpoint
       app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
      {
     Predicate = _ => true,
       ResponseWriter = HealthCheckResponseWriter.WriteResponse
  });

         // Ready endpoint (includes database checks)
  app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
   Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
   ResponseWriter = HealthCheckResponseWriter.WriteResponse
   });

      // Live endpoint (basic application checks only)
  app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
     Predicate = healthCheck => healthCheck.Tags.Contains("live"),
       ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

        return app;
        }
 }
}