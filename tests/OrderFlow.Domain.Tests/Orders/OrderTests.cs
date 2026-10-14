using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Domain.Tests.Orders
{
    public class OrderTests
    {
        [Fact]
        public void Constructor_NewOrder_IsPending()
        {
            Order order = new Order(Guid.NewGuid());
            Assert.Equal(OrderStatus.Pending, order.Status);
        }

        [Fact]
        public void AddItem_ValidItem_CalculatesTotal()
        {
            Order order = new Order(Guid.NewGuid());
            Product product = new Product("YEM-1", "Besi Yemi", 12, 100);
            order.AddItem(product.Id, product.Name, 10, 5);
            order.AddItem(product.Id, product.Name, 20, 10);

            Assert.Equal(250, order.Total);
        }

        [Fact]
        public void Approve_EmptyOrder_Throws()
        {
            Order order = new Order(Guid.NewGuid());
            Assert.Throws<DomainException>(() => order.Approve());
        }

        [Fact]
        public void Approve_PendingWithItems_BecomesApproved()
        {
            Order order = new Order(Guid.NewGuid());
            Product product = new Product("YEM-1", "Besi Yemi", 12, 100);
            order.AddItem(product.Id, product.Name, 10, 5);
            order.Approve();
            Assert.Equal(OrderStatus.Approved, order.Status);
        }

        [Fact]
        public void Ship_PendingOrder_Throws()
        {
            Order order = new Order(Guid.NewGuid());
            Assert.Throws<DomainException>(() => order.Ship());
        }

        [Fact]
        public void Cancel_ShippedOrder_Throws()
        {
            Order order = new Order(Guid.NewGuid());
            Product product = new Product("YEM-1", "Besi Yemi", 12, 100);
            order.AddItem(product.Id, product.Name, 10, 5);
            order.Approve();
            order.Ship();
            Assert.Throws<DomainException>(() => order.Cancel());
        }

        [Fact]
        public void AddItem_ApprovedOrder_Throws()
        {
            Order order = new Order(Guid.NewGuid());
            Product product = new Product("YEM-1", "Besi Yemi", 12, 100);
            order.AddItem(product.Id, product.Name, 10, 5);
            order.Approve();
            Assert.Throws<DomainException>(() => order.AddItem(product.Id, product.Name, 10, 10));
        }
    }
}