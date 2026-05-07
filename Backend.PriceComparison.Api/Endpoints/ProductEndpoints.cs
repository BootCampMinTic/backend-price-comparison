using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateProduct;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.Product;
using System.Net;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Product");

        group.MapGet("products", GetAllProducts)
            .WithName("GetAllProducts")
            .WithSummary("Get all products (paginated)")
            .Produces<PagedResponse<IEnumerable<ProductDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("products/{id:int}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Get product by ID")
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("stores/{storeId:int}/products", GetProductsByStore)
            .WithName("GetProductsByStore")
            .WithSummary("Get products by store (paginated)")
            .Produces<PagedResponse<IEnumerable<ProductDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("products", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllProducts(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllProductsQuery { PageNumber = pageNumber, PageSize = pageSize });
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<ProductDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<ProductDto>>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> GetProductById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> GetProductsByStore(
        int storeId,
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetProductsByStoreQuery(storeId, pageNumber, pageSize));
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<ProductDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<ProductDto>>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Product created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
