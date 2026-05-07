using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.State;
using Backend.PriceComparison.Application.Store.Queries.TypeUser;

namespace Backend.PriceComparison.Api.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Catalog");

        group.MapGet("states", GetAllStates)
            .WithName("GetAllStates")
            .WithSummary("Get all states")
            .Produces<IEnumerable<StateDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("type-users", GetAllTypeUsers)
            .WithName("GetAllTypeUsers")
            .WithSummary("Get all user types")
            .Produces<IEnumerable<TypeUserDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllStates(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllStatesQuery());
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> GetAllTypeUsers(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllTypeUsersQuery());
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }
}
