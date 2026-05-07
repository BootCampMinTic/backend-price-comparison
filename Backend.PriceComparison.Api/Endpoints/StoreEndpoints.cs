using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateStore;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.Store;
using System.Net;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Api.Endpoints;

public static class StoreEndpoints
{
    public static void MapStoreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Store");

        group.MapGet("stores", GetAllStores)
            .WithName("GetAllStores")
            .WithSummary("Get all stores (paginated)")
            .Produces<PagedResponse<IEnumerable<StoreDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("stores/{id:int}", GetStoreById)
            .WithName("GetStoreById")
            .WithSummary("Get store by ID")
            .Produces<StoreDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("stores", CreateStore)
            .WithName("CreateStore")
            .WithSummary("Create a new store")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllStores(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllStoresQuery { PageNumber = pageNumber, PageSize = pageSize });
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<StoreDto>>(result.Value!, pageNumber, pageSize));

        return result.Error!.HttpStatusCode switch
        {
            HttpStatusCode.NotFound => TypedResults.NotFound(ApiResponse<IEnumerable<StoreDto>>.ErrorResponse(result.Error.Description)),
            _ => TypedResults.BadRequest(ApiResponse<IEnumerable<StoreDto>>.ErrorResponse(result.Error.Description))
        };
    }

    private static async Task<IResult> GetStoreById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetStoreByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> CreateStore(
        [FromBody] CreateStoreCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Store created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
