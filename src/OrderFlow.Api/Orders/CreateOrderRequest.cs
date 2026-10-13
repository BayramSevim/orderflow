namespace OrderFlow.Api.Orders;

public record CreateOrderRequest(Guid DealerId, List<CreateOrderItemRequest> Items);