namespace OrderFlow.Application.Dealers
{
    public record DealerResponse(Guid Id, string Code, string Name, decimal CreditLimit, bool IsActive);
}