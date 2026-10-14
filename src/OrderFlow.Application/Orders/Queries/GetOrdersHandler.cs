using OrderFlow.Application.Common.Messaging;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.Queries
{
    public record GetOrdersQuery();

    public class GetOrdersHandler : IQueryHandler<GetOrdersQuery, List<OrderResponse>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<List<OrderResponse>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            List<Order> orders = await _orderRepository.GetOrdersAsync(cancellationToken);
            return orders.Select(o => new OrderResponse(o.Id,
                                                    o.DealerId,
                                                    o.Status.ToString(),
                                                    o.CreatedAt,
                                                    o.Total,
                                                    o.Items.Select(i => new OrderItemResponse(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList())).ToList();
        }
    }
}