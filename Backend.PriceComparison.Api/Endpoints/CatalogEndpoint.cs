using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Wrappers;

namespace Backend.PriceComparison.Api.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Catalog");

        group.MapGet("catalogs", GetAllCatalogs)
            .WithName("GetAllCatalogs")
            .WithSummary("Get all catalogs (paginated)")
            .Produces<ProblemDetails>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("catalogs/{id:int}", GetCatalogById)
            .WithName("GetCatalogById")
            .WithSummary("Get a catalog item by ID")
            .Produces<ProblemDetails>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("catalogs", CreateCatalog)
            .WithName("CreateCatalog")
            .WithSummary("Create a new catalog item")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);
    }

    private static Task<IResult> GetAllCatalogs()
    {
        return Task.FromResult((IResult)TypedResults.StatusCode(StatusCodes.Status501NotImplemented));
    }

    private static Task<IResult> GetCatalogById(int id)
    {
        return Task.FromResult((IResult)TypedResults.StatusCode(StatusCodes.Status501NotImplemented));
    }

    private static Task<IResult> CreateCatalog()
    {
        return Task.FromResult((IResult)TypedResults.StatusCode(StatusCodes.Status501NotImplemented));
    }
}
