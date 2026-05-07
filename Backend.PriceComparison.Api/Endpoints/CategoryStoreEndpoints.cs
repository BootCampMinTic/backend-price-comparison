using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.CategoryStore;

namespace Backend.PriceComparison.Api.Endpoints;

public static class CategoryStoreEndpoints
{
    public static void MapCategoryStoreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Catalog");

        group.MapGet("category-stores", GetAllCategoryStores)
            .WithName("GetAllCategoryStores")
            .WithSummary("Get all store categories")
            .Produces<IEnumerable<CategoryStoreDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("category-stores/{id:int}", GetCategoryStoreById)
            .WithName("GetCategoryStoreById")
            .WithSummary("Get store category by ID")
            .Produces<CategoryStoreDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("category-stores", CreateCategoryStore)
            .WithName("CreateCategoryStore")
            .WithSummary("Create a new store category")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllCategoryStores(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllCategoryStoresQuery());
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> GetCategoryStoreById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetCategoryStoreByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> CreateCategoryStore(
        [FromBody] CreateCategoryStoreCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Category store created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
