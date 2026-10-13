using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Dealers;
using OrderFlow.Domain.Dealers;

namespace OrderFlow.Infrastructure.Persistence.Repositories
{
    public class DealerRepository : IDealerRepository
    {
        private readonly OrderFlowDbContext _context;

        public DealerRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public Task<Dealer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Dealers.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
        {
            return _context.Dealers.AnyAsync(d => d.Code == code, cancellationToken);
        }

        public async Task AddAsync(Dealer dealer, CancellationToken cancellationToken)
        {
            await _context.Dealers.AddAsync(dealer, cancellationToken);
        }
    }
}