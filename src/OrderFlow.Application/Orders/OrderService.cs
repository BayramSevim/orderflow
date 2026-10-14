using OrderFlow.Application.Common;
using OrderFlow.Application.Dealers;
using OrderFlow.Application.Products;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Orders
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDealerRepository _dealerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IDealerRepository dealerRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _dealerRepository = dealerRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }
        public async Task<Guid> CreateAsync(Guid dealerId, List<OrderLine> lines, CancellationToken ct)
        {
            Dealer? dealer = await _dealerRepository.GetByIdAsync(dealerId, ct);
            if (dealer is null)
                throw new NotFoundException("Dealer is not defined");
            if (!dealer.IsActive)
                throw new DomainException("Dealer is not active");
            if (lines is null || lines.Count == 0)
                throw new DomainException("Order must have at least one item");

            Order order = new Order(dealerId);

            foreach (var line in lines)
            {
                Product? product = await _productRepository.GetByIdAsync(line.ProductId, ct);
                if (product is null)
                    throw new NotFoundException("Product is not defined");
                if (!product.IsActive)
                    throw new DomainException("Product is not active");

                order.AddItem(product.Id, product.Name, product.Price, line.Quantity);
            }

            await _orderRepository.AddAsync(order, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return order.Id;
        }

        public async Task<OrderResponse> GetByIdAsync(Guid Id, CancellationToken ct)
        {
            Order? order = await _orderRepository.GetByIdAsync(Id, ct);
            if (order is null)
                throw new NotFoundException("Order is not defined");


            return new OrderResponse(order.Id,
                                    order.DealerId,
                                    order.Status.ToString(),
                                    order.CreatedAt,
                                    order.Total,
                                    order.Items
                                            .Select(i => new OrderItemResponse(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal))
                                            .ToList()
                                    );
        }

        public async Task<List<Order>> GetOrdersAsync(CancellationToken ct) => await _orderRepository.GetOrdersAsync(ct);
        public async Task<string> ApproveAsync(Guid id, CancellationToken ct) => await ChangeStatus(id, order => order.Approve(), ct);
        public async Task<string> ShipAsync(Guid id, CancellationToken ct) => await ChangeStatus(id, order => order.Ship(), ct);
        public async Task<string> DeliverAsync(Guid id, CancellationToken ct) => await ChangeStatus(id, order => order.Deliver(), ct);
        public async Task<string> CancelAsync(Guid id, CancellationToken ct) => await ChangeStatus(id, order => order.Cancel(), ct);

        private async Task<string> ChangeStatus(Guid id, Action<Order> change, CancellationToken ct)
        {
            Order? order = await _orderRepository.GetByIdAsync(id, ct);
            if (order is null)
                throw new NotFoundException("Order is not defined");

            change(order);
            await _unitOfWork.SaveChangesAsync(ct);

            return order.Status.ToString();
        }
    }
}