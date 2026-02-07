namespace DistributedCachingDemo.Caching;

public static class CacheKeyBuilder
{
    public static string Product(Guid productId) =>
        $"product:{productId}";
}
