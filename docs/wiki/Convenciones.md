# Convenciones de Codigo

## C# General

| Convencion | Ejemplo |
|------------|---------|
| `sealed` en todas las clases no heredables | `public sealed class ProductEntity` |
| Primary constructors para DI | `public sealed class Handler(IPort _port) : IHandler` |
| `_camelCase` para campos inyectados | `_productRepository`, `_cacheService` |
| Strings con `= string.Empty` | `public string Name { get; set; } = string.Empty;` |
| `string?` para campos nullable | `public string? Phone { get; set; }` |

## Domain

| Convencion | Ejemplo |
|------------|---------|
| Entidades en `Domain/{Feature}/Entities/` | `Domain/Store/Entities/ProductEntity.cs` |
| Puertos en `Domain/Ports/` | `Domain/Ports/IProductRepository.cs` |
| Result pattern (no excepciones) | `Result<T, Error>` |
| Error builder estatico | `StoreErrorBuilder.NotFound(id, "Product")` |
| Sin dependencias externas | No `using` de otros proyectos |

## Application

| Convencion | Ejemplo |
|------------|---------|
| Commands en `Application/{Feature}/Commands/{Action}{Entity}/` | `Commands/CreateProduct/` |
| Queries en `Application/{Feature}/Queries/{Entity}/` | `Queries/Product/` |
| DTOs en `Application/{Feature}/Dtos/` | `Dtos/ProductDto.cs` |
| Mappers en `Application/{Feature}/Mappers/` | `Mappers/StoreProfile.cs` |
| Validators co-localizados con commands | `CreateProductCommandValidator.cs` |
| Cache keys en `Application/Client/CacheKeys.cs` | Clase estatica compartida |
| Solo inyectar puertos (nunca tipos de Infra) | `IProductRepository`, no `ProductRepository` |

## Infrastructure

| Convencion | Ejemplo |
|------------|---------|
| Repos en `Infrastructure/{Feature}/Repositories/` | `Store/Repositories/ProductRepository.cs` |
| `internal sealed` para repos | Ocultos fuera del assembly |
| Mocks en `Infrastructure/Mock/` | `MockProductRepository.cs` |
| `AsNoTracking()` en todos los reads | Performance EF Core |
| `Include()` explicito para FKs | Evitar N+1 |
| `using` del namespace `Domain.Ports` | Implementa interfaces del Domain |

## API

| Convencion | Ejemplo |
|------------|---------|
| Endpoints en `Api/Endpoints/` | `ProductEndpoints.cs` |
| Clases `static` con metodo `Map{Feature}Endpoints` | Metodo de extension |
| `MapGroup("api/v1")` con `WithTags` | Organizacion en Scalar |
| `IMediator` inyectado directamente | Sin servicio intermedio |

## Nombrado

| Que | Como |
|-----|------|
| Entidad | `{Name}Entity` |
| Puerto | `I{Name}Repository` |
| Repositorio | `{Name}Repository` (internal) |
| Mock | `Mock{Name}Repository` (public) |
| Query | `Get{Action}{Name}Query` |
| Command | `{Action}{Name}Command` |
| DTO | `{Name}Dto` |
| Validator | `{Action}{Name}CommandValidator` |
| Endpoint file | `{Feature}Endpoints.cs` |
| Error builder | `{Feature}ErrorBuilder` |

## Git

| Regla |
|-------|
| Conventional Commits: `feat(scope): descripcion` |
| Commits en ingles |
| `opencode.json` en `.gitignore` (contiene credenciales) |
| No commitear `.env`, `*.pfx`, `secrets.*` |
