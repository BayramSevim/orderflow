using OrderFlow.Application.Common;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.Queries
{
    public record GetOrderByIdQuery(Guid OrderId);

    public class GetOrderByIdHandler : IQueryHandler<GetOrderByIdQuery, OrderResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUser _currentUser;

        public GetOrderByIdHandler(IOrderRepository orderRepository, ICurrentUser currentUser)
        {
            _orderRepository = orderRepository;
            _currentUser = currentUser;
        }

        public async Task<OrderResponse> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            Order? order = await _orderRepository.GetByIdAsync(query.OrderId, cancellationToken);
            if (order is null)
                throw new NotFoundException("Order is not defined");

            if (!_currentUser.IsAdmin && order.DealerId != _currentUser.DealerId)
                throw new NotFoundException("Order is not defined");

            return new OrderResponse(order.Id,
                                    order.DealerId,
                                    order.Status.ToString(),
                                    order.CreatedAt,
                                    order.Total,
                                    order.Items
                                            .Select(i => new OrderItemResponse(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal))
                                            .ToList()
                                    );
        }
    }
}