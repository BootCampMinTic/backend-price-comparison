using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.CategoryProduct;

namespace Backend.PriceComparison.Api.Endpoints;

public static class CategoryProductEndpoints
{
    public static void MapCategoryProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Catalog");

        group.MapGet("category-products", GetAllCategoryProducts)
            .WithName("GetAllCategoryProducts")
            .WithSummary("Get all product categories")
            .Produces<IEnumerable<CategoryProductDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("category-products/{id:int}", GetCategoryProductById)
            .WithName("GetCategoryProductById")
            .WithSummary("Get product category by ID")
            .Produces<CategoryProductDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("category-products", CreateCategoryProduct)
            .WithName("CreateCategoryProduct")
            .WithSummary("Create a new product category")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllCategoryProducts(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllCategoryProductsQuery());
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> GetCategoryProductById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetCategoryProductByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> CreateCategoryProduct(
        [FromBody] CreateCategoryProductCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Category product created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
