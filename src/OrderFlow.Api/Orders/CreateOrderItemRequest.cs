namespace OrderFlow.Api.Orders;

public record CreateOrderItemRequest(Guid ProductId, int Quantity);
