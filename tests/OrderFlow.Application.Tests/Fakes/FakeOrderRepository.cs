using OrderFlow.Application.Orders;
using OrderFlow.Application.Products;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Tests.Fakes
{
    public class FakeOrderRepository : IOrderRepository
    {
        public List<Order> Orders { get; } = new();

        public Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            Orders.Add(order);
            return Task.CompletedTask;
        }

        public Task<List<Order>> GetByDealerIdAsync(Guid dealerId, CancellationToken cancellationToken)
                => Task.FromResult(Orders.Where(x => x.DealerId == dealerId).ToList());

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Orders.FirstOrDefault(x => x.Id == id));

        public Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken) => Task.FromResult(Orders);
    }
}