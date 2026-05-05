using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Adapter.Cache;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Adapter;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Configuration;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Client.Repositories;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;
using StackExchange.Redis;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql;

public static class DependencyInjectionService
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var useMockInfrastructure = bool.TryParse(configuration["UseMockInfrastructure"], out var parsedUseMockInfrastructure)
            && parsedUseMockInfrastructure;
        if (useMockInfrastructure)
        {
            services.AddSingleton<ICacheService, InMemoryCacheService>();
            services.AddScoped<IClientRepository, MockClientRepository>();
            services.AddScoped<IDocumentTypeRepository, MockDocumentTypeRepository>();
            services.AddSingleton<IMessageProvider, MessageProvider>();

            return services;
        }

        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION") ?? configuration.GetConnectionString("MysqlConnection");
        services.AddDbContext<ClientDbContext>(
            options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
        ));

        var redisSettings = new RedisSettings
        {
            ConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
                ?? configuration["Redis:ConnectionString"]
                ?? string.Empty
        };

        if (int.TryParse(configuration["Redis:CacheExpirationMinutes"], out var cacheExpirationMinutes))
            redisSettings.CacheExpirationMinutes = cacheExpirationMinutes;

        services.AddSingleton(redisSettings);
        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(redisSettings.ConnectionString));
        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddSingleton<IMessageProvider, MessageProvider>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        
        return services;
    }
}
