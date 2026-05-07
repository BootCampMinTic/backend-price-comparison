using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateUser;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.User;

namespace Backend.PriceComparison.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("User");

        group.MapGet("users", GetAllUsers)
            .WithName("GetAllUsers")
            .WithSummary("Get all users")
            .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("users/{id:int}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("users", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllUsers(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllUsersQuery());
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> GetUserById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "User created successfully"));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
