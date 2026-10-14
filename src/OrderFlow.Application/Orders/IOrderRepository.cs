using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken);
        Task<List<Order>> GetByDealerIdAsync(Guid dealerId, CancellationToken cancellationToken);
    }
}