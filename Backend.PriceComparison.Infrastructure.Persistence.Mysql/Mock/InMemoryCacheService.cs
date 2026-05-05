using System.Collections.Concurrent;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;

public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object?> _cache = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var value) && value is T typed)
            return Task.FromResult<T?>(typed);

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        _cache[key] = value;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _cache.Keys.Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
        foreach (var key in keysToRemove)
            _cache.TryRemove(key, out _);

        return Task.CompletedTask;
    }
}
