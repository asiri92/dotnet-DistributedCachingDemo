namespace DistributedCachingDemo.Cache;

public interface ICacheStore
{
    Task<CacheEntry<T>?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, CacheEntry<T> entry);
    Task RemoveAsync(string key);
}
