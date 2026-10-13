using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders;

public record OrderResponse(Guid Id, Guid DealerId, string Status, DateTime CreatedAt, decimal Total, List<OrderItemResponse> Items);
