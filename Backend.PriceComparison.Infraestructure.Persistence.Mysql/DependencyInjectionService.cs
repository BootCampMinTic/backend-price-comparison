using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Adapter.Cache;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Adapter;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Configuration;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Client.DomainServices;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Context;
using StackExchange.Redis;

namespace Backend.PriceComparison.Infraestructure.Persistence.Mysql;

public static class DependencyInjectionService
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
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

        services.AddTransient<IMessageProvider, MessageProvider>();
        services.AddScoped<IClientDomainService, ClientDomainService>();
        services.AddScoped<IDocumentTypeDomainService, DocumentTypeDomainService>();
        
        return services;
    }
}
