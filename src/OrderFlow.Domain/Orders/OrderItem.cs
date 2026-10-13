using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Orders
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal => UnitPrice * Quantity;


        internal OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero");
            if (unitPrice <= 0)
                throw new DomainException("UnitPrice must be greater than zero");

            Id = Guid.NewGuid();
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}