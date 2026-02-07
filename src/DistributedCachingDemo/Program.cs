using DistributedCachingDemo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FakeProductRepository>();

var app = builder.Build();

app.MapGet("/", () => "Distributed Caching Demo – Phase 1");

app.Run();
