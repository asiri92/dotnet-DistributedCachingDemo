namespace DistributedCachingDemo.Cache;

public sealed class CacheEntry<T>
{
    public T Value { get; }
    public DateTime ExpiresAt { get; }

    public CacheEntry(T value, DateTime expiresAt)
    {
        Value = value;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired() =>
        DateTime.UtcNow >= ExpiresAt;
}
