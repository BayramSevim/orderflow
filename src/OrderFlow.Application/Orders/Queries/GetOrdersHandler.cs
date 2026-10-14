using OrderFlow.Application.Common;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.Queries
{
    public record GetOrdersQuery();

    public class GetOrdersHandler : IQueryHandler<GetOrdersQuery, List<OrderResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUser _currentUser;

        public GetOrdersHandler(IOrderRepository orderRepository, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderResponse>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            List<Order> orders = _currentUser.IsAdmin
                                ? await _orderRepository.GetOrdersAsync(cancellationToken)
                                : await _orderRepository.GetByDealerIdAsync(_currentUser.DealerId!.Value, cancellationToken);
            return orders.Select(o => new OrderResponse(o.Id,
                                                    o.DealerId,
                                                    o.Status.ToString(),
                                                    o.CreatedAt,
                                                    o.Total,
                                                    o.Items.Select(i => new OrderItemResponse(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList())).ToList();
        }
    }
}