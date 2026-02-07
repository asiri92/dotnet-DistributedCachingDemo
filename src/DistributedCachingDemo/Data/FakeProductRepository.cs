using DistributedCachingDemo.Models;

namespace DistributedCachingDemo.Data;

public sealed class FakeProductRepository
{
    private static readonly List<Product> _products =
    [
        new Product
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Mechanical Keyboard",
            Price = 149.99m
        },
        new Product
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Wireless Mouse",
            Price = 79.99m
        }
    ];

    public Task<Product?> GetByIdAsync(Guid id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }
}
