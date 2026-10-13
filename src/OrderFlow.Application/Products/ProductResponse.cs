namespace OrderFlow.Application.Products;

public record ProductResponse(Guid Id, string Code, string Name, decimal Price, int StockQuantity, bool IsActive);
