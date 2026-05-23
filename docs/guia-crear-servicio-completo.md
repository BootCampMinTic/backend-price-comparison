# Guia: Crear un servicio completo

Esta guia explica paso a paso como crear un servicio completo (endpoint -> handler -> mapper -> domain -> dto -> repository -> interface) usando el endpoint **Product** como ejemplo.

## Indice

1. [Estructura general](#1-estructura-general)
2. [Paso 1: Domain - Entidad](#2-paso-1-domain---entidad)
3. [Paso 2: Domain - Puerto (Interface)](#3-paso-2-domain---puerto-interface)
4. [Paso 3: Application - DTO](#4-paso-3-application---dto)
5. [Paso 4: Application - Query/Command y Handler](#5-paso-4-application---querycommand-y-handler)
6. [Paso 5: Application - Validador](#6-paso-5-application---validador)
7. [Paso 6: Application - Mapper (AutoMapper)](#7-paso-6-application---mapper-automapper)
8. [Paso 7: Infrastructure - Repositorio](#8-paso-7-infrastructure---repositorio)
9. [Paso 8: Infrastructure - Registrar en DI](#9-paso-8-infrastructure---registrar-en-di)
10. [Paso 9: API - Endpoints](#10-paso-9-api---endpoints)
11. [Paso 10: API - Registrar endpoints](#11-paso-10-api---registrar-endpoints)
12. [Paso 11: Cache - Invalidacion](#12-paso-11-cache---invalidacion)
13. [Resumen: Checklist para nueva entidad](#13-resumen-checklist-para-nueva-entidad)

---

## 1. Estructura general

`
Solution/
+-- Backend.PriceComparison.Domain/
|   +-- Store/Entities/           <- Entidades
|   +-- Ports/                    <- Interfaces (puertos)
+-- Backend.PriceComparison.Application/
|   +-- Store/Dtos/               <- DTOs
|   +-- Store/Commands/           <- Commands + Handlers + Validators
|   +-- Store/Queries/            <- Queries + Handlers
|   +-- Store/Mappers/            <- AutoMapper Profile
+-- Backend.PriceComparison.Infrastructure.Persistence.Mysql/
|   +-- Store/Repositories/       <- Implementaciones concretas
|   +-- Context/                  <- DbContext
+-- Backend.PriceComparison.Api/
    +-- Endpoints/                <- Minimal API endpoints
    +-- Program.cs                <- Punto de entrada
`

Las dependencias apuntan hacia adentro: **API -> Application -> Domain <- Infrastructure**

---

## 2. Paso 1: Domain - Entidad

La entidad es el modelo de negocio puro, sin dependencias de infraestructura.

### Ejemplo: ProductEntity.cs

**Ruta:** Backend.PriceComparison.Domain/Store/Entities/ProductEntity.cs

`csharp
namespace Backend.PriceComparison.Domain.Store.Entities;

public sealed class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    public int CategoryProductId { get; set; }
    public CategoryProductEntity? CategoryProduct { get; set; }
}
`

**Reglas:**
- sealed class - evitar herencia no deseada
- Usar = string.Empty para strings (evitar null)
- Propiedades de navegacion (Store, CategoryProduct) son 
ullable porque no siempre se incluyen
- Solo propiedades simples y relaciones - **sin logica de negocio** aqui (la logica va en handlers/domain services)

### Tambien registrar en ClientDbContext.cs

**Ruta:** Backend.PriceComparison.Infrastructure.Persistence.Mysql/Context/ClientDbContext.cs

`csharp
public DbSet<ProductEntity> Products { get; set; }

// En OnModelCreating:
modelBuilder.Entity<ProductEntity>(entity =>
{
    entity.ToTable(""product"");
    entity.Property(e => e.Id).HasColumnName(""id_product"");
    entity.Property(e => e.Name).HasColumnName(""name"");
    entity.Property(e => e.Price).HasColumnName(""price"");
    entity.Property(e => e.StoreId).HasColumnName(""id_store"");
    entity.Property(e => e.CategoryProductId).HasColumnName(""id_category_product"");
    entity.HasOne(e => e.Store).WithMany().HasForeignKey(e => e.StoreId);
    entity.HasOne(e => e.CategoryProduct).WithMany().HasForeignKey(e => e.CategoryProductId);
});
`

---

## 3. Paso 2: Domain - Puerto (Interface)

El puerto es el **contrato** que el dominio necesita. La implementacion concreta esta en Infrastructure.

### Ejemplo: IProductRepository.cs

**Ruta:** Backend.PriceComparison.Domain/Ports/IProductRepository.cs

`csharp
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Domain.Ports;

public interface IProductRepository
{
    Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Result<IEnumerable<ProductEntity>, Error>> GetByStoreAsync(int storeId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Result<ProductEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(ProductEntity entity, CancellationToken cancellationToken);
}
`

**Reglas:**
- Usar Result<TValue, TError> en lugar de excepciones para errores de negocio
- CancellationToken siempre como ultimo parametro
- Metodos asincronos retornan Task<Result<...>>
- VoidResult para operaciones que no retornan valor (create, update, delete)

---

## 4. Paso 3: Application - DTO

El DTO es lo que se expone al cliente HTTP. Nunca expongas la entidad directamente.

### Ejemplo: ProductDto.cs

**Ruta:** Backend.PriceComparison.Application/Store/Dtos/ProductDto.cs

`csharp
namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int StoreId { get; set; }
    public string? StoreName { get; set; }
    public int CategoryProductId { get; set; }
    public string? CategoryProductDescription { get; set; }
}
`

**Reglas:**
- No incluir navegaciones completas, solo los campos planos que el cliente necesita
- sealed class con propiedades publicas
- Usar string? para campos opcionales

---

## 5. Paso 4: Application - Query/Command y Handler

Separamos **lectura** (Queries) de **escritura** (Commands). Cada uno tiene su propio archivo de request y su handler.

### Query: GetAllProductsQuery.cs

**Ruta:** Backend.PriceComparison.Application/Store/Queries/Product/GetAllProductsQuery.cs

`csharp
using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

public record GetAllProductsQuery : IRequest<Result<IEnumerable<ProductDto>, Error>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
`

### Query Handler: GetAllProductsQueryHandler.cs

**Misma ruta.**

`csharp
using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

internal sealed class GetAllProductsQueryHandler(
    IProductRepository productRepository,
    ICacheService cacheService,
    IMapper mapper) : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>, Error>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IEnumerable<ProductDto>, Error>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductsPage(request.PageNumber, request.PageSize);

        var cached = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _productRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = _mapper.Map<IEnumerable<ProductDto>>(result.Value);
        await _cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);

        return dtos;
    }
}
`

### Command: CreateProductCommand.cs

**Ruta:** Backend.PriceComparison.Application/Store/Commands/CreateProduct/CreateProductCommand.cs

`csharp
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    double Price,
    int StoreId,
    int CategoryProductId) : IRequest<Result<VoidResult, Error>>;
`

### Command Handler: CreateProductCommandHandler.cs

**Misma ruta.**

`csharp
using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ICacheService cacheService) : IRequestHandler<CreateProductCommand, Result<VoidResult, Error>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<VoidResult, Error>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ProductEntity>(request);

        var result = await _productRepository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync(CacheKeys.ProductsPrefix, cancellationToken);

        return VoidResult.Instance;
    }
}
`

---

## 6. Paso 5: Application - Validador

Usamos FluentValidation para validar los commands antes de que lleguen al handler.

### Ejemplo: CreateProductCommandValidator.cs

**Ruta:** Backend.PriceComparison.Application/Store/Commands/CreateProduct/CreateProductCommandValidator.cs

`csharp
using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(""Product name is required"");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage(""Price must be greater than zero"");

        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage(""StoreId must be greater than zero"");

        RuleFor(x => x.CategoryProductId)
            .GreaterThan(0).WithMessage(""CategoryProductId must be greater than zero"");
    }
}
`

**Reglas:**
- Validar SOLO reglas de formato/obligatoriedad (no logica de negocio como ""existe en BD"")
- El validador se registra automaticamente via AddValidatorsFromAssembly
- El ValidationBehaviour<,> lo ejecuta antes del handler

---

## 7. Paso 6: Application - Mapper (AutoMapper)

El perfil de mapeo convierte Command -> Entity y Entity -> DTO.

### Editar: StoreProfile.cs

**Ruta:** Backend.PriceComparison.Application/Store/Mappers/StoreProfile.cs

`csharp
CreateMap<ProductEntity, ProductDto>()
    .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
    .ForMember(dest => dest.CategoryProductDescription, opt => opt.MapFrom(src => src.CategoryProduct != null ? src.CategoryProduct.Description : null));

CreateMap<CreateProductCommand, ProductEntity>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())          // El Id lo genera la BD
    .ForMember(dest => dest.Store, opt => opt.Ignore())       // No se mapea desde command
    .ForMember(dest => dest.CategoryProduct, opt => opt.Ignore()); // No se mapea desde command
`

**Reglas:**
- Command -> Entity: Ignorar Id (autogenerado) y navegaciones
- Entity -> DTO: Aplanar navegaciones (ej: Store.Name -> StoreName)
- Usar opt.MapFrom para transformaciones
- Usar opt.Ignore() para propiedades que no vienen del source

---

## 8. Paso 7: Infrastructure - Repositorio

Implementacion concreta del puerto usando EF Core + MySQL.

### Ejemplo: ProductRepository.cs

**Ruta:** Backend.PriceComparison.Infrastructure.Persistence.Mysql/Store/Repositories/ProductRepository.cs

`csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class ProductRepository(
    ClientDbContext context,
    ILogger<ProductRepository> logger) : IProductRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _context.Products
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.CategoryProduct)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return StoreErrorBuilder.NoRecordsFound(""product"");

        return entities;
    }

    public async Task<Result<ProductEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.CategoryProduct)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity is null)
            return StoreErrorBuilder.NotFound(id, ""Product"");

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(ProductEntity entity, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
            return StoreErrorBuilder.CreationFailed(""product"");

        return VoidResult.Instance;
    }
}
`

**Reglas:**
- AsNoTracking() en lecturas (solo consultas, no actualizaciones)
- Include() para cargar navegaciones que el DTO necesita
- Logging con ILogger<T> para depuracion
- Usar StoreErrorBuilder para errores estandar
- sealed internal class - no expuesto fuera de Infrastructure

---

## 9. Paso 8: Infrastructure - Registrar en DI

Registrar el repositorio en el contenedor de dependencias para que los handlers puedan inyectarlo.

### Editar: DependencyInjectionService.cs

**Ruta:** Backend.PriceComparison.Infrastructure.Persistence.Mysql/DependencyInjectionService.cs

`csharp
services.AddScoped<IProductRepository, ProductRepository>();
`

Usar AddScoped porque el DbContext tambien es Scoped.

---

## 10. Paso 9: API - Endpoints

Los endpoints son Minimal APIs que reciben el request HTTP, llaman al mediator y devuelven la respuesta.

### Ejemplo: ProductEndpoints.cs

**Ruta:** Backend.PriceComparison.Api/Endpoints/ProductEndpoints.cs

`csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Extensions;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.CreateProduct;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.Product;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(""api/v1"")
            .WithTags(""Product"");

        group.MapGet(""products"", GetAllProducts)
            .WithName(""GetAllProducts"")
            .WithSummary(""Get all products (paginated)"")
            .Produces<PagedResponse<IEnumerable<ProductDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet(""products/{id:int}"", GetProductById)
            .WithName(""GetProductById"")
            .WithSummary(""Get product by ID"")
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost(""products"", CreateProduct)
            .WithName(""CreateProduct"")
            .WithSummary(""Create a new product"")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllProducts(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllProductsQuery { PageNumber = pageNumber, PageSize = pageSize });
        if (result.IsSuccess)
            return TypedResults.Ok(new PagedResponse<IEnumerable<ProductDto>>(result.Value!, pageNumber, pageSize));

        return TypedResults.BadRequest(ApiResponse<IEnumerable<ProductDto>>.ErrorResponse(result.Error!.Description));
    }

    private static async Task<IResult> GetProductById(int id, IMediator mediator)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return result.Match(onSuccess => TypedResults.Ok(onSuccess));
    }

    private static async Task<IResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return TypedResults.Ok(ApiResponse<object>.SuccessResponse(new { }, ""Product created successfully""));

        return TypedResults.BadRequest(ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }
}
`

**Reglas:**
- Metodo static de extension MapXxxEndpoints(this IEndpointRouteBuilder app)
- Usar MapGroup(""api/v1"") con .WithTags(""TagName"")
- Decorar handlers con .Produces<T>() para documentacion OpenAPI
- Cada handler es static para que no necesite instancia
- Inyectar IMediator como parametro del handler
- Usar Result<T, Error>.Match() o IsSuccess para bifurcar respuesta
- Retornar TypedResults.Ok() / BadRequest()

---

## 11. Paso 10: API - Registrar endpoints

Registrar el metodo de extension en Program.cs.

### Editar: Program.cs

**Ruta:** Backend.PriceComparison.Api/Program.cs

`csharp
app.MapProductEndpoints();
`

---

## 12. Paso 11: Cache - Invalidacion

Los queries usan cache con Redis. Los commands deben invalidar las claves afectadas.

### En el handler de escritura:

`csharp
await _cacheService.RemoveByPrefixAsync(CacheKeys.ProductsPrefix, cancellationToken);
`

Esto invalida **todas** las claves que empiecen con products:* para que la proxima lectura traiga datos frescos.

### En CacheKeys.cs:

`csharp
public const string ProductsPrefix = ""products"";
public static string ProductById(int id) => $""product:{id}"";
public static string ProductsPage(int pageNumber, int pageSize) => $""{ProductsPrefix}:page:{pageNumber}:size:{pageSize}"";
`

---

## 13. Resumen: Checklist para nueva entidad

Suponiendo una entidad Category:

- [ ] **Domain**: Crear CategoryEntity.cs en Domain/Store/Entities/
- [ ] **Domain**: Crear ICategoryRepository.cs en Domain/Ports/
- [ ] **Domain**: Configurar entity en ClientDbContext.cs (tabla, columnas, FKs)
- [ ] **Application**: Crear CategoryDto.cs en Application/Store/Dtos/
- [ ] **Application**: Crear GetAllCategoriesQuery.cs + Handler
- [ ] **Application**: Crear GetCategoryByIdQuery.cs + Handler (si aplica)
- [ ] **Application**: Crear CreateCategoryCommand.cs + Handler + Validator
- [ ] **Application**: Registrar mappings en StoreProfile.cs (Command->Entity, Entity->DTO)
- [ ] **Application**: Agregar claves de cache en CacheKeys.cs
- [ ] **Infrastructure**: Crear CategoryRepository.cs en Infrastructure/.../Repositories/
- [ ] **Infrastructure**: Registrar ICategoryRepository, CategoryRepository en DependencyInjectionService.cs
- [ ] **API**: Crear CategoryEndpoints.cs en Api/Endpoints/
- [ ] **API**: Registrar pp.MapCategoryEndpoints() en Program.cs
- [ ] **Handler**: Invalidar cache en commands de escritura (RemoveByPrefixAsync)
- [ ] **Test**: dotnet build .\backend-price-comparison.sln -c Debug
- [ ] **Test**: dotnet test .\backend-price-comparison.sln -c Release

---

> **Tip:** Usa el endpoint **Product** como referencia viva. Todos los archivos estan en sus rutas correspondientes y siguen exactamente este patron.