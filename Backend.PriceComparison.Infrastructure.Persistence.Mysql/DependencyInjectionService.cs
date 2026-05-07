using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Adapter.Cache;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Adapter;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Configuration;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Client.Repositories;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;
using StackExchange.Redis;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql;

public static class DependencyInjectionService
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION") ?? configuration.GetConnectionString("MysqlConnection");
        services.AddDbContext<ClientDbContext>(
            options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(maxRetryCount: 3)));

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

        services.AddSingleton<ICacheService>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(RedisCacheService).FullName!);

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                logger.LogInformation("No Redis connection string configured. Using in-memory cache.");
                return new InMemoryCacheService();
            }

            var config = $"{settings.ConnectionString},abortConnect={settings.AbortConnect.ToString().ToLower()},syncTimeout={settings.SyncTimeoutMs},connectTimeout={settings.ConnectTimeoutMs}";
            try
            {
                var multiplexer = ConnectionMultiplexer.Connect(config);
                var server = multiplexer.GetServer(multiplexer.GetEndPoints()[0]);
                server.Ping();
                logger.LogInformation("Redis connected successfully");
                return new RedisCacheService(multiplexer, sp.GetRequiredService<IOptions<RedisSettings>>(), loggerFactory.CreateLogger<RedisCacheService>());
            }
            catch
            {
                logger.LogWarning("Redis connection failed. Falling back to in-memory cache.");
                return new InMemoryCacheService();
            }
        });

        services.AddSingleton<IMessageProvider, MessageProvider>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();

        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<ITypeUserRepository, TypeUserRepository>();
        services.AddScoped<ICategoryProductRepository, CategoryProductRepository>();
        services.AddScoped<ICategoryStoreRepository, CategoryStoreRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IProductSaleRepository, ProductSaleRepository>();

        return services;
    }
}
