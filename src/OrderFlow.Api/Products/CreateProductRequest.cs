namespace OrderFlow.Api.Products
{
    public record CreateProductRequest(string Code, string Name, decimal Price, int StockQuantity);
}