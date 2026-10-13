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
    }
}