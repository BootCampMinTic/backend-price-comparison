using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateSale;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.Sale;
using System.Net;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Api.Endpoints;

public static class SaleEndpoints
{
    public static void MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Sale");

        group.MapGet("sales", GetAllSales)
            .WithName("GetAllSales")
            .WithSummary("Get all sales (paginated)")
            .Produces<PagedResponse<IEnumerable<SaleDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("sales/{id:int}", GetSaleById)
            .WithName("GetSaleById")
            .WithSummary("Get sale by ID with product details")
            .Produces<SaleDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("sales", CreateSale)
            .WithName("CreateSale")
            .WithSummary("Create a new sale with products")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllSales(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllSalesQuery { PageNumber = pageNumber, PageSize = pageSize });
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<SaleDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<SaleDto>>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> GetSaleById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetSaleByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> CreateSale(
        [FromBody] CreateSaleCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Sale created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
