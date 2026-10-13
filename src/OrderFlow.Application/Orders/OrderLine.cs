namespace OrderFlow.Application.Orders;

public record OrderLine(Guid ProductId, int Quantity);
