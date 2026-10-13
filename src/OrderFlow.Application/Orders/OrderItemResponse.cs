namespace OrderFlow.Application.Orders;

public record OrderItemResponse(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal) { }
