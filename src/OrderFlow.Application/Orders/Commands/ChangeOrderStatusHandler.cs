using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Application.Dealers;
using OrderFlow.Application.Products;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Orders.Commands
{
    public enum OrderAction { Approve, Ship, Deliver, Cancel }

    public record ChangeOrderStatusCommand(Guid OrderId, OrderAction Action);
    public class ChangeOrderStatusHandler : ICommandHandler<ChangeOrderStatusCommand, string>
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeOrderStatusHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public async Task<string> Handle(ChangeOrderStatusCommand command, CancellationToken cancellationToken)
        {
            Order? order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
            if (order is null)
                throw new NotFoundException("Order is not defined");

            switch (command.Action)
            {
                case OrderAction.Approve:
                    order.Approve();

                    foreach (var line in order.Items)
                    {
                        Product? product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);
                        if (product is null)
                            throw new NotFoundException("Product is not defined");

                        product.DecreaseStock(line.Quantity);
                    }

                    break;
                case OrderAction.Ship:
                    order.Ship();
                    break;
                case OrderAction.Deliver:
                    order.Deliver();
                    break;
                case OrderAction.Cancel:
                    var wasApproved = order.Status == OrderStatus.Approved;

                    if (wasApproved)
                    {
                        foreach (var line in order.Items)
                        {
                            Product? product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken);
                            if (product is null)
                                throw new NotFoundException("Product is not defined");

                            product.IncreaseStock(line.Quantity);
                        }
                    }

                    order.Cancel();
                    break;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return order.Status.ToString();
        }
    }
}