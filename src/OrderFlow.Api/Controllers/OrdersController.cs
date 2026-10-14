using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Orders;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Application.Orders;
using OrderFlow.Application.Orders.Commands;
using OrderFlow.Application.Orders.Queries;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateOrderRequest request,
            [FromServices] ICommandHandler<CreateOrderCommand, Guid> handler,
            CancellationToken cancellationToken)
        {
            var command = new CreateOrderCommand(request.DealerId, request.Items.Select(i => new OrderLine(i.ProductId, i.Quantity)).ToList());
            var id = await handler.Handle(command, cancellationToken);
            return Ok(new { id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            [FromServices] IQueryHandler<GetOrderByIdQuery, OrderResponse> handler,
            CancellationToken cancellationToken)
        {
            return Ok(await handler.Handle(new GetOrderByIdQuery(id), cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromServices] IQueryHandler<GetOrdersQuery, List<OrderResponse>> handler,
            CancellationToken cancellationToken)
        {
            return Ok(await handler.Handle(new GetOrdersQuery(), cancellationToken));
        }

        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromServices] ICommandHandler<ChangeOrderStatusCommand, string> handler, CancellationToken cancellationToken)
        => Ok(await handler.Handle(new ChangeOrderStatusCommand(id, OrderAction.Approve), cancellationToken));

        [HttpPost("{id:guid}/ship")]
        public async Task<IActionResult> Ship(Guid id, [FromServices] ICommandHandler<ChangeOrderStatusCommand, string> handler, CancellationToken cancellationToken)
        => Ok(await handler.Handle(new ChangeOrderStatusCommand(id, OrderAction.Ship), cancellationToken));

        [HttpPost("{id:guid}/deliver")]
        public async Task<IActionResult> Deliver(Guid id, [FromServices] ICommandHandler<ChangeOrderStatusCommand, string> handler, CancellationToken cancellationToken)
        => Ok(await handler.Handle(new ChangeOrderStatusCommand(id, OrderAction.Deliver), cancellationToken));

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromServices] ICommandHandler<ChangeOrderStatusCommand, string> handler, CancellationToken cancellationToken)
        => Ok(await handler.Handle(new ChangeOrderStatusCommand(id, OrderAction.Cancel), cancellationToken));

    }
}