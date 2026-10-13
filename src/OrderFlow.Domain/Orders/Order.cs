using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Orders
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid DealerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items;
        public decimal Total => _items.Sum(i => i.LineTotal);

        public Order(Guid dealerId)
        {
            Id = Guid.NewGuid();
            DealerId = dealerId;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Only pending orders can be modified");

            OrderItem orderItem = new OrderItem(productId, productName, unitPrice, quantity);
            _items.Add(orderItem);
        }

        public void Approve()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Only pending orders can be approved");
            if (_items.Count == 0)
                throw new DomainException("Cannot approve an empty order");

            Status = OrderStatus.Approved;
        }

        public void Ship()
        {
            if (Status != OrderStatus.Approved)
                throw new DomainException("Only approved orders can be shipped");

            Status = OrderStatus.Shipped;
        }

        public void Deliver()
        {
            if (Status != OrderStatus.Shipped)
                throw new DomainException("Only shipped orders can be delivered");

            Status = OrderStatus.Delivered;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Pending && Status != OrderStatus.Approved)
                throw new DomainException("Only pending or approved orders can be cancelled");

            Status = OrderStatus.Cancelled;
        }
    }
}