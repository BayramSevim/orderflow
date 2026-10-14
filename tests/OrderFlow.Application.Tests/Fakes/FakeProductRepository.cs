using OrderFlow.Application.Products;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Tests.Fakes
{
    public class FakeProductRepository : IProductRepository
    {
        public List<Product> Products { get; } = new();

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
            => Task.FromResult(Products.FirstOrDefault(p => p.Id == id));

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct)
            => Task.FromResult(Products.Any(p => p.Code == code));

        public Task AddAsync(Product product, CancellationToken ct)
        {
            Products.Add(product);
            return Task.CompletedTask;
        }

        public Task<List<Product>> GetProductsAsync(CancellationToken ct)
            => Task.FromResult(Products.ToList());
    }
}