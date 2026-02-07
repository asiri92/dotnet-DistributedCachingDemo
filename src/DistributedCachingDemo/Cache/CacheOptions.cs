namespace DistributedCachingDemo.Cache;

public sealed class CacheOptions
{
    public TimeSpan DefaultTtl { get; init; } = TimeSpan.FromMinutes(5);
}
