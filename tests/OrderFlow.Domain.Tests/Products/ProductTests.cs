using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Products;

namespace OrderFlow.Domain.Tests.Products
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_ValidValues_SetsProperties()
        {
            Product product = new Product("YEM-1", "Besi Yemi", 12, 10);

            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal("YEM-1", product.Code);
            Assert.Equal("Besi Yemi", product.Name);
            Assert.Equal(10, product.StockQuantity);
            Assert.True(product.IsActive);
        }

        [Fact]
        public void Constructor_ZeroPrice_Throws()
        {
            Assert.Throws<DomainException>(() => new Product("YEM-1", "Besi Yemi", 0, 10));
        }

        [Fact]
        public void DecreaseStock_EnoughStock_ReducesQuantity()
        {
            Product product = new Product("YEM-1", "Besi Yemi", 12, 100);

            product.DecreaseStock(70);
            Assert.Equal(30, product.StockQuantity);
        }

        [Fact]
        public void DecreaseStock_InsufficientStock_Throws()
        {
            Product product = new Product("YEM-1", "Besi Yemi", 12, 10);

            Assert.Throws<DomainException>(() => product.DecreaseStock(30));
        }
    }
}