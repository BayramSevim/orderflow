using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Products
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; }
        public uint Version { get; private set; }

        public Product(string code, string name, decimal price, int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Code is required");
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");
            if (price <= 0)
                throw new DomainException("Price must be greater than zero");
            if (stockQuantity < 0)
                throw new DomainException("Stock quantity cannot be negative");


            Id = Guid.NewGuid();
            Code = code;
            Name = name;
            Price = price;
            StockQuantity = stockQuantity;
            IsActive = true;
        }

        public void ChangePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new DomainException("Price must be greater than zero");

            Price = newPrice;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero");
            if (StockQuantity < quantity)
                throw new DomainException($"Insufficient stock for {Code}");

            StockQuantity -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero");

            StockQuantity += quantity;
        }
    }
}