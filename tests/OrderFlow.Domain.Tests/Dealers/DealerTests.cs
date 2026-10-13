using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Tests.Dealers;

public class DealerTests
{
    [Fact]
    public void Constructor_ValidValues_SetsProperties()
    {
        Dealer dealer = new Dealer("BAY-001", "Ege Yem", 10000);

        Assert.NotEqual(Guid.Empty, dealer.Id);
        Assert.Equal("BAY-001", dealer.Code);
        Assert.Equal("Ege Yem", dealer.Name);
        Assert.Equal(10000, dealer.CreditLimit);
        Assert.True(dealer.IsActive);
    }

    [Fact]
    public void Constructor_NegativeCreditLimit_Throws()
    {
        Assert.Throws<DomainException>(() => new Dealer("BAY-001", "Ege Yem", -1));
    }
}