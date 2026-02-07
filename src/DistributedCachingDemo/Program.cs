using DistributedCachingDemo.Cache;
using DistributedCachingDemo.Caching;
using DistributedCachingDemo.Data;
using DistributedCachingDemo.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FakeProductRepository>();
builder.Services.AddSingleton<ICacheStore, InMemoryCacheStore>();
builder.Services.AddSingleton(new CacheOptions
{
    DefaultTtl = TimeSpan.FromSeconds(30)
});
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


app.Run();
