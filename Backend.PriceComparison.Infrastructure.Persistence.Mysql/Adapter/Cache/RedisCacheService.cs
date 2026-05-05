using System.Text.Json;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Adapter.Cache;

/// <summary>Redis-backed implementation of <see cref="ICacheService"/>.</summary>
public class RedisCacheService(
    IConnectionMultiplexer connectionMultiplexer,
    IOptions<RedisSettings> options,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    private readonly int _defaultExpirationMinutes = options.Value.CacheExpirationMinutes;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        if (!value.HasValue)
        {
            logger.LogDebug("Cache miss for key {CacheKey}", key);
            return default;
        }

        logger.LogDebug("Cache hit for key {CacheKey}", key);
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        var expirationTime = expiration ?? TimeSpan.FromMinutes(_defaultExpirationMinutes);
        await _database.StringSetAsync(key, serializedValue, expirationTime);
        logger.LogDebug("Cache set for key {CacheKey} with TTL {TtlMinutes} minute(s)", key, expirationTime.TotalMinutes);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
        logger.LogDebug("Cache evicted key {CacheKey}", key);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var endpoints = connectionMultiplexer.GetEndPoints();
        var deleted = 0;
        foreach (var endpoint in endpoints)
        {
            var server = connectionMultiplexer.GetServer(endpoint);
            var keys = server.Keys(pattern: $"{prefix}*");
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
                deleted++;
            }
        }
        logger.LogDebug("Cache evicted {Count} key(s) with prefix {Prefix}", deleted, prefix);
    }
}
