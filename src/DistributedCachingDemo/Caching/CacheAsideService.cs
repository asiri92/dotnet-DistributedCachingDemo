using DistributedCachingDemo.Cache;

namespace DistributedCachingDemo.Caching;

public sealed class CacheAsideService
{
    private readonly ICacheStore _cache;
    private readonly CacheOptions _options;
    private readonly ILogger<CacheAsideService> _logger;

    public CacheAsideService(
        ICacheStore cache,
        CacheOptions options,
        ILogger<CacheAsideService> logger)
    {
        _cache = cache;
        _options = options;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        Func<Task<T?>> factory)
    {
        var cached = await _cache.GetAsync<T>(key);

        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT for key {Key}", key);
            return cached.Value;
        }

        _logger.LogInformation("Cache MISS for key {Key}", key);

        var value = await factory();

        if (value is null)
            return default;

        var entry = new CacheEntry<T>(
            value,
            DateTime.UtcNow.Add(_options.DefaultTtl));

        await _cache.SetAsync(key, entry);

        return value;
    }

    public Task InvalidateAsync(string key)
    {
        _logger.LogInformation("Cache INVALIDATE for key {Key}", key);
        return _cache.RemoveAsync(key);
    }
}
