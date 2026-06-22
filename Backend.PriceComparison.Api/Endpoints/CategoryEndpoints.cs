using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;
using Backend.PriceComparison.Application.Store.Commands.UpdateCategoryProduct;
using Backend.PriceComparison.Application.Store.Commands.UpdateCategoryStore;
using Backend.PriceComparison.Application.Store.Commands.DeleteCategoryProduct;
using Backend.PriceComparison.Application.Store.Commands.DeleteCategoryStore;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.CategoryProduct;
using Backend.PriceComparison.Application.Store.Queries.CategoryStore;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/categories");

        // ─── Category Product endpoints ─────────────────────────────────────

        group.MapGet("products", GetAllCategoryProducts)
            .WithTags("CategoryProduct")
            .WithName("GetAllCategoryProducts")
            .WithSummary("Get all product categories (paginated)")
            .WithDescription("Retrieves a paginated list of all product categories available in the system.")
            .Produces<PagedResponse<IEnumerable<CategoryProductDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("products/{id:int}", GetCategoryProductById)
            .WithTags("CategoryProduct")
            .WithName("GetCategoryProductById")
            .WithSummary("Get a product category by ID")
            .WithDescription("Retrieves a single product category by its unique identifier.")
            .Produces<CategoryProductDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("products", CreateCategoryProduct)
            .WithTags("CategoryProduct")
            .WithName("CreateCategoryProduct")
            .WithSummary("Create a new product category")
            .WithDescription("Creates a new product category with the specified description.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPut("products/{id:int}", UpdateCategoryProduct)
            .WithTags("CategoryProduct")
            .WithName("UpdateCategoryProduct")
            .WithSummary("Update an existing product category")
            .WithDescription("Updates the description of an existing product category by its ID.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("products/{id:int}", DeleteCategoryProduct)
            .WithTags("CategoryProduct")
            .WithName("DeleteCategoryProduct")
            .WithSummary("Delete a product category")
            .WithDescription("Deletes an existing product category by its ID.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // ─── Category Store endpoints ───────────────────────────────────────

        group.MapGet("stores", GetAllCategoryStores)
            .WithTags("CategoryStore")
            .WithName("GetAllCategoryStores")
            .WithSummary("Get all store categories (paginated)")
            .WithDescription("Retrieves a paginated list of all store categories available in the system.")
            .Produces<PagedResponse<IEnumerable<CategoryStoreDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("stores/{id:int}", GetCategoryStoreById)
            .WithTags("CategoryStore")
            .WithName("GetCategoryStoreById")
            .WithSummary("Get a store category by ID")
            .WithDescription("Retrieves a single store category by its unique identifier.")
            .Produces<CategoryStoreDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("stores", CreateCategoryStore)
            .WithTags("CategoryStore")
            .WithName("CreateCategoryStore")
            .WithSummary("Create a new store category")
            .WithDescription("Creates a new store category with the specified description.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPut("stores/{id:int}", UpdateCategoryStore)
            .WithTags("CategoryStore")
            .WithName("UpdateCategoryStore")
            .WithSummary("Update an existing store category")
            .WithDescription("Updates the description of an existing store category by its ID.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("stores/{id:int}", DeleteCategoryStore)
            .WithTags("CategoryStore")
            .WithName("DeleteCategoryStore")
            .WithSummary("Delete a store category")
            .WithDescription("Deletes an existing store category by its ID.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    // ─── Category Product handlers ─────────────────────────────────────────

    private static async Task<IResult> GetAllCategoryProducts(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllCategoryProductsQuery(pageNumber, pageSize));
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<CategoryProductDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<CategoryProductDto>>.ErrorResponse(result.Error!.Description));
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
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Product category created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> UpdateCategoryProduct(
        int id,
        [FromBody] UpdateCategoryProductCommand command,
        IMediator mediator)
    {
        if (id != command.Id)
            return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Route ID does not match command ID."));

        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Product category updated successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> DeleteCategoryProduct(int id, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteCategoryProductCommand(id));
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Product category deleted successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }

    // ─── Category Store handlers ───────────────────────────────────────────

    private static async Task<IResult> GetAllCategoryStores(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllCategoryStoresQuery(pageNumber, pageSize));
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<CategoryStoreDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<CategoryStoreDto>>.ErrorResponse(result.Error!.Description));
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
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Store category created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> UpdateCategoryStore(
        int id,
        [FromBody] UpdateCategoryStoreCommand command,
        IMediator mediator)
    {
        if (id != command.Id)
            return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse("Route ID does not match command ID."));

        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Store category updated successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> DeleteCategoryStore(int id, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteCategoryStoreCommand(id));
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Store category deleted successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
