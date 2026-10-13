namespace OrderFlow.Api.Dealers
{
    public record CreateDealerRequest(string Code, string Name, decimal CreditLimit);
}