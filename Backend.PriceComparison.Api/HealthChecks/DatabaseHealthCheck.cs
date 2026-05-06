using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backend.PriceComparison.Api.HealthChecks
{
    public class DatabaseHealthCheck(IServiceProvider serviceProvider) : IHealthCheck
    {
     private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<HealthCheckResult> CheckHealthAsync(
  HealthCheckContext context,
            CancellationToken cancellationToken = default)
    {
     try
            {
     using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context.ClientDbContext>();
      
          if (dbContext == null)
               {
              return HealthCheckResult.Unhealthy("Database context not configured");
           }

         // Simple database connectivity check
         var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
    
       if (canConnect)
      {
            return HealthCheckResult.Healthy("Database connection is healthy");
         }
    else
             {
          return HealthCheckResult.Unhealthy("Cannot connect to database");
        }
     }
            catch (Exception ex)
         {
              return HealthCheckResult.Unhealthy($"Database health check failed: {ex.Message}");
            }
        }
    }
}