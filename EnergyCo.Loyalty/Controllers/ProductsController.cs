using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace EnergyCo.Loyalty.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IDispatcher dispatcher) : ControllerBase
{
    /// <summary>List all available products.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetProductsQuery(), cancellationToken);
        return Ok(result.Value);
    }
}
