using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Application.Client.Services;
using System.Net;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Api.Endpoints;

public static class ClientEndpoints
{
    public static void MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/client")
            .WithTags("Client");

        group.MapPost("natural", CreateNaturalClient)
            .WithName("CreateNaturalClient")
            .WithSummary("Create new natural client")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("legal", CreateLegalClient)
            .WithName("CreateLegalClient")
            .WithSummary("Create new legal client")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("legal", GetAllLegal)
            .WithName("GetAllLegalClients")
            .WithSummary("Get all legal clients")
            .Produces<PagedResponse<IEnumerable<ClientDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<IEnumerable<ClientDto>>>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("natural", GetAllNatural)
            .WithName("GetAllNaturalClients")
            .WithSummary("Get all natural clients")
            .Produces<PagedResponse<IEnumerable<ClientDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<IEnumerable<ClientDto>>>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("natural/{id:int}", GetNaturalClientById)
            .WithName("GetNaturalClientById")
            .WithSummary("Get natural client by Id")
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("legal/{id:int}", GetLegalClientById)
            .WithName("GetLegalClientById")
            .WithSummary("Get legal client by Id")
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("natural/{number}/document-number", GetNaturalClientByDocumentNumber)
            .WithName("GetNaturalClientByDocumentNumber")
            .WithSummary("Get natural client by document number")
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("legal/{number}/document-number", GetLegalClientByDocumentNumber)
            .WithName("GetLegalClientByDocumentNumber")
            .WithSummary("Get legal client by document number")
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<ClientDto>>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static IResult ErrorResponse<T>(Error error)
    {
        var apiResponse = ApiResponse<T>.ErrorResponse(error.Description);
        return error.HttpStatusCode switch
        {
            HttpStatusCode.NotFound => TypedResults.NotFound(apiResponse),
            HttpStatusCode.InternalServerError => TypedResults.Json(apiResponse, statusCode: (int)HttpStatusCode.InternalServerError),
            _ => TypedResults.BadRequest(apiResponse)
        };
    }

    private static async Task<IResult> CreateNaturalClient(
        [FromBody] CreateClientNaturalPosCommand createClientCommand,
        IClientCommandService commandService)
    {
        var result = await commandService.CreateNaturalClientAsync(createClientCommand);

        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Natural client created successfully"));

        return ErrorResponse<object>(result.Error!);
    }

    private static async Task<IResult> CreateLegalClient(
        [FromBody] CreateClientLegalPosCommand createClientCommand,
        IClientCommandService commandService)
    {
        var result = await commandService.CreateLegalClientAsync(createClientCommand);

        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Legal client created successfully"));

        return ErrorResponse<object>(result.Error!);
    }

    private static async Task<IResult> GetAllLegal(
        IClientQueryService queryService,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await queryService.GetAllLegalClientsAsync(pageNumber, pageSize);

        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<ClientDto>>(result.Value!, pageNumber, pageSize));

        return ErrorResponse<IEnumerable<ClientDto>>(result.Error!);
    }

    private static async Task<IResult> GetAllNatural(
        IClientQueryService queryService,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await queryService.GetAllNaturalClientsAsync(pageNumber, pageSize);

        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<ClientDto>>(result.Value!, pageNumber, pageSize));

        return ErrorResponse<IEnumerable<ClientDto>>(result.Error!);
    }

    private static async Task<IResult> GetNaturalClientById(
        int id,
        IClientQueryService queryService)
    {
        var result = await queryService.GetClientByIdAsync(id, "natural");

        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<ClientDto>.SuccessResponse(result.Value!, "Client retrieved successfully"));

        return ErrorResponse<ClientDto>(result.Error!);
    }

    private static async Task<IResult> GetLegalClientById(
        int id,
        IClientQueryService queryService)
    {
        var result = await queryService.GetClientByIdAsync(id, "legal");

        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<ClientDto>.SuccessResponse(result.Value!, "Client retrieved successfully"));

        return ErrorResponse<ClientDto>(result.Error!);
    }

    private static async Task<IResult> GetNaturalClientByDocumentNumber(
        string number,
        IClientQueryService queryService)
    {
        var result = await queryService.GetClientByDocumentNumberAsync(number, "natural");

        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<ClientDto>.SuccessResponse(result.Value!, "Client retrieved successfully"));

        return ErrorResponse<ClientDto>(result.Error!);
    }

    private static async Task<IResult> GetLegalClientByDocumentNumber(
        string number,
        IClientQueryService queryService)
    {
        var result = await queryService.GetClientByDocumentNumberAsync(number, "legal");

        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<ClientDto>.SuccessResponse(result.Value!, "Client retrieved successfully"));

        return ErrorResponse<ClientDto>(result.Error!);
    }
}
