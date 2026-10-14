using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Application.Dealers;
using OrderFlow.Application.Products;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Orders.Commands
{
    public record CreateOrderCommand(Guid DealerId, List<OrderLine> Lines);
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDealerRepository _dealerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(IOrderRepository orderRepository, IDealerRepository dealerRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _dealerRepository = dealerRepository;
        }
        public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            Dealer? dealer = await _dealerRepository.GetByIdAsync(command.DealerId, cancellationToken);
            if (dealer is null)
                throw new NotFoundException("Dealer is not defined");
            if (!dealer.IsActive)
                throw new DomainException("Dealer is not active");
            if (command.Lines is null || command.Lines.Count == 0)
                throw new DomainException("Order must have at least one item");

            Order order = new Order(command.DealerId);

            foreach (var line in command.Lines)
            {
                Product? product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);
                if (product is null)
                    throw new NotFoundException("Product is not defined");
                if (!product.IsActive)
                    throw new DomainException("Product is not active");

                order.AddItem(product.Id, product.Name, product.Price, line.Quantity);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}