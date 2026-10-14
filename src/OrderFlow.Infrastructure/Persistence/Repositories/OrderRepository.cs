using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Dealers;
using OrderFlow.Application.Orders;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderFlowDbContext _context;

        public OrderRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        public async Task<List<Order>> GetByDealerIdAsync(Guid dealerId, CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(o => o.Items).Where(x => x.DealerId == dealerId).ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(o => o.Items).ToListAsync(cancellationToken);
        }
    }
}