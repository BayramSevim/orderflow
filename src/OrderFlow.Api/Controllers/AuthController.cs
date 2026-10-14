using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Auth;
using OrderFlow.Api.Dealers;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Application.Dealers;
using OrderFlow.Application.Users.Commands;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Create(
                            RegisterRequest request,
                            [FromServices] ICommandHandler<RegisterUserCommand, Guid> handler,
                            CancellationToken cancellationToken)
        {
            var id = await handler.Handle(new RegisterUserCommand(request.Email, request.Password, request.Role, request.DealerId), cancellationToken);
            return Ok(new { id });
        }
    }
}