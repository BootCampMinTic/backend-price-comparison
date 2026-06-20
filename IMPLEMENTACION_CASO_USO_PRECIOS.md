# 📋 Implementación del Caso de Uso: "Registrar y Consultar Precios de Productos en Diferentes Supermercados"

## 🏗️ Arquitectura: Clean Architecture + CQRS con MediatR

Este documento describe la implementación completa del caso de uso siguiendo principios SOLID, Clean Code y Clean Architecture, estructurado en capas con separación de responsabilidades.

---

## 📚 ÍNDICE

1. [Capa Domain](#capa-domain)
   - Entidades
   - Interfaces de Puertos
2. [Capa Application](#capa-application)
   - DTOs
   - Validators
   - Commands y Handlers
   - Queries y Handlers
   - Mappers
3. [Capa Infrastructure](#capa-infrastructure)
   - Implementación de Repositorios
4. [Capa API](#capa-api)
   - Endpoints HTTP (Minimal API)
5. [Configuración e Inyección de Dependencias](#configuración)
6. [Flujo Completo de Ejecución](#flujo)

---

## 🔵 CAPA DOMAIN

### Responsabilidad
La capa Domain contiene la lógica de negocio pura, sin dependencias externas. Define las entidades, reglas de negocio y contratos (interfaces) que el resto del sistema debe cumplir.

### Ubicación: `Backend.PriceComparison.Domain/Store/Entities/`

#### 1️⃣ **ProductEntity.cs** - Entidad de Dominio para Productos
```csharp
namespace Backend.PriceComparison.Domain.Store.Entities;

/// <summary>
/// Representa un producto disponible en la plataforma.
/// Encapsula los datos y la identidad del producto.
/// </summary>
public sealed class ProductEntity
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre descriptivo del producto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Precio base del producto (puede variar por supermercado).
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Identificador del supermercado donde se registró inicialmente.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Navegación a la entidad Store relacionada (relación de clave foránea).
    /// </summary>
    public StoreEntity? Store { get; set; }

    /// <summary>
    /// Identificador de la categoría del producto.
    /// </summary>
    public int CategoryProductId { get; set; }

    /// <summary>
    /// Navegación a la entidad CategoryProduct relacionada.
    /// </summary>
    public CategoryProductEntity? CategoryProduct { get; set; }
}
```

#### 2️⃣ **StoreEntity.cs** - Entidad de Dominio para Supermercados
```csharp
namespace Backend.PriceComparison.Domain.Store.Entities;

/// <summary>
/// Representa un supermercado o tienda en la plataforma.
/// Encapsula la información del negocio de venta al por menor.
/// </summary>
public sealed class StoreEntity
{
    /// <summary>
    /// Identificador único del supermercado.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre comercial del supermercado.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Dirección física del supermercado.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Número de contacto del supermercado (opcional).
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Identificador de la categoría de supermercado (ej: Cadena Nacional, Local).
    /// </summary>
    public int CategoryStoreId { get; set; }

    /// <summary>
    /// Navegación a la entidad CategoryStore relacionada.
    /// </summary>
    public CategoryStoreEntity? CategoryStore { get; set; }
}
```

#### 3️⃣ **PriceHistoryEntity.cs** - Entidad de Dominio para Historial de Precios
```csharp
namespace Backend.PriceComparison.Domain.Store.Entities;

/// <summary>
/// Representa el historial de precios de un producto en un supermercado específico.
/// Esta es la entidad central del caso de uso: permite registrar y comparar precios.
/// 
/// Patrón: AGGREGATE ROOT
/// - La entidad agrupa el precio de un producto en una tienda en un momento específico.
/// - Permite construir históricos y comparativas de precios.
/// </summary>
public sealed class PriceHistoryEntity
{
    /// <summary>
    /// Identificador único del registro de historial de precio.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto asociado (FK).
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Navegación a la entidad Product relacionada.
    /// </summary>
    public ProductEntity? Product { get; set; }

    /// <summary>
    /// Identificador del supermercado (tienda) asociado (FK).
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Navegación a la entidad Store relacionada.
    /// </summary>
    public StoreEntity? Store { get; set; }

    /// <summary>
    /// Precio registrado para el producto en este supermercado en la fecha especificada.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Fecha y hora UTC en la que se registró el precio.
    /// </summary>
    public DateTime Date { get; set; }
}
```

### Ubicación: `Backend.PriceComparison.Domain/Ports/`

#### 4️⃣ **IPriceHistoryRepository.cs** - Contrato del Repositorio de Historial de Precios
```csharp
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

/// <summary>
/// Puerto (Interfaz) que define el contrato para acceder a datos de historial de precios.
/// 
/// Principio: DEPENDENCY INVERSION
/// - La capa Domain define el contrato (abstracción).
/// - La capa Infrastructure implementa este contrato (adaptador concreto).
/// - La capa Application depende de la abstracción, no de la implementación.
/// </summary>
public interface IPriceHistoryRepository
{
    /// <summary>
    /// Obtiene la lista de registros de precios de un producto en todos los supermercados.
    /// 
    /// Patrón: QUERY
    /// - Lee del repositorio sin modificar estado.
    /// - Incluye relaciones necesarias (Product, Store).
    /// - Resultados ordenados de menor a mayor precio para fácil comparación.
    /// </summary>
    /// <param name="productId">Identificador del producto a consultar.</param>
    /// <param name="cancellationToken">Token para cancelación de operaciones asincrónicas.</param>
    /// <returns>
    /// Result<IEnumerable<PriceHistoryEntity>, Error>
    /// - Si éxito: Colección de registros de historial.
    /// - Si fallo: Error descriptivo.
    /// </returns>
    Task<Result<IEnumerable<PriceHistoryEntity>, Error>> GetPricesByProductAsync(
        int productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Registra (persiste) un nuevo entrada de precio en el repositorio.
    /// 
    /// Patrón: COMMAND
    /// - Modifica el estado del sistema.
    /// - Realiza validaciones y persistencia.
    /// - Usa Pattern Result<VoidResult, Error> para manejo de errores sin excepciones.
    /// </summary>
    /// <param name="entity">Entidad de historial de precio a persistir.</param>
    /// <param name="cancellationToken">Token para cancelación de operaciones asincrónicas.</param>
    /// <returns>
    /// Result<VoidResult, Error>
    /// - Si éxito: VoidResult.Instance (operación completada).
    /// - Si fallo: Error descriptivo sin lanzar excepción.
    /// </returns>
    Task<Result<VoidResult, Error>> CreateAsync(
        PriceHistoryEntity entity,
        CancellationToken cancellationToken);
}
```

#### 5️⃣ **IProductRepository.cs** - Contrato para Acceso a Productos
```csharp
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

/// <summary>
/// Puerto que define el contrato para acceso a datos de productos.
/// Se utiliza para validar la existencia de productos antes de registrar precios.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Obtiene todos los productos con paginación.
    /// </summary>
    Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Obtiene los productos de un supermercado específico.
    /// </summary>
    Task<Result<IEnumerable<ProductEntity>, Error>> GetByStoreAsync(
        int storeId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Obtiene un producto específico por su ID.
    /// </summary>
    Task<Result<ProductEntity, Error>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    /// <summary>
    /// Crea (persiste) un nuevo producto.
    /// </summary>
    Task<Result<VoidResult, Error>> CreateAsync(
        ProductEntity entity,
        CancellationToken cancellationToken);
}
```

#### 6️⃣ **IStoreRepository.cs** - Contrato para Acceso a Supermercados
```csharp
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

/// <summary>
/// Puerto que define el contrato para acceso a datos de supermercados.
/// Se utiliza para validar la existencia de supermercados antes de registrar precios.
/// </summary>
public interface IStoreRepository
{
    /// <summary>
    /// Obtiene todos los supermercados con paginación.
    /// </summary>
    Task<Result<IEnumerable<StoreEntity>, Error>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Obtiene un supermercado específico por su ID.
    /// </summary>
    Task<Result<StoreEntity, Error>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    /// <summary>
    /// Crea (persiste) un nuevo supermercado.
    /// </summary>
    Task<Result<VoidResult, Error>> CreateAsync(
        StoreEntity entity,
        CancellationToken cancellationToken);
}
```

---

## 🟢 CAPA APPLICATION

### Responsabilidad
Orquesta la lógica de aplicación, implementa CQRS (Command Query Responsibility Segregation), maneja validaciones y coordina la comunicación entre Domain e Infrastructure. No contiene lógica de negocio directa.

### Ubicación: `Backend.PriceComparison.Application/Store/`

#### 7️⃣ **PriceHistoryDto.cs** - Data Transfer Object (Lectura)
Ubicación: `Backend.PriceComparison.Application/Store/Dtos/`

```csharp
namespace Backend.PriceComparison.Application.Store.Dtos;

/// <summary>
/// DTO (Data Transfer Object) para representar el historial de precios en respuestas.
/// 
/// Propósito: Evitar exponer las entidades de dominio directamente en respuestas HTTP.
/// - Encapsula solo los datos necesarios.
/// - Proporciona nombres amigables para la API.
/// - Protege la estructura interna del dominio contra cambios externos.
/// </summary>
public sealed class PriceHistoryDto
{
    /// <summary>
    /// Identificador único del registro.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Nombre del producto (denormalizado para evitar navegación).
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Identificador del supermercado.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Nombre del supermercado.
    /// </summary>
    public string? StoreName { get; set; }

    /// <summary>
    /// Precio del producto en este supermercado.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Fecha de registro del precio.
    /// </summary>
    public DateTime Date { get; set; }
}
```

#### 8️⃣ **RegisterPriceCommand.cs** - Comando CQRS para Registrar Precio
Ubicación: `Backend.PriceComparison.Application/Store/Commands/RegisterPrice/`

```csharp
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.RegisterPrice;

/// <summary>
/// Comando CQRS que representa la intención de registrar el precio de un producto en un supermercado.
/// 
/// Patrón: COMMAND (escritura/cambio de estado)
/// - Contiene los datos necesarios para la operación.
/// - Es un record (inmutable) para garantizar integridad de datos.
/// - Implementa IRequest para integración con MediatR.
/// </summary>
public record RegisterPriceCommand(
    /// <summary>Identificador del producto</summary>
    int ProductId,
    
    /// <summary>Identificador del supermercado</summary>
    int StoreId,
    
    /// <summary>Precio a registrar (debe ser mayor a 0)</summary>
    double Price,
    
    /// <summary>Fecha del registro (no puede ser futura)</summary>
    DateTime Date
) : IRequest<Result<VoidResult, Error>>;
```

#### 9️⃣ **RegisterPriceCommandValidator.cs** - Validación con FluentValidation
Ubicación: `Backend.PriceComparison.Application/Store/Commands/RegisterPrice/`

```csharp
using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.RegisterPrice;

/// <summary>
/// Validador para el comando RegisterPriceCommand.
/// 
/// Patrón: VALIDATION (reglas de negocio de entrada)
/// - Define reglas de validación de datos de entrada.
/// - Se ejecuta automáticamente vía ValidationBehaviour<,> (middleware de MediatR).
/// - Usa FluentValidation para expresividad clara.
/// 
/// Principio: SEPARATION OF CONCERNS
/// - Las validaciones están separadas de la lógica del manejador.
/// - Reutilizables y fáciles de testear.
/// </summary>
public class RegisterPriceCommandValidator : AbstractValidator<RegisterPriceCommand>
{
    public RegisterPriceCommandValidator()
    {
        // ProductId: Debe ser un identificador válido (mayor a 0)
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("El identificador del producto debe ser mayor a cero.");

        // StoreId: Debe ser un identificador válido (mayor a 0)
        RuleFor(x => x.StoreId)
            .GreaterThan(0)
            .WithMessage("El identificador del supermercado debe ser mayor a cero.");

        // Price: Debe ser un valor positivo
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("El precio debe ser mayor a cero.");

        // Date: No puede estar vacía y no puede ser una fecha futura
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("La fecha del registro no puede estar vacía.")
            .LessThanOrEqualTo(x => DateTime.UtcNow)
            .WithMessage("La fecha del registro no puede ser en el futuro.");
    }
}
```

#### 🔟 **RegisterPriceCommandHandler.cs** - Manejador del Comando
Ubicación: `Backend.PriceComparison.Application/Store/Commands/RegisterPrice/`

```csharp
using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.RegisterPrice;

/// <summary>
/// Manejador CQRS que procesa el comando de registro de precio.
/// 
/// Patrón: COMMAND HANDLER (procesamiento de escritura)
/// - Orquesta la lógica de aplicación.
/// - Valida precondiciones de negocio.
/// - Persiste cambios mediante repositorio.
/// - Invalida cachés si es necesario.
/// - Retorna Result<VoidResult, Error> sin lanzar excepciones.
/// </summary>
public sealed class RegisterPriceCommandHandler(
    /// <summary>Repositorio para acceso a historial de precios</summary>
    IPriceHistoryRepository _priceHistoryRepository,
    
    /// <summary>Repositorio para validar existencia de producto</summary>
    IProductRepository _productRepository,
    
    /// <summary>Repositorio para validar existencia de supermercado</summary>
    IStoreRepository _storeRepository,
    
    /// <summary>Mapeador de AutoMapper para transformaciones</summary>
    IMapper _mapper,
    
    /// <summary>Servicio de caché para invalidaciones</summary>
    ICacheService _cacheService)
    : IRequestHandler<RegisterPriceCommand, Result<VoidResult, Error>>
{
    /// <summary>
    /// Procesa el comando validando existencia de producto y supermercado,
    /// creando el registro de historial y invalidando cachés relevantes.
    /// </summary>
    public async Task<Result<VoidResult, Error>> Handle(
        RegisterPriceCommand request,
        CancellationToken cancellationToken)
    {
        // Paso 1: Validar que el producto existe
        var productResult = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);
        
        if (!productResult.IsSuccess)
        {
            return productResult.Error!;
        }

        // Paso 2: Validar que el supermercado existe
        var storeResult = await _storeRepository.GetByIdAsync(
            request.StoreId,
            cancellationToken);
        
        if (!storeResult.IsSuccess)
        {
            return storeResult.Error!;
        }

        // Paso 3: Mapear comando a entidad de dominio
        var entity = _mapper.Map<PriceHistoryEntity>(request);

        // Paso 4: Persistir el nuevo registro de precio
        var createResult = await _priceHistoryRepository.CreateAsync(
            entity,
            cancellationToken);

        if (!createResult.IsSuccess)
        {
            return createResult.Error!;
        }

        // Paso 5: Invalidar caché del comparador para este producto
        // (fuerza recarga en próxima consulta)
        await _cacheService.RemoveByPrefixAsync(
            $"price_history_{request.ProductId}",
            cancellationToken);

        return VoidResult.Instance;
    }
}
```

#### 1️⃣1️⃣ **GetPriceComparisonByProductQuery.cs** - Consulta CQRS para Comparar Precios
Ubicación: `Backend.PriceComparison.Application/Store/Queries/GetPriceComparisonByProduct/`

```csharp
using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Queries.GetPriceComparisonByProduct;

/// <summary>
/// Consulta CQRS que representa la intención de obtener la comparación de precios
/// de un producto en diferentes supermercados.
/// 
/// Patrón: QUERY (lectura sin cambio de estado)
/// - Contiene los parámetros de filtrado necesarios.
/// - Es un record inmutable para integridad de datos.
/// - Implementa IRequest para integración con MediatR.
/// </summary>
public record GetPriceComparisonByProductQuery(
    /// <summary>Identificador del producto a comparar</summary>
    int ProductId
) : IRequest<Result<IEnumerable<PriceHistoryDto>, Error>>;
```

#### 1️⃣2️⃣ **GetPriceComparisonByProductQueryHandler.cs** - Manejador de la Consulta
Ubicación: `Backend.PriceComparison.Application/Store/Queries/GetPriceComparisonByProduct/`

```csharp
using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.GetPriceComparisonByProduct;

/// <summary>
/// Manejador CQRS que procesa la consulta de comparación de precios.
/// 
/// Patrón: QUERY HANDLER (lectura optimizada)
/// - Implementa lectura desde caché distribuida (Redis) para rendimiento.
/// - Si no está en caché, consulta repositorio y cachea resultado.
/// - No modifica estado del sistema.
/// - Usa TTL (Time To Live) para invalidación automática de caché.
/// 
/// Principio: CQRS (Command Query Responsibility Segregation)
/// - Separación clara entre escritura (Command) y lectura (Query).
/// - Permite optimizaciones independientes para cada patrón.
/// </summary>
public sealed class GetPriceComparisonByProductQueryHandler(
    /// <summary>Repositorio para obtener datos de historial de precios</summary>
    IPriceHistoryRepository _priceHistoryRepository,
    
    /// <summary>Mapeador AutoMapper para transformaciones</summary>
    IMapper _mapper,
    
    /// <summary>Servicio de caché para optimización de lectura</summary>
    ICacheService _cacheService)
    : IRequestHandler<GetPriceComparisonByProductQuery, Result<IEnumerable<PriceHistoryDto>, Error>>
{
    /// <summary>
    /// Procesa la consulta intentando caché primero, luego repositorio,
    /// y finalmente almacena resultado en caché.
    /// </summary>
    public async Task<Result<IEnumerable<PriceHistoryDto>, Error>> Handle(
        GetPriceComparisonByProductQuery request,
        CancellationToken cancellationToken)
    {
        // Paso 1: Construir clave de caché
        var cacheKey = $"price_history_{request.ProductId}";

        // Paso 2: Intentar obtener desde caché
        var cachedData = await _cacheService.GetAsync<IEnumerable<PriceHistoryDto>>(
            cacheKey,
            cancellationToken);
        
        if (cachedData is not null)
        {
            return Result<IEnumerable<PriceHistoryDto>, Error>.Success(cachedData);
        }

        // Paso 3: Si no está en caché, consultar repositorio
        var repositoryResult = await _priceHistoryRepository.GetPricesByProductAsync(
            request.ProductId,
            cancellationToken);
        
        if (!repositoryResult.IsSuccess)
        {
            return repositoryResult.Error!;
        }

        // Paso 4: Mapear entidades a DTOs
        var dtos = _mapper.Map<IEnumerable<PriceHistoryDto>>(repositoryResult.Value);

        // Paso 5: Almacenar en caché con TTL de 10 minutos
        await _cacheService.SetAsync(
            cacheKey,
            dtos,
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Result<IEnumerable<PriceHistoryDto>, Error>.Success(dtos);
    }
}
```

#### 1️⃣3️⃣ **StoreProfile.cs** - Configuración de AutoMapper
Ubicación: `Backend.PriceComparison.Application/Store/Mappers/`

```csharp
using AutoMapper;
using Backend.PriceComparison.Application.Store.Commands.RegisterPrice;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Application.Store.Mappers;

/// <summary>
/// Perfil de AutoMapper que define las reglas de transformación entre entidades y DTOs.
/// 
/// Patrón: MAPPER (transformación de datos)
/// - Encapsula reglas de mapeo de forma centralizada.
/// - Reutilizable en aplicación.
/// - Fácil de mantener y actualizar.
/// </summary>
public class StoreProfile : Profile
{
    public StoreProfile()
    {
        // ============== MAPEOS PRINCIPALES PARA ESTE CASO DE USO ==============

        // Mapeo: PriceHistoryEntity -> PriceHistoryDto
        // Denormaliza datos de productos y supermercados para respuesta HTTP
        CreateMap<PriceHistoryEntity, PriceHistoryDto>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.StoreName,
                opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null));

        // Mapeo: RegisterPriceCommand -> PriceHistoryEntity
        // Transforma comando en entidad de dominio, ignorando navegación e ID
        CreateMap<RegisterPriceCommand, PriceHistoryEntity>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.Product,
                opt => opt.Ignore())
            .ForMember(dest => dest.Store,
                opt => opt.Ignore());

        // ============== MAPEOS ADICIONALES DE APOYO ==============
        // (Reutilizados en otros casos de uso del módulo Store)

        CreateMap<StoreEntity, StoreDto>()
            .ForMember(dest => dest.CategoryStoreDescription,
                opt => opt.MapFrom(src => src.CategoryStore != null
                    ? src.CategoryStore.Description
                    : null));

        CreateMap<ProductEntity, ProductDto>()
            .ForMember(dest => dest.StoreName,
                opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
            .ForMember(dest => dest.CategoryProductDescription,
                opt => opt.MapFrom(src => src.CategoryProduct != null
                    ? src.CategoryProduct.Description
                    : null));
    }
}
```

---

## 🔴 CAPA INFRASTRUCTURE

### Responsabilidad
Implementa los puertos definidos en Domain. Contiene adaptadores concretos para persistencia (bases de datos), cachés, proveedores de mensajes, etc.

### Ubicación: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Store/Repositories/`

#### 1️⃣4️⃣ **PriceHistoryRepository.cs** - Implementación del Repositorio de Precios
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

/// <summary>
/// Implementación concreta del repositorio IPriceHistoryRepository usando Entity Framework Core.
/// 
/// Patrón: REPOSITORY (abstracción de persistencia)
/// - Implementa el contrato definido en la capa Domain.
/// - Encapsula la complejidad de acceso a datos con EF Core.
/// - Proporciona métodos específicos para consultas comunes.
/// 
/// Principio: DEPENDENCY INVERSION + SINGLE RESPONSIBILITY
/// - El repositorio solo se preocupa por persistencia.
/// - La lógica de negocio está en Handlers (Application).
/// - Domain no depende de EF Core, solo el Repository lo hace.
/// </summary>
internal sealed class PriceHistoryRepository(
    /// <summary>Contexto de Entity Framework Core para acceso a MySQL</summary>
    ClientDbContext context,
    
    /// <summary>Logger para auditoría y diagnóstico</summary>
    ILogger<PriceHistoryRepository> logger)
    : IPriceHistoryRepository
{
    private readonly ClientDbContext _context = context;

    /// <summary>
    /// Obtiene el historial de precios de un producto, cargando relaciones y ordenando por precio.
    /// </summary>
    public async Task<Result<IEnumerable<PriceHistoryEntity>, Error>> GetPricesByProductAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        try
        {
            // Consulta LINQ optimizada con Include() para evitar N+1
            var entities = await _context.Set<PriceHistoryEntity>()
                .AsNoTracking()  // Optimización: no rastrear cambios (solo lectura)
                .Include(ph => ph.Product)  // Cargar producto para nombre
                .Include(ph => ph.Store)    // Cargar supermercado para nombre
                .Where(ph => ph.ProductId == productId)
                .OrderBy(ph => ph.Price)  // Ordenar de menor a mayor para fácil comparación
                .ToListAsync(cancellationToken);

            if (entities.Count == 0)
            {
                logger.LogDebug(
                    "No se encontró historial de precios para producto ID: {ProductId}",
                    productId);
                
                return StoreErrorBuilder.NoRecordsFound("historial de precios");
            }

            logger.LogInformation(
                "Se recuperaron {Count} registros de precio para producto ID: {ProductId}",
                entities.Count,
                productId);

            return entities;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al consultar historial de precios para producto ID: {ProductId}",
                productId);

            return StoreErrorBuilder.DatabaseError("al consultar historial de precios");
        }
    }

    /// <summary>
    /// Persiste un nuevo registro de historial de precios en la base de datos.
    /// </summary>
    public async Task<Result<VoidResult, Error>> CreateAsync(
        PriceHistoryEntity entity,
        CancellationToken cancellationToken)
    {
        try
        {
            // Agregar entidad al contexto
            await _context.Set<PriceHistoryEntity>()
                .AddAsync(entity, cancellationToken);

            // Persistir cambios en MySQL
            var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!saved)
            {
                logger.LogWarning(
                    "Fallo al persistir registro de precio. Producto ID: {ProductId}, Tienda ID: {StoreId}",
                    entity.ProductId,
                    entity.StoreId);

                return StoreErrorBuilder.CreationFailed("historial de precios");
            }

            logger.LogInformation(
                "Historial de precio registrado exitosamente. ID: {PriceHistoryId}, Producto: {ProductId}, Tienda: {StoreId}",
                entity.Id,
                entity.ProductId,
                entity.StoreId);

            return VoidResult.Instance;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al crear registro de precio. Producto ID: {ProductId}, Tienda ID: {StoreId}",
                entity.ProductId,
                entity.StoreId);

            return StoreErrorBuilder.DatabaseError("al registrar el precio");
        }
    }
}
```

#### 1️⃣5️⃣ **Otros Repositorios Necesarios**

Ubicación: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Store/Repositories/`

**ProductRepository.cs** - Para validar existencia de productos:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

/// <summary>
/// Implementación del repositorio de productos usando Entity Framework Core.
/// Se utiliza para validar la existencia de productos antes de registrar precios.
/// </summary>
internal sealed class ProductRepository(
    ClientDbContext context,
    ILogger<ProductRepository> logger)
    : IProductRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var products = await _context.Set<ProductEntity>()
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (products.Count == 0)
        {
            logger.LogDebug("No se encontraron productos.");
            return StoreErrorBuilder.NoRecordsFound("productos");
        }

        return products;
    }

    public async Task<Result<IEnumerable<ProductEntity>, Error>> GetByStoreAsync(
        int storeId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var products = await _context.Set<ProductEntity>()
            .AsNoTracking()
            .Where(p => p.StoreId == storeId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (products.Count == 0)
        {
            logger.LogDebug("No se encontraron productos para la tienda ID: {StoreId}", storeId);
            return StoreErrorBuilder.NoRecordsFound("productos");
        }

        return products;
    }

    public async Task<Result<ProductEntity, Error>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _context.Set<ProductEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            logger.LogDebug("Producto no encontrado. ID: {ProductId}", id);
            return StoreErrorBuilder.NotFound("Producto", id);
        }

        return product;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(
        ProductEntity entity,
        CancellationToken cancellationToken)
    {
        await _context.Set<ProductEntity>().AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Fallo al crear producto: {ProductName}", entity.Name);
            return StoreErrorBuilder.CreationFailed("producto");
        }

        return VoidResult.Instance;
    }
}
```

**StoreRepository.cs** - Para validar existencia de supermercados:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

/// <summary>
/// Implementación del repositorio de supermercados usando Entity Framework Core.
/// Se utiliza para validar la existencia de supermercados antes de registrar precios.
/// </summary>
internal sealed class StoreRepository(
    ClientDbContext context,
    ILogger<StoreRepository> logger)
    : IStoreRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<StoreEntity>, Error>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var stores = await _context.Set<StoreEntity>()
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (stores.Count == 0)
        {
            logger.LogDebug("No se encontraron supermercados.");
            return StoreErrorBuilder.NoRecordsFound("supermercados");
        }

        return stores;
    }

    public async Task<Result<StoreEntity, Error>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var store = await _context.Set<StoreEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (store is null)
        {
            logger.LogDebug("Supermercado no encontrado. ID: {StoreId}", id);
            return StoreErrorBuilder.NotFound("Supermercado", id);
        }

        return store;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(
        StoreEntity entity,
        CancellationToken cancellationToken)
    {
        await _context.Set<StoreEntity>().AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Fallo al crear supermercado: {StoreName}", entity.Name);
            return StoreErrorBuilder.CreationFailed("supermercado");
        }

        return VoidResult.Instance;
    }
}
```

