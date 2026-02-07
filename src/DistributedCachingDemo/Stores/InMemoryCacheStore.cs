using DistributedCachingDemo.Cache;

namespace DistributedCachingDemo.Stores;

public sealed class InMemoryCacheStore : ICacheStore
{
    private readonly Dictionary<string, object> _cache = new();
    private readonly object _lock = new();

    public Task<CacheEntry<T>?> GetAsync<T>(string key)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                var entry = (CacheEntry<T>)value;

                if (entry.IsExpired())
                {
                    _cache.Remove(key);
                    return Task.FromResult<CacheEntry<T>?>(null);
                }

                return Task.FromResult<CacheEntry<T>?>(entry);
            }

            return Task.FromResult<CacheEntry<T>?>(null);
        }
    }

    public Task SetAsync<T>(string key, CacheEntry<T> entry)
    {
        lock (_lock)
        {
            _cache[key] = entry;
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        lock (_lock)
        {
            _cache.Remove(key);
        }

        return Task.CompletedTask;
    }
}
