# backend-price-comparison

API de comparacion de precios basada en ASP.NET Core Minimal APIs con arquitectura hexagonal (Puertos y Adaptadores). Este proyecto sirve como **ejemplo base** para que desarrolladores junior aprendan el patron de creacion de servicios completos — el endpoint **Product** es el ejemplo canonico.

## Contenido

- [Arquitectura](#arquitectura)
- [Tecnologias principales](#tecnologias-principales)
- [Requisitos](#requisitos)
- [Configuracion](#configuracion)
- [Ejecucion local](#ejecucion-local)
- [Documentacion de la API](#documentacion-de-la-api)
- [Endpoints](#endpoints)
- [Health checks](#health-checks)
- [Pruebas](#pruebas)
- [Docker](#docker)
- [Guia: Crear un nuevo servicio](#guia-crear-un-nuevo-servicio)

## Arquitectura

La solucion usa **Hexagonal / Puertos y Adaptadores** organizada por capas:

`
Api —? Application —? Domain ?— Infrastructure.Persistence.Mysql
                                    (adapter side)
`

| Proyecto | Responsabilidad |
| --- | --- |
| Backend.PriceComparison.Api | Punto de entrada HTTP. Minimal APIs, CORS, Scalar/OpenAPI, Redis, health checks y middleware Bearer. |
| Backend.PriceComparison.Application | Casos de uso: commands/queries con MediatR, AutoMapper, FluentValidation. |
| Backend.PriceComparison.Domain | Entidades, puertos (interfaces), resultados Result<T, Error>. Sin dependencias externas. |
| Backend.PriceComparison.Infrastructure.Persistence.Mysql | Implementaciones concretas de los puertos: EF Core + MySQL, Redis, mocks en memoria. |
| Backend.PriceComparison.Api.Tests | Pruebas xUnit de integracion. |
| Backend.PriceComparison.Domain.Test | Pruebas xUnit de dominio. |

### Flujo de datos (usando Product como ejemplo):

`
HTTP Request
  -> ProductEndpoints (API)
    -> IMediator.Send(GetAllProductsQuery)  (Application)
      -> GetAllProductsQueryHandler
        -> ICacheService.GetAsync()         <- si esta en cache, retorna
        -> IProductRepository.GetAllAsync() (Domain port)
          -> ProductRepository               (Infrastructure adapter)
            -> EF Core / MySQL
        -> ICacheService.SetAsync()
      -> Result<ProductDto, Error>
    -> ApiResponse<PagedResponse<ProductDto>>
  -> HTTP Response
`

## Tecnologias principales

- .NET 10.0
- ASP.NET Core Minimal APIs
- Entity Framework Core 9.0 con Pomelo.EntityFrameworkCore.MySql
- MySQL
- Redis con StackExchange.Redis
- MediatR (commands/queries)
- AutoMapper (entidad <-> DTO)
- FluentValidation (pipeline de validacion)
- Scalar (documentacion OpenAPI)
- xUnit + coverlet (pruebas)

## Requisitos

- .NET SDK 10.0.x
- MySQL (o usar mock)
- Redis (o usar mock)
- Docker (opcional)

## Configuracion

| Variable / clave | Uso |
| --- | --- |
| MYSQL_CONNECTION / ConnectionStrings:MysqlConnection | Cadena de conexion MySQL para EF Core. |
| REDIS_CONNECTION / Redis:ConnectionString | Conexion a Redis. |
| Redis:CacheExpirationMinutes | TTL por defecto del cache (default: 10). |
| UseMockInfrastructure | 	rue -> usa repositorios en memoria (no necesita MySQL/Redis). |
| AllowedOrigins | Origenes CORS. |

## Ejecucion local

`powershell
dotnet restore .\backend-price-comparison.sln
dotnet build .\backend-price-comparison.sln -c Debug
dotnet run --project .\Backend.PriceComparison.Api\Backend.PriceComparison.Api.csproj --launch-profile http
`

La API queda en http://localhost:5062 y https://localhost:5000.

## Documentacion de la API

`	ext
http://localhost:5062/scalar/v1
http://localhost:5062/openapi/v1.json
`

## Endpoints

Todos los endpoints requieren el header Authorization: Bearer <token>, excepto /health*, /openapi*, /scalar*.

### Product

| Metodo | Ruta | Descripcion |
| --- | --- | --- |
| GET | /api/v1/products?pageNumber=1&pageSize=10 | Lista productos paginados. |
| GET | /api/v1/products/{id} | Producto por ID. |
| GET | /api/v1/stores/{storeId}/products?pageNumber=1&pageSize=10 | Productos por tienda (paginado). |
| POST | /api/v1/products | Crear un nuevo producto. |

Ejemplos:

`ash
curl -H "Authorization: Bearer dev-token" "http://localhost:5062/api/v1/products?pageNumber=1&pageSize=10"
`

`json
{
  "name": "Producto Ejemplo",
  "price": 25000.00,
  "storeId": 1,
  "categoryProductId": 1
}
`

## Health checks

| Metodo | Ruta | Uso |
| --- | --- | --- |
| GET | /health | Estado completo. |
| GET | /health/ready | Readiness (incluye DB). |
| GET | /health/live | Liveness. |
| GET | /api/v1/health | Estado completo versionado. |
| GET | /api/v1/health/ready | Readiness versionado. |
| GET | /api/v1/health/live | Liveness versionado. |

## Pruebas

`powershell
dotnet test .\backend-price-comparison.sln -c Release
`

## Docker

`powershell
# Construir
docker build -t backend-api-eds-client:local .

# Ejecutar con mocks (sin MySQL/Redis)
docker run -d --name backend-api-eds-client -p 8080:8080 
  -e ASPNETCORE_ENVIRONMENT=Development 
  -e UseMockInfrastructure=true 
  backend-api-eds-client:local

# Ejecutar con MySQL/Redis reales
docker run -d --name backend-api-eds-client -p 8080:8080 
  -e ASPNETCORE_ENVIRONMENT=Production 
  -e MYSQL_CONNECTION=""Server=host.docker.internal;Port=3306;Database=clients;User Id=root;Password=local_password;ConvertZeroDateTime=True;SslMode=Disabled"" 
  -e REDIS_CONNECTION=""host.docker.internal:6379"" 
  backend-api-eds-client:local
`

## Guia: Crear un nuevo servicio

Consulta la guia completa en [docs/guia-crear-servicio-completo.md](docs/guia-crear-servicio-completo.md) donde se explica paso a paso como crear un servicio completo usando **Product** como ejemplo.

Resumen de archivos a crear para una nueva entidad (ej: Category):

| Capa | Archivo | Proposito |
| --- | --- | --- |
| Domain | Entities/CategoryEntity.cs | Entidad. |
| Domain | Ports/ICategoryRepository.cs | Interface del repositorio (puerto). |
| Application | Dtos/CategoryDto.cs | DTO de respuesta. |
| Application | Queries/Category/GetAllCategoriesQuery.cs | Query. |
| Application | Queries/Category/GetAllCategoriesQueryHandler.cs | Handler. |
| Application | Commands/CreateCategory/CreateCategoryCommand.cs | Command. |
| Application | Commands/CreateCategory/CreateCategoryCommandHandler.cs | Handler. |
| Application | Commands/CreateCategory/CreateCategoryCommandValidator.cs | Validacion. |
| Application | Mappers/StoreProfile.cs | Mapping AutoMapper (editar). |
| Infrastructure | Store/Repositories/CategoryRepository.cs | Implementacion del repositorio. |
| Infrastructure | DependencyInjectionService.cs | Registrar repositorio (editar). |
| API | Endpoints/CategoryEndpoints.cs | Endpoints. |
| API | Program.cs | Registrar endpoints (editar). |