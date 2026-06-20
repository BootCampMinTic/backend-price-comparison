using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.RegisterPrice;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.GetPriceComparisonByProduct;

namespace Backend.PriceComparison.Api.Endpoints;

/// <summary>
/// Mapea los endpoints HTTP asociados al registro e historial de precios de un producto.
/// </summary>
public static class PriceHistoryEndpoints
{
    /// <summary>
    /// Configura las rutas para el caso de uso de registro y consulta de comparación de precios de productos.
    /// </summary>
    /// <param name="app">Constructor de rutas del endpoint.</param>
    public static void MapPriceHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("PriceHistory");

        // POST api/v1/prices - Registrar precio de un producto en un supermercado
        group.MapPost("prices", RegisterPrice)
            .WithName("RegisterPrice")
            .WithSummary("Registrar el precio de un producto en un supermercado específico")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET api/v1/products/{productId:int}/prices - Comparar precios de un producto en diferentes supermercados
        group.MapGet("products/{productId:int}/prices", GetPriceComparison)
            .WithName("GetPriceComparison")
            .WithSummary("Consultar la comparación de precios de un producto en diferentes supermercados")
            .Produces<ApiResponse<IEnumerable<PriceHistoryDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<IEnumerable<PriceHistoryDto>>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> RegisterPrice(
        [FromBody] RegisterPriceCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
        {
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Precio registrado exitosamente."));
        }

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> GetPriceComparison(
        int productId,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetPriceComparisonByProductQuery(productId));
        if (result.IsSuccess)
        {
            return TypedResults.Ok(ApiResponse<IEnumerable<PriceHistoryDto>>.SuccessResponse(result.Value!, "Comparación de precios recuperada exitosamente."));
        }

        return TypedResults.BadRequest(ApiResponse<IEnumerable<PriceHistoryDto>>.ErrorResponse(result.Error!.Description));
    }
}
