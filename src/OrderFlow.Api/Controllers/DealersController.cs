using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Dealers;
using OrderFlow.Application.Dealers;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/dealers")]
    public class DealersController : ControllerBase
    {
        private readonly DealerService _dealerService;

        public DealersController(DealerService dealerService)
        {
            _dealerService = dealerService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDealerRequest request, CancellationToken cancellationToken)
        {
            var id = await _dealerService.CreateAsync(request.Code, request.Name, request.CreditLimit, cancellationToken);
            return Ok(new { id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dealer = await _dealerService.GetByIdAsync(id, cancellationToken);

            if (dealer is null)
                return NotFound();

            return Ok(dealer);
        }

        [HttpGet]
        public async Task<IActionResult> GetDealers(CancellationToken cancellationToken)
        {
            return Ok(await _dealerService.GetDealersAsync(cancellationToken));
        }
    }
}