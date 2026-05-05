using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        services.AddOptions<RedisSettings>()
            .Bind(configuration.GetSection("Redis"))
            .PostConfigure(settings =>
            {
                var envConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
                if (!string.IsNullOrWhiteSpace(envConnection))
                    settings.ConnectionString = envConnection;
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            return ConnectionMultiplexer.Connect(settings.ConnectionString);
        });
        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddSingleton<IMessageProvider, MessageProvider>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();

        return services;
    }
}
