using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Orders;
using OrderFlow.Application.Orders;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var id = await _orderService.CreateAsync(request.DealerId, request.Items.Select(i => new OrderLine(i.ProductId, i.Quantity)).ToList(), cancellationToken);
            return Ok(new { id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _orderService.GetByIdAsync(id, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
        {
            return Ok(await _orderService.GetOrdersAsync(cancellationToken));
        }

        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken) => Ok(await _orderService.ApproveAsync(id, cancellationToken));

        [HttpPost("{id:guid}/ship")]
        public async Task<IActionResult> Ship(Guid id, CancellationToken cancellationToken) => Ok(await _orderService.ShipAsync(id, cancellationToken));

        [HttpPost("{id:guid}/deliver")]
        public async Task<IActionResult> Deliver(Guid id, CancellationToken cancellationToken) => Ok(await _orderService.DeliverAsync(id, cancellationToken));

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken) => Ok(await _orderService.CancelAsync(id, cancellationToken));
    }
}