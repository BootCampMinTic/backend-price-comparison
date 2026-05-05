using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Application.Client.Queries.DocumentType;

namespace Backend.PriceComparison.Api.Endpoints;

public static class DocumentTypeEndpoints
{
    public static void MapDocumentTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/client")
            .WithTags("Client");

        group.MapGet("document-type", GetAllDocumentTypes)
            .WithName("GetAllDocumentTypes")
            .WithSummary("Get all document types")
            .Produces<ApiResponseDto<IEnumerable<DocumentTypeDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllDocumentTypes(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllDocumentTypeQuery());
        return result.Match(
            onSuccess => TypedResults.Ok(onSuccess),
            onFailure => TypedResults.BadRequest(onFailure)
        );
    }
}
