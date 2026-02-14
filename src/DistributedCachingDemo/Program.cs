using DistributedCachingDemo.Cache;
using DistributedCachingDemo.Caching;
using DistributedCachingDemo.Data;
using DistributedCachingDemo.Stores;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FakeProductRepository>();
//builder.Services.AddSingleton<ICacheStore, InMemoryCacheStore>();
builder.Services.AddSingleton(new CacheOptions
{
    DefaultTtl = TimeSpan.FromSeconds(30)
});

// For Redis, ensure you have a Redis server running locally on the default port (6379).
builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddSingleton<ICacheStore, RedisCacheStore>();

builder.Services.AddSingleton<CacheAsideService>();

builder.Services.AddLogging();

var app = builder.Build();

app.MapGet("/", () => "Distributed Caching Demo – Phase 2");

app.MapGet("/products/{id:guid}", async (
    Guid id,
    CacheAsideService cache,
    FakeProductRepository repo) =>
{
    var cacheKey = CacheKeyBuilder.Product(id);

    var product = await cache.GetAsync(
        cacheKey,
        () => repo.GetByIdAsync(id));

    return product is not null
        ? Results.Ok(product)
        : Results.NotFound();
});

app.MapPut("/products/{id:guid}", async (
    Guid id,
    string name,
    decimal price,
    CacheAsideService cache,
    FakeProductRepository repo) =>
{
    var updated = await repo.UpdateAsync(id, name, price);

    if (updated is null)
        return Results.NotFound();

    var cacheKey = CacheKeyBuilder.Product(id);

    await cache.InvalidateAsync(cacheKey);

    return Results.Ok(updated);
});

app.MapDelete("/products/{id:guid}", async (
    Guid id,
    CacheAsideService cache,
    FakeProductRepository repo) =>
{
    var deleted = await repo.DeleteAsync(id);

    if (!deleted)
        return Results.NotFound();

    var cacheKey = CacheKeyBuilder.Product(id);

    await cache.InvalidateAsync(cacheKey);

    return Results.NoContent();
});



app.Run();
