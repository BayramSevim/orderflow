using OrderFlow.Application.Orders;
using OrderFlow.Application.Orders.Commands;
using OrderFlow.Application.Tests.Fakes;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Tests.Orders
{
    public class CreateOrderHandlerTests
    {
        private readonly FakeOrderRepository _orders = new();
        private readonly FakeDealerRepository _dealers = new();
        private readonly FakeProductRepository _products = new();
        private readonly FakeUnitOfWork _unitOfWork = new();

        private CreateOrderHandler CreateHandler(FakeCurrentUser currentUser)
            => new CreateOrderHandler(_orders, _dealers, _products, _unitOfWork, currentUser);

        [Fact]
        public async Task Handle_ValidOrder_SavesOrderWithPriceSnapshot()
        {
            var dealer = new Dealer("BAY-001", "Ege Yem", 10000);
            _dealers.Dealers.Add(dealer);
            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            _products.Products.Add(product);

            var handler = CreateHandler(new FakeCurrentUser { Role = "Dealer", DealerId = dealer.Id });
            var command = new CreateOrderCommand(dealer.Id, new List<OrderLine> { new(product.Id, 10) });

            var orderId = await handler.Handle(command, CancellationToken.None);

            product.ChangePrice(900);

            var order = Assert.Single(_orders.Orders);
            Assert.Equal(orderId, order.Id);
            Assert.Equal(850, Assert.Single(order.Items).UnitPrice);
            Assert.Equal(8500, order.Total);
            Assert.Equal(1, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task Handle_DealerOrdersForAnotherDealer_ThrowsForbidden()
        {
            var dealerA = new Dealer("BAY-001", "Ege Yem", 10000);
            var dealerB = new Dealer("BAY-002", "Marmara Tarim", 10000);
            _dealers.Dealers.AddRange(new[] { dealerA, dealerB });
            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            _products.Products.Add(product);

            var handler = CreateHandler(new FakeCurrentUser { Role = "Dealer", DealerId = dealerB.Id });
            var command = new CreateOrderCommand(dealerA.Id, new List<OrderLine> { new(product.Id, 10) });

            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));

            Assert.Empty(_orders.Orders);
            Assert.Equal(0, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task Handle_InactiveProduct_Throws()
        {
            var dealer = new Dealer("BAY-001", "Ege Yem", 10000);
            _dealers.Dealers.Add(dealer);
            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            product.Deactivate();
            _products.Products.Add(product);

            var handler = CreateHandler(new FakeCurrentUser());
            var command = new CreateOrderCommand(dealer.Id, new List<OrderLine> { new(product.Id, 10) });

            await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

            Assert.Empty(_orders.Orders);
        }

        [Fact]
        public async Task Handle_UnknownDealer_ThrowsNotFound()
        {
            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            _products.Products.Add(product);

            var handler = CreateHandler(new FakeCurrentUser());
            var command = new CreateOrderCommand(Guid.NewGuid(), new List<OrderLine> { new(product.Id, 10) });

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