---

## 🟡 CAPA API

### Responsabilidad
Expone los servicios de aplicación a través de endpoints HTTP. Traduce solicitudes HTTP en Commands/Queries y respuestas en DTOs, sin contener lógica de negocio.

### Ubicación: `Backend.PriceComparison.Api/Endpoints/`

#### 1️⃣6️⃣ **PriceHistoryEndpoints.cs** - Definición de Endpoints HTTP
```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Store.Commands.RegisterPrice;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Application.Store.Queries.GetPriceComparisonByProduct;

namespace Backend.PriceComparison.Api.Endpoints;

/// <summary>
/// Define los endpoints HTTP (Minimal API) para el caso de uso de
/// registro y comparación de precios de productos en supermercados.
/// 
/// Patrón: ENDPOINTS (mapeo de rutas HTTP)
/// - Mapea solicitudes HTTP a Commands/Queries.
/// - Traduce respuestas de aplicación a DTOs.
/// - No contiene lógica de negocio.
/// 
/// Principio: SEPARATION OF CONCERNS
/// - Los endpoints solo orquestan, no implementan lógica.
/// - MediatR inyectado para envío de comandos/consultas.
/// </summary>
public static class PriceHistoryEndpoints
{
    /// <summary>
    /// Mapea las rutas HTTP para el caso de uso de precios.
    /// Llamado desde Program.cs en configuración de startup.
    /// </summary>
    /// <param name="app">Constructor de rutas del endpoint.</param>
    public static void MapPriceHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        // Crear grupo de rutas bajo /api/v1 con etiqueta para documentación
        var group = app.MapGroup("api/v1")
            .WithTags("PriceHistory");

        // ============== ENDPOINT 1: REGISTRAR PRECIO ==============
        group.MapPost("prices", RegisterPrice)
            .WithName("RegisterPrice")
            .WithSummary("Registrar el precio de un producto en un supermercado específico")
            .WithDescription(
                "Registra un nuevo precio para un producto en un supermercado con fecha. " +
                "Valida existencia de producto y supermercado antes de registrar.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // ============== ENDPOINT 2: COMPARAR PRECIOS ==============
        group.MapGet("products/{productId:int}/prices", GetPriceComparison)
            .WithName("GetPriceComparison")
            .WithSummary("Consultar la comparación de precios de un producto en diferentes supermercados")
            .WithDescription(
                "Obtiene el historial de precios de un producto en todos los supermercados. " +
                "Resultados cacheados por 10 minutos para optimización.")
            .Produces<ApiResponse<IEnumerable<PriceHistoryDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<IEnumerable<PriceHistoryDto>>>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Endpoint HTTP POST /api/v1/prices
    /// Registra un nuevo precio de producto en un supermercado.
    /// </summary>
    /// <param name="command">Comando con datos del precio a registrar (ProductId, StoreId, Price, Date).</param>
    /// <param name="mediator">Inyección de MediatR para envío de comando.</param>
    /// <returns>
    /// 200 OK: Precio registrado exitosamente.
    /// 400 Bad Request: Validación fallida o producto/supermercado no existe.
    /// 401 Unauthorized: Token no válido o ausente.
    /// 500 Internal Server Error: Error de servidor.
    /// </returns>
    private static async Task<IResult> RegisterPrice(
        [FromBody] RegisterPriceCommand command,
        IMediator mediator)
    {
        // Enviar comando a través de MediatR (ejecuta validación + handler)
        var result = await mediator.Send(command);

        // Traducir resultado a respuesta HTTP
        if (result.IsSuccess)
        {
            return TypedResults.Ok(
                ApiResponse<object>.SuccessResponse(
                    new { },
                    "Precio registrado exitosamente."));
        }

        // Si falla, retornar error con descripción
        return TypedResults.BadRequest(
            ApiResponse<object>.ErrorResponse(result.Error!.Description));
    }

    /// <summary>
    /// Endpoint HTTP GET /api/v1/products/{productId}/prices
    /// Obtiene la comparación de precios de un producto en diferentes supermercados.
    /// </summary>
    /// <param name="productId">Identificador del producto a comparar.</param>
    /// <param name="mediator">Inyección de MediatR para envío de consulta.</param>
    /// <returns>
    /// 200 OK: Lista de precios del producto en diferentes supermercados.
    /// 400 Bad Request: Producto no encontrado o sin historial de precios.
    /// 401 Unauthorized: Token no válido o ausente.
    /// 500 Internal Server Error: Error de servidor.
    /// </returns>
    private static async Task<IResult> GetPriceComparison(
        int productId,
        IMediator mediator)
    {
        // Enviar consulta a través de MediatR
        var result = await mediator.Send(new GetPriceComparisonByProductQuery(productId));

        // Traducir resultado a respuesta HTTP
        if (result.IsSuccess)
        {
            return TypedResults.Ok(
                ApiResponse<IEnumerable<PriceHistoryDto>>.SuccessResponse(
                    result.Value!,
                    "Comparación de precios recuperada exitosamente."));
        }

        // Si falla, retornar error
        return TypedResults.BadRequest(
            ApiResponse<IEnumerable<PriceHistoryDto>>.ErrorResponse(result.Error!.Description));
    }
}
```

