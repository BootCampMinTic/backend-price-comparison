using System.ComponentModel.DataAnnotations;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Configuration;

/// <summary>
/// Strongly-typed Redis configuration bound from <c>Redis</c> section in appsettings
/// (or the <c>REDIS_CONNECTION</c> environment variable for the connection string).
/// Redis is optional — when not configured or unreachable, in-memory cache is used.
/// </summary>
public class RedisSettings
{
    /// <summary>StackExchange.Redis connection string (host[:port][,options]). Optional — falls back to in-memory cache when empty.</summary>
    public string? ConnectionString { get; set; }

    /// <summary>Default TTL applied when a cache entry is stored without an explicit expiration.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Redis CacheExpirationMinutes must be a positive integer.")]
    public int CacheExpirationMinutes { get; set; } = 3600;
}
