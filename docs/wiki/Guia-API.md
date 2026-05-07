# Guia: Capa API

Esta guia explica como crear endpoints Minimal API.

## Paso 1: Crear el archivo de endpoints

**Ubicacion:** `Backend.PriceComparison.Api/Endpoints/{Feature}Endpoints.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateProduct;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.Product;

namespace Backend.PriceComparison.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1")
            .WithTags("Product");       // ← Tag para Swagger/Scalar

        // GET all (paginado)
        group.MapGet("products", GetAllProducts)
            .WithName("GetAllProducts")
            .WithSummary("Get all products (paginated)")
            .Produces<PagedResponse<IEnumerable<ProductDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET by id
        group.MapGet("products/{id:int}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Get product by ID")
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST create
        group.MapPost("products", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    // GET all
    private static async Task<IResult> GetAllProducts(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(
            new GetAllProductsQuery { PageNumber = pageNumber, PageSize = pageSize });

        if (result.IsSuccess)
            return TypedResults.Ok(
                new PagedResponse<IEnumerable<ProductDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(
            ApiResponse<IEnumerable<ProductDto>>.ErrorResponse(result.Error!.Description));
    }

    // GET by id
    private static async Task<IResult> GetProductById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    // POST
    private static async Task<IResult> CreateProduct(
        [FromBody] CreateProductCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(
                ApiResponse<object>.SuccessResponse(new { }, "Product created successfully"));

        return TypedResults.BadRequest(
            ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
```

## Paso 2: Registrar en Program.cs

```csharp
// En Program.cs, despues de los endpoints existentes:
app.MapClientEndpoints();
app.MapDocumentTypeEndpoints();
app.MapProductEndpoints();    // ← Nuevo
app.MapStoreEndpoints();      // ← Nuevo
```

## Reglas para endpoints

| Regla | Ejemplo |
|-------|---------|
| Clase `static` con metodo de extension | `public static void MapProductEndpoints(this IEndpointRouteBuilder app)` |
| Usar `MapGroup("api/v1")` con `WithTags` | Agrupa en Scalar UI |
| `WithName` + `WithSummary` en cada endpoint | Documentacion OpenAPI |
| `Produces<T>` para 200 | `Produces<ProductDto>(200)` |
| `Produces<ProblemDetails>` para 401, 500 | Errores de infraestructura |
| `Produces<ApiResponse<T>>(400)` para errores de negocio | Validation errors |
| Inyectar `IMediator` (no servicios directos) | Patron simple |
| `PagedResponse<T>` para listas paginadas | Envuelve data con metadata |
| `ApiResponse<T>.SuccessResponse` para POST | Mensaje de exito |
| `ApiResponse<T>.ErrorResponse` para errores | Mensaje de error |
| `result.Match(onSuccess: ...)` en GetById | Extension helper del proyecto |

## Tipos de respuesta

### Para GET (lista paginada)
```csharp
return TypedResults.Ok(new PagedResponse<IEnumerable<XxxDto>>(data, page, size));
```

### Para GET by id
```csharp
return result.Match(onSuccess => TypedResults.Ok(onSuccess));
```

### Para POST (crear)
```csharp
return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, "Created successfully"));
```

### Para errores
```csharp
return TypedResults.BadRequest(ApiResponse<T>.ErrorResponse(errorMessage));
```

## Autenticacion

- Endpoints bajo `api/v1/` requieren `Authorization: Bearer <token>`
- El token no se valida realmente (solo se verifica que exista)
- Los endpoints de `/health`, `/scalar`, `/openapi` son publicos

## Verificacion

- [ ] Clase `static` con metodo de extension `Map{Feature}Endpoints`
- [ ] `MapGroup("api/v1").WithTags("{Tag}")`
- [ ] `WithName` + `WithSummary` en todos los endpoints
- [ ] `Produces<T>` correcto para cada status code
- [ ] `IMediator` inyectado directamente
- [ ] `PagedResponse` para listas paginadas
- [ ] Registrado en `Program.cs`
