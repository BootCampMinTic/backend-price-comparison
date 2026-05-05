using Backend.PriceComparison.Api.Common.Configurations;
using Backend.PriceComparison.Application;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql;
using Backend.PriceComparison.Api.Configuration;
using Backend.PriceComparison.Api.Endpoints;
using Backend.PriceComparison.Api.Extensions;
using Backend.PriceComparison.Api.Middleware;
using Backend.PriceComparison.Api.Services;
using Backend.PriceComparison.Application.Common.Interfaces;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<RedisSettings>(config.GetSection("Redis"));

var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
    ?? config.GetSection("Redis:ConnectionString").Value;

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString!));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services
  .AddApplication()
    .AddPersistence(builder.Configuration);

builder.Services.AddCustomHealthChecks();

// Add global exception filter configuration
builder.Services.AddSingleton<GlobalExceptionConfiguration>();

builder.Services.AddRouting(routing => routing.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();

// Add OpenAPI
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BackendPriceComparison", policy =>
    {
        var allowedOrigins = config.GetSection("AllowedOrigins").Get<List<string>>();
        policy.WithOrigins(allowedOrigins.ToArray())
     .AllowAnyMethod()
       .AllowAnyHeader()
     .AllowCredentials();
    });
});

var app = builder.Build();

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
