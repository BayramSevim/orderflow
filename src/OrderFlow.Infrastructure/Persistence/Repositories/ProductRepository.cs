using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Products;
using OrderFlow.Domain.Products;

namespace OrderFlow.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderFlowDbContext _context;

        public ProductRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Products.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
        {
            return _context.Products.AnyAsync(d => d.Code == code, cancellationToken);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            await _context.Products.AddAsync(product, cancellationToken);
        }

        public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken)
        {
            return await _context.Products.ToListAsync(cancellationToken);
        }
    }
}