---

## 🔧 CONFIGURACIÓN E INYECCIÓN DE DEPENDENCIAS

### Ubicación: `Backend.PriceComparison.Application/DependencyInjectionService.cs`

```csharp
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Backend.PriceComparison.Application.Common.Behaviors;
using Backend.PriceComparison.Application.Store.Mappers;
using System.Reflection;
using FluentValidation;

namespace Backend.PriceComparison.Application;

/// <summary>
/// Servicio de inyección de dependencias para la capa Application.
/// 
/// Patrón: DEPENDENCY INJECTION (composición de dependencias)
/// - Registra todos los servicios de aplicación.
/// - Configura MediatR, AutoMapper, FluentValidation.
/// - Agrega middleware de validación automática.
/// </summary>
public static class DependencyInjectionService
{
    /// <summary>
    /// Extiende IServiceCollection para agregar servicios de Application.
    /// Llamado desde Program.cs: services.AddApplication()
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        #region AutoMapper Configuration
        // Configurar AutoMapper con perfil de Store
        var mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<StoreProfile>();
        });
        services.AddSingleton(mapper.CreateMapper());
        #endregion

        #region MediatR Configuration
        // Registrar MediatR: encuentra automáticamente Handlers en este assembly
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        #endregion

        #region FluentValidation Configuration
        // Registrar validadores: busca AbstractValidator<T> en este assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        #endregion

        #region MediatR Pipeline Behaviors
        // Agregar comportamiento de validación automática como pipeline behavior
        // Se ejecuta ANTES de cada Handler
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>)
        );
        #endregion

        return services;
    }
}
```

### Ubicación: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/DependencyInjectionService.cs`

```csharp
// Fragmento relevante para este caso de uso:
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IStoreRepository, StoreRepository>();
services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
```

### Ubicación: `Backend.PriceComparison.Api/Program.cs`

```csharp
// Fragmento relevante:
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()              // Registra servicios de Application
    .AddPersistence(builder.Configuration);  // Registra repositorios y cachés

// ... más configuración ...

var app = builder.Build();

// Mapear endpoints
app.MapPriceHistoryEndpoints();  // Registra rutas del caso de uso

app.Run();
```

---

## 🔄 FLUJO COMPLETO DE EJECUCIÓN

### Caso 1: Registrar Precio de Producto

```
┌─────────────────────────────────────────────────────────────────────────┐
│ SOLICITUD HTTP                                                          │
│ POST /api/v1/prices                                                     │
│ {                                                                       │
│   "productId": 1,                                                       │
│   "storeId": 5,                                                         │
│   "price": 45.99,                                                       │
│   "date": "2024-06-19T10:30:00Z"                                        │
│ }                                                                       │
└──────────────┬──────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ CAPA API: PriceHistoryEndpoints.cs                                      │
│ - Recibe solicitud HTTP                                                │
│ - Deserializa JSON a RegisterPriceCommand                              │
│ - Inyecta IMediator                                                     │
└──────────────┬───────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ CAPA APPLICATION: MediatR Pipeline                                      │
│ ┌────────────────────────────────────────────────────────────────────┐ │
│ │ 1. ValidationBehaviour (IPipelineBehavior)                        │ │
│ │    - Obtiene validador: RegisterPriceCommandValidator             │ │
│ │    - Valida datos:                                                │ │
│ │      ✓ ProductId > 0                                             │ │
│ │      ✓ StoreId > 0                                               │ │
│ │      ✓ Price > 0                                                 │ │
│ │      ✓ Date <= now (no futuro)                                   │ │
│ │    - Si validación falla: retorna ValidationException (400)       │ │
│ │    - Si válido: continúa al Handler                              │ │
│ └────────────────────────────────────────────────────────────────────┘ │
│ ┌────────────────────────────────────────────────────────────────────┐ │
│ │ 2. RegisterPriceCommandHandler.Handle()                            │ │
│ │    a. Valida existencia de Product en Domain                       │ │
│ │       - IProductRepository.GetByIdAsync(1)                        │ │
│ │       - Si no existe: return Error                                │ │
│ │    b. Valida existencia de Store en Domain                        │ │
│ │       - IStoreRepository.GetByIdAsync(5)                          │ │
│ │       - Si no existe: return Error                                │ │
│ │    c. Mapea Command → PriceHistoryEntity                          │ │
│ │       - IMapper.Map<PriceHistoryEntity>(command)                 │ │
│ │    d. Persiste en repositorio                                     │ │
│ │       - IPriceHistoryRepository.CreateAsync(entity)               │ │
│ │    e. Invalida caché                                              │ │
│ │       - ICacheService.RemoveByPrefixAsync("price_history_1")      │ │
│ │    f. Retorna Result<VoidResult, Error>                           │ │
│ └────────────────────────────────────────────────────────────────────┘ │
└──────────────┬───────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ CAPA INFRASTRUCTURE: PriceHistoryRepository.CreateAsync()                │
│ - Agrega entidad al DbContext de EF Core                               │
│ - SaveChangesAsync() ejecuta INSERT en MySQL                           │
│ - Retorna Result<VoidResult, Error>                                    │
└──────────────┬───────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ BASE DE DATOS: MySQL - Tabla PriceHistory                               │
│ INSERT INTO PriceHistory (ProductId, StoreId, Price, Date) VALUES (...)│
│ ✓ Inserción exitosa                                                     │
└──────────────┬───────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ RESPUESTA HTTP: 200 OK                                                   │
│ {                                                                       │
│   "success": true,                                                      │
│   "message": "Precio registrado exitosamente.",                        │
│   "data": {}                                                            │
│ }                                                                       │
└──────────────────────────────────────────────────────────────────────────┘
```

### Caso 2: Consultar Comparación de Precios

```
┌─────────────────────────────────────────────────────────────────────────┐
│ SOLICITUD HTTP                                                          │
│ GET /api/v1/products/1/prices                                           │
└──────────────┬──────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ CAPA API: PriceHistoryEndpoints.GetPriceComparison()                     │
│ - Recibe parámetro de ruta: productId = 1                              │
│ - Inyecta IMediator                                                     │
│ - Envía: GetPriceComparisonByProductQuery(1)                           │
└──────────────┬───────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ CAPA APPLICATION: GetPriceComparisonByProductQueryHandler.Handle()       │
│ ┌────────────────────────────────────────────────────────────────────┐ │
│ │ 1. Construir clave de caché: "price_history_1"                   │ │
│ │ 2. ICacheService.GetAsync("price_history_1")                     │ │
│ │    ├─ ✓ Si está en caché (Redis/Memory):                        │ │
│ │    │   └─ Retorna IEnumerable<PriceHistoryDto> inmediatamente   │ │
│ │    │                                                              │ │
│ │    └─ ✗ Si NO está en caché:                                   │ │
│ │        3. IPriceHistoryRepository.GetPricesByProductAsync(1)     │ │
│ │           - EF Core query con Include (Product, Store)           │ │
│ │           - OrderBy(Price)                                        │ │
│ │           - Retorna IEnumerable<PriceHistoryEntity>             │ │
│ │        4. IMapper.Map<IEnumerable<PriceHistoryDto>>()           │ │
│ │        5. ICacheService.SetAsync(..., TimeSpan.FromMinutes(10)) │ │
│ │           - Almacena resultado en caché por 10 minutos           │ │
│ │        6. Retorna dtos                                            │ │
│ └────────────────────────────────────────────────────────────────────┘ │
└──────────────┬───────────────────────────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────────────────────────┐
│ RESPUESTA HTTP: 200 OK                                                   │
│ {                                                                       │
│   "success": true,                                                      │
│   "message": "Comparación de precios recuperada exitosamente.",        │
│   "data": [                                                             │
│     {                                                                   │
│       "id": 10,                                                         │
│       "productId": 1,                                                   │
│       "productName": "Leche Integral 1L",                              │
│       "storeId": 3,                                                     │
│       "storeName": "Carrefour",                                        │
│       "price": 38.50,                                                   │
│       "date": "2024-06-19T09:15:00Z"                                   │
│     },                                                                  │
│     {                                                                   │
│       "id": 11,                                                         │
│       "productId": 1,                                                   │
│       "productName": "Leche Integral 1L",                              │
│       "storeId": 5,                                                     │
│       "storeName": "Walmart",                                          │
│       "price": 41.99,                                                   │
│       "date": "2024-06-19T10:00:00Z"                                   │
│     }                                                                   │
│   ]                                                                     │
│ }                                                                       │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## 📊 Diagrama de Dependencias (Arquitectura Hexagonal)

```
┌────────────────────────────────────────────────────────────────────────┐
│                           CAPA API                                     │
│  PriceHistoryEndpoints.cs (Minimal API)                              │
│  ├── POST /api/v1/prices                                            │
│  └── GET /api/v1/products/{id}/prices                               │
└────────────────────┬─────────────────────────────────────────────────┘
                     │ depende de
                     ▼
┌────────────────────────────────────────────────────────────────────────┐
│                      CAPA APPLICATION                                  │
│  MediatR Pipeline                                                     │
│  ├── RegisterPriceCommand + Validator + Handler                      │
│  └── GetPriceComparisonByProductQuery + Handler                      │
│                                                                        │
│  AutoMapper: StoreProfile                                            │
│  DTOs: PriceHistoryDto                                               │
└────────────────────┬─────────────────────────────────────────────────┘
                     │ depende de
                     ▼
┌────────────────────────────────────────────────────────────────────────┐
│                       CAPA DOMAIN                                      │
│  ┌─────────────────────────────────────────────────────────┐         │
│  │ ENTITIES (sin dependencias externas)                    │         │
│  ├── ProductEntity                                         │         │
│  ├── StoreEntity                                           │         │
│  └── PriceHistoryEntity (AGGREGATE ROOT)                  │         │
│  └─────────────────────────────────────────────────────────┘         │
│  ┌─────────────────────────────────────────────────────────┐         │
│  │ PORTS (Interfaces de Contrato)                         │         │
│  ├── IPriceHistoryRepository                              │         │
│  ├── IProductRepository                                   │         │
│  ├── IStoreRepository                                     │         │
│  └── ICacheService                                        │         │
│  └─────────────────────────────────────────────────────────┘         │
└────────────────────┬─────────────────────────────────────────────────┘
                     │ implementado por
                     ▼
