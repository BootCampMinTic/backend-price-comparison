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
 var group = app.MapGroup("api/v1")
            .WithTags("Store");

 group.MapGet("Store", GetAllStore)
            .WithName("GetAllStore")
            .WithSummary("Get all Store (paginated)")
            .Produces<PagedResponse<IEnumerable<StoreDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

 group.MapGet("Store/{id:int}", GetStoreById)
            .WithName("GetStoreById")
            .WithSummary("Get Store by ID")
            .Produces<StoretDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

 group.MapGet("stores/{storeId:int}/Store", GetStoreByStore)
            .WithName("GetStoreByStore")
            .WithSummary("Get Store by store (paginated)")
            .Produces<PagedResponse<IEnumerable<StoreDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

group.MapPost("Store", CreateStore)
            .WithName("CreateStore")
            .WithSummary("Create a new Store")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    

    private static async Task<IResult> GetAllStore(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllStoreQuery { PageNumber = pageNumber, PageSize = pageSize });
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<StoreDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<StoreDto>>.ErrorResponse(result.Error!.Description));
    }


     private static async Task<IResult> GetStoreById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetStoreByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> GetStoreByStore(
        int storeId,
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetStoreByStoreQuery(storeId, pageNumber, pageSize));
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<StoreDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<StoreDto>>.ErrorResponse(result.Error!.Description));
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








