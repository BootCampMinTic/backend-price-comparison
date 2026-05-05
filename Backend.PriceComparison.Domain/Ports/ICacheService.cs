namespace Backend.PriceComparison.Domain.Ports;

/// <summary>
/// Abstraction over the distributed cache used by query handlers.
/// Implementations are expected to serialize <typeparamref name="T"/> as JSON.
/// </summary>
public interface ICacheService
{
    /// <summary>Reads a cached value by its key.</summary>
    /// <typeparam name="T">Type to deserialize the cached entry into.</typeparam>
    /// <param name="key">Full cache key.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The cached value, or <c>null</c> when the key is missing or expired.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Stores a value under the given key.</summary>
    /// <typeparam name="T">Type of the value to cache.</typeparam>
    /// <param name="key">Full cache key.</param>
    /// <param name="value">Value to persist.</param>
    /// <param name="expiration">Optional TTL. When omitted, the implementation default applies.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>Deletes a single cache entry.</summary>
    /// <param name="key">Full cache key to evict.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Deletes every cache entry whose key starts with the given prefix.</summary>
    /// <param name="prefix">Common key prefix (for example <c>clients:natural</c>).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}