┌────────────────────────────────────────────────────────────────────────┐
│                     CAPA INFRASTRUCTURE                                │
│  Repositories (implementación EF Core)                               │
│  ├── PriceHistoryRepository (IPriceHistoryRepository)                │
│  ├── ProductRepository (IProductRepository)                          │
│  ├── StoreRepository (IStoreRepository)                              │
│  │                                                                    │
│  │ Adaptadores de Caché & Persistencia                              │
│  ├── RedisCacheService (ICacheService)                              │
│  └── ClientDbContext (EF Core MySQL)                                │
└────────────────────────────────────────────────────────────────────────┘
```

---

## ✅ Principios SOLID Aplicados

| Principio | Aplicación |
|-----------|-----------|
| **S**ingle Responsibility | Cada clase tiene una única razón para cambiar. Repositorio solo maneja persistencia, Handler solo orquesta, Validator solo valida. |
| **O**pen/Closed | Abierto a extensión (nuevos handlers, comandos) cerrado a modificación. Se agrega sin cambiar existente. |
| **L**iskov Substitution | Las implementaciones de repositorios pueden sustituirse sin afectar el código cliente. |
| **I**nterface Segregation | Interfaces específicas (IPriceHistoryRepository, IProductRepository) sin métodos innecesarios. |
| **D**ependency Inversion | Domain define interfaces, Infrastructure implementa. Application depende de abstracciones, no de concreciones. |

---

## 🔒 Patrones de Seguridad Implementados

1. **Pattern Matching Result<T, E>**: Sin excepciones, manejo de errores explícito
2. **Encapsulación**: DTOs no exponen entidades de dominio
3. **Validación Automática**: ValidationBehaviour intercepta cada comando
4. **Logging**: Registro de operaciones críticas para auditoría
5. **Aislamiento de Contexto**: EF Core AsNoTracking() para queries (lectura pura)

---

## 🚀 Próximas Extensiones

Para extender este caso de uso:

1. **Agregar Paginación**: Modificar query handler para soportar pageNumber/pageSize
2. **Agregar Filtros**: Permitir filtrar por rango de fechas o rango de precios
3. **Agregar Estadísticas**: Query handler que retorne media, min, max de precios
4. **Agregar Notificaciones**: Event sourcing cuando se registra un precio
5. **Agregar Autenticación**: Verificar usuario antes de registrar precios

---

**Fecha de Documento**: 2024-06-19
**Versión**: 1.0
**Arquitectura**: Clean Architecture + CQRS + Minimal API
**Framework**: .NET 10 con EF Core 9
