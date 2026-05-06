using Backend.PriceComparison.Application;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql;
using Backend.PriceComparison.Api.Endpoints;
using Backend.PriceComparison.Api.Extensions;
using Backend.PriceComparison.Api.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services
  .AddApplication()
    .AddPersistence(builder.Configuration);

builder.Services.AddCustomHealthChecks();

builder.Services.AddRouting(routing => routing.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();

// Add OpenAPI
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BackendPriceComparison", policy =>
    {
        var allowedOrigins = config.GetSection("AllowedOrigins").Get<List<string>>() ?? ["*"];
        policy.WithOrigins([.. allowedOrigins])
     .AllowAnyMethod()
       .AllowAnyHeader()
     .AllowCredentials();
    });
});

var app = builder.Build();

// Custom exception handling middleware
app.UseCustomExceptionMiddleware();

app.UseCors("BackendPriceComparison");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Map OpenAPI endpoint
app.MapOpenApi();

// Add Scalar UI - Configured to be the default documentation page
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("backend-price-comparison API")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .WithPreferredScheme("Bearer")
        .WithApiKeyAuthentication(x => x.Token = "");
});

// Health checks
app.UseHealthCheckEndpoints();

// Custom authentication middleware
app.UseMiddleware<BearerTokenMiddleware>();

// Map Minimal API endpoints
app.MapClientEndpoints();
app.MapDocumentTypeEndpoints();
app.MapHealthApiEndpoints();

app.Run();

public partial class Program { }
