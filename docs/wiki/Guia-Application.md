# Guia: Capa Application

Esta guia explica como crear queries, commands, handlers y validators en la capa **Application**.

## Estructura de archivos

```
Application/
└── {Feature}/
    ├── Dtos/
    │   └── {Entity}Dto.cs
    ├── Queries/
    │   └── {Entity}/
    │       ├── GetAll{Entity}sQuery.cs
    │       └── GetAll{Entity}sQueryHandler.cs
    ├── Commands/
    │   └── Create{Entity}/
    │       ├── Create{Entity}Command.cs
    │       ├── Create{Entity}CommandHandler.cs
    │       └── Create{Entity}CommandValidator.cs
    └── Mappers/
        └── {Feature}Profile.cs
```

## Paso 1: Crear el DTO

**Ubicacion:** `Application/{Feature}/Dtos/{Entity}Dto.cs`

```csharp
namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int StoreId { get; set; }
    public string? StoreName { get; set; }         // Flattened FK
    public int CategoryProductId { get; set; }
    public string? CategoryProductDescription { get; set; } // Flattened FK
}
```

### Reglas para DTOs

| Regla | Ejemplo |
|-------|---------|
| Usar `sealed class` | `public sealed class ProductDto` |
| FK strings: aplanar a `{Entity}Name` | `StoreName` no `Store.Name` |
| Password NUNCA se incluye en DTO | `UserDto` no tiene `Password` |
| Propiedades nullable donde aplique | `string?` para datos opcionales |

## Paso 2: Crear Query + Handler

### Query (Request)

```csharp
// Application/Store/Queries/Product/GetAllProductsQuery.cs
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

public record GetAllProductsQuery : IRequest<Result<IEnumerable<ProductDto>, Error>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

### Handler

```csharp
// Application/Store/Queries/Product/GetAllProductsQueryHandler.cs
using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

public sealed class GetAllProductsQueryHandler(
    IProductRepository _productRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>, Error>>
{
    public async Task<Result<IEnumerable<ProductDto>, Error>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Intentar cache
        var cacheKey = CacheKeys.ProductsPage(request.PageNumber, request.PageSize);
        var cached = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        // 2. Consultar repositorio
        var result = await _productRepository.GetAllAsync(
            request.PageNumber, request.PageSize, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        // 3. Mapear y cachear
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);
        return dtos.ToList();
    }
}
```

### Reglas para handlers

| Regla | Ejemplo |
|-------|---------|
| `sealed` class con primary constructor | `public sealed class XxxHandler(IPort _port) : IRequestHandler<...>` |
| Inyectar solo puertos (nunca repos concreto) | `IProductRepository`, nunca `ProductRepository` |
| Cache-aside: try cache → miss → repo → set cache | Patron estandar |
| `ToList()` antes de retornar | Evita lazy evaluation |
| Si `!result.IsSuccess`, retornar `result.Error!` | Propagar errores del repo |

## Paso 3: Crear Command + Handler + Validator

### Command

```csharp
public record CreateProductCommand(
    string Name,
    double Price,
    int StoreId,
    int CategoryProductId
) : IRequest<Result<VoidResult, Error>>;
```

### Handler

```csharp
public sealed class CreateProductCommandHandler(
    IProductRepository _productRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateProductCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ProductEntity>(request);
        var result = await _productRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync(CacheKeys.ProductsPrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
```

### Reglas para command handlers

| Regla |
|-------|
| Siempre invalidar cache despues de un write exitoso |
| `RemoveByPrefixAsync` para listas, `RemoveAsync` para items individuales |
| Mapear command → entity via AutoMapper |
| Retornar `VoidResult.Instance` en exito |

### Validator

```csharp
using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StoreId).GreaterThan(0);
        RuleFor(x => x.CategoryProductId).GreaterThan(0);
    }
}
```

## Paso 4: AutoMapper Profile

```csharp
// Application/Store/Mappers/StoreProfile.cs
using AutoMapper;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Application.Store.Mappers;

public class StoreProfile : Profile
{
    public StoreProfile()
    {
        // Entity → DTO (lectura)
        CreateMap<ProductEntity, ProductDto>()
            .ForMember(dest => dest.StoreName,
                opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
            .ForMember(dest => dest.CategoryProductDescription,
                opt => opt.MapFrom(src => src.CategoryProduct != null
                    ? src.CategoryProduct.Description : null));

        // Command → Entity (escritura)
        CreateMap<CreateProductCommand, ProductEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Store, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryProduct, opt => opt.Ignore());
    }
}
```

### Registrar Profile en DI

**Archivo:** `Application/DependencyInjectionService.cs`

```csharp
var mapper = new MapperConfiguration(config =>
{
    config.AddProfile<ClientProfile>();
    config.AddProfile<StoreProfile>();   // ← Agregar aca
});
```

## Paso 5: Cache Keys

**Archivo:** `Application/Client/CacheKeys.cs`

```csharp
public static class CacheKeys
{
    // ... existing keys ...

    public const string ProductsPrefix = "products";

    public static string ProductsPage(int page, int size)
        => $"{ProductsPrefix}:page:{page}:size:{size}";

    public static string ProductById(int id)
        => $"product:{id}";
}
```

## Verificacion

- [ ] DTO creado con propiedades aplanadas
- [ ] Query + Handler con cache-aside
- [ ] Command + Handler + Validator
- [ ] Cache invalidado en command handler
- [ ] AutoMapper profile creado y registrado en DI
- [ ] CacheKeys extendido con nuevas claves
