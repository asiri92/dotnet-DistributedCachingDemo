using System.Text.Json;
using DistributedCachingDemo.Cache;
using StackExchange.Redis;

namespace DistributedCachingDemo.Stores;

public sealed class RedisCacheStore : ICacheStore
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheStore> _logger;

    public RedisCacheStore(
        IConnectionMultiplexer connection,
        ILogger<RedisCacheStore> logger)
    {
        _database = connection.GetDatabase();
        _logger = logger;
    }

    public async Task<CacheEntry<T>?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (!value.HasValue)
                return null;

            var entry = JsonSerializer.Deserialize<CacheEntry<T>>(value!);

            if (entry is null || entry.IsExpired())
            {
                await _database.KeyDeleteAsync(key);
                return null;
            }

            return entry;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis GET failed for key {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, CacheEntry<T> entry)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(entry);

            var ttl = entry.ExpiresAt - DateTime.UtcNow;

            await _database.StringSetAsync(
                key,
                serialized,
                ttl > TimeSpan.Zero ? ttl : TimeSpan.FromSeconds(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis SET failed for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis REMOVE failed for key {Key}", key);
        }
    }
}
