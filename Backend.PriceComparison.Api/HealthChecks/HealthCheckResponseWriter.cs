using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Backend.PriceComparison.Api.HealthChecks
{
    public static class HealthCheckResponseWriter
    {
     public static async Task WriteResponse(HttpContext context, HealthReport healthReport)
   {
   context.Response.ContentType = "application/json; charset=utf-8";

     var options = new JsonSerializerOptions
  {
     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      WriteIndented = true
 };

       var response = new
      {
    Status = healthReport.Status.ToString(),
   TotalDuration = healthReport.TotalDuration.TotalMilliseconds,
        Timestamp = DateTime.UtcNow,
      Entries = healthReport.Entries.Select(entry => new
     {
     Name = entry.Key,
        Status = entry.Value.Status.ToString(),
      Duration = entry.Value.Duration.TotalMilliseconds,
    Description = entry.Value.Description,
        Tags = entry.Value.Tags,
      Data = entry.Value.Data?.Count > 0 ? entry.Value.Data : null,
     Exception = entry.Value.Exception?.Message
      })
   };

   var json = JsonSerializer.Serialize(response, options);
   await context.Response.WriteAsync(json);
   }
    }
}