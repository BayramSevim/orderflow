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

    }
}