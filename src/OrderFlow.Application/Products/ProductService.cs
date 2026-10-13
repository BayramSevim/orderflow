using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Products
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(string code, string name, decimal price, int stockQuantity, CancellationToken cancellationToken)
        {
            if (await _productRepository.CodeExistsAsync(code, cancellationToken))
                throw new ConflictException("Product code already exists");

            var product = new Product(code, name, price, stockQuantity);

            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Id;
        }

        public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);

            if (product is null)
                return null;

            return new ProductResponse(product.Id, product.Code, product.Name, product.Price, product.StockQuantity, product.IsActive);
        }

        public async Task<List<ProductResponse>> GetProductsAsync(CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetProductsAsync(cancellationToken);

            return products
                    .Select(d => new ProductResponse(d.Id, d.Code, d.Name, d.Price, d.StockQuantity, d.IsActive))
                    .ToList();
        }

    }
}