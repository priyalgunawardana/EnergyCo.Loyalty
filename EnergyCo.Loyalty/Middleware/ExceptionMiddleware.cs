using System.Net;
using System.Text.Json;
using EnergyCo.Loyalty.Domain.Exceptions;

namespace EnergyCo.Loyalty.Api.Middleware;

internal sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            BasketNotFoundException or ProductNotFoundException => (HttpStatusCode.NotFound, "Not Found"),
            BasketEmptyException => (HttpStatusCode.UnprocessableEntity, "Unprocessable Entity"),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Not Found"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = $"https://tools.ietf.org/html/rfc9110#section-15.{(int)status / 100}.{(int)status % 100}",
            title,
            status = (int)status,
            detail = ex.Message
        };

        return context.Response.WriteAsJsonAsync(problem, _jsonOptions);
    }
}
