using OrderFlow.Application.Orders.Commands;
using OrderFlow.Application.Tests.Fakes;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Tests.Orders
{
    public class ChangeOrderStatusHandlerTests
    {
        [Fact]
        public async Task Approve_DecreasesProductStock()
        {
            var products = new FakeProductRepository();
            var orders = new FakeOrderRepository();
            var unitOfWork = new FakeUnitOfWork();
            var currentUser = new FakeCurrentUser();

            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            products.Products.Add(product);

            var order = new Order(Guid.NewGuid());
            order.AddItem(product.Id, product.Name, product.Price, 30);
            orders.Orders.Add(order);

            var handler = new ChangeOrderStatusHandler(orders, products, unitOfWork, currentUser);

            await handler.Handle(new ChangeOrderStatusCommand(order.Id, OrderAction.Approve), CancellationToken.None);

            Assert.Equal(70, product.StockQuantity);
            Assert.Equal(OrderStatus.Approved, order.Status);
            Assert.Equal(1, unitOfWork.SaveCount);
        }

        [Fact]
        public async Task Cancel_ApprovedOrder_RestoresStock()
        {
            var products = new FakeProductRepository();
            var orders = new FakeOrderRepository();
            var unitOfWork = new FakeUnitOfWork();
            var currentUser = new FakeCurrentUser();

            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            products.Products.Add(product);

            var order = new Order(Guid.NewGuid());
            order.AddItem(product.Id, product.Name, product.Price, 30);
            orders.Orders.Add(order);

            var handler = new ChangeOrderStatusHandler(orders, products, unitOfWork, currentUser);

            await handler.Handle(new ChangeOrderStatusCommand(order.Id, OrderAction.Approve), CancellationToken.None);
            await handler.Handle(new ChangeOrderStatusCommand(order.Id, OrderAction.Cancel), CancellationToken.None);

            Assert.Equal(100, product.StockQuantity);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(2, unitOfWork.SaveCount);
        }

        [Fact]
        public async Task Approve_InsufficientStock_Throws()
        {
            var products = new FakeProductRepository();
            var orders = new FakeOrderRepository();
            var unitOfWork = new FakeUnitOfWork();
            var currentUser = new FakeCurrentUser();

            var product = new Product("YEM-1", "Besi Yemi", 850, 10);
            products.Products.Add(product);

            var order = new Order(Guid.NewGuid());
            order.AddItem(product.Id, product.Name, product.Price, 30);
            orders.Orders.Add(order);

            var handler = new ChangeOrderStatusHandler(orders, products, unitOfWork, currentUser);

            await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new ChangeOrderStatusCommand(order.Id, OrderAction.Approve), CancellationToken.None));

            Assert.Equal(10, product.StockQuantity);
            Assert.Equal(0, unitOfWork.SaveCount);
        }

        [Fact]
        public async Task Cancel_PendingOrder_DoesNotChangeStock()
        {
            var products = new FakeProductRepository();
            var orders = new FakeOrderRepository();
            var unitOfWork = new FakeUnitOfWork();
            var currentUser = new FakeCurrentUser();

            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            products.Products.Add(product);

            var order = new Order(Guid.NewGuid());
            order.AddItem(product.Id, product.Name, product.Price, 30);
            orders.Orders.Add(order);

            var handler = new ChangeOrderStatusHandler(orders, products, unitOfWork, currentUser);

            await handler.Handle(new ChangeOrderStatusCommand(order.Id, OrderAction.Cancel), CancellationToken.None);

            Assert.Equal(100, product.StockQuantity);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Fact]
        public async Task Cancel_OtherDealersOrder_ThrowsNotFound()
        {
            var products = new FakeProductRepository();
            var orders = new FakeOrderRepository();
            var unitOfWork = new FakeUnitOfWork();

            var dealerA = Guid.NewGuid();
            var dealerB = Guid.NewGuid();
            var currentUser = new FakeCurrentUser { Role = "Dealer", DealerId = dealerB };

            var product = new Product("YEM-1", "Besi Yemi", 850, 100);
            products.Products.Add(product);

            var order = new Order(dealerA);
            order.AddItem(product.Id, product.Name, product.Price, 30);
            orders.Orders.Add(order);

            var handler = new ChangeOrderStatusHandler(orders, products, unitOfWork, currentUser);

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new ChangeOrderStatusCommand(order.Id, OrderAction.Cancel), CancellationToken.None));

            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(0, unitOfWork.SaveCount);
        }
    }
}