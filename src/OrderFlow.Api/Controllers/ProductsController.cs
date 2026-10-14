using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Dealers;
using OrderFlow.Api.Products;
using OrderFlow.Application.Dealers;
using OrderFlow.Application.Products;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var id = await _productService.CreateAsync(request.Code, request.Name, request.Price, request.StockQuantity, cancellationToken);
            return Ok(new { id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
        {
            return Ok(await _productService.GetProductsAsync(cancellationToken));
        }
    }
}