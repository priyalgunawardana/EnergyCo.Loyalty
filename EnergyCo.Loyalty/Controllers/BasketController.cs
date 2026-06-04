using EnergyCo.Loyalty.Api.Contracts;
using EnergyCo.Loyalty.Api.Mappings;
using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Baskets.Commands.AddToBasket;
using EnergyCo.Loyalty.Application.Baskets.Commands.CheckoutBasket;
using EnergyCo.Loyalty.Application.Baskets.Commands.ExpressCheckout;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace EnergyCo.Loyalty.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:guid}/basket")]
public sealed class BasketController(IDispatcher dispatcher) : ControllerBase
{
    /// <summary>Retrieves the current basket for a customer.</summary>
    [HttpGet]
    [ProducesResponseType<BasketResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBasket(
        [FromRoute] Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(
            new GetBasketQuery(customerId.ToString()),
            cancellationToken);

        if (result.IsFailure) return ToErrorResponse(result);
        return Ok(ApiResponseMapper.ToBasketResponse(result.Value!));
    }

    /// <summary>Adds a product to the customer's basket.</summary>
    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(
        [FromRoute] Guid customerId,
        [FromBody] AddToBasketRequest request,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(
            new AddToBasketCommand(customerId.ToString(), request.ProductId, request.UnitPrice, request.Quantity),
            cancellationToken);

        return result.IsFailure ? ToErrorResponse(result) : NoContent();
    }

    /// <summary>Checks out the customer's basket and returns the final rewards and discount summary.</summary>
    [HttpPost("checkout")]
    [ProducesResponseType<CheckoutResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Checkout(
        [FromRoute] Guid customerId,
        [FromBody] CheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (!ApiResponseMapper.TryParseTransactionDate(request.TransactionDate, out var transactionDate))
            return InvalidTransactionDateResponse();

        var result = await dispatcher.SendAsync(
            new CheckoutBasketCommand(customerId.ToString(), request.LoyaltyCard, transactionDate),
            cancellationToken);

        if (result.IsFailure) return ToErrorResponse(result);
        return Ok(ApiResponseMapper.ToCheckoutResponse(result.Value!));
    }

    /// <summary>
    /// Submits a full basket in a single request and returns the final rewards and discount summary.
    /// </summary>
    [HttpPost("checkout/express")]
    [ProducesResponseType<CheckoutResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExpressCheckout(
        [FromRoute] Guid customerId,
        [FromBody] ExpressCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (!ApiResponseMapper.TryParseTransactionDate(request.TransactionDate, out var transactionDate))
            return InvalidTransactionDateResponse();

        var items = request.Items
            .Select(i => new ExpressCheckoutItem(i.ProductId, i.UnitPrice, i.Quantity))
            .ToList();

        var result = await dispatcher.SendAsync(
            new ExpressCheckoutCommand(customerId.ToString(), request.LoyaltyCard, transactionDate, items),
            cancellationToken);

        if (result.IsFailure) return ToErrorResponse(result);
        return Ok(ApiResponseMapper.ToCheckoutResponse(result.Value!));
    }

    private IActionResult InvalidTransactionDateResponse() =>
        Problem(
            detail:     $"TransactionDate must be in {ApiResponseMapper.TransactionDateFormat} format, e.g. {ApiResponseMapper.TransactionDateExample}.",
            statusCode: StatusCodes.Status400BadRequest,
            title:      "Bad Request");

    private IActionResult ToErrorResponse(Result result)
    {
        var isNotFound = result.ErrorKind == ErrorKind.NotFound;

        return Problem(
            detail:     result.Error,
            statusCode: isNotFound ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
            title:      isNotFound ? "Not Found" : "Bad Request");
    }
}


