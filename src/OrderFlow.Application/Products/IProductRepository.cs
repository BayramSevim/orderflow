using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Products
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken);
    }
}