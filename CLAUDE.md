# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Common commands (PowerShell)

```powershell
# Restore + build the whole solution
dotnet restore .\backend-price-comparison.sln
dotnet build .\backend-price-comparison.sln -c Debug

# Run the API (Scalar UI auto-opens at /scalar/v1)
dotnet run --project .\Backend.PriceComparison.Api\Backend.PriceComparison.Api.csproj --launch-profile http
# -> http://localhost:5062  https://localhost:5000

# Run all tests
dotnet test .\backend-price-comparison.sln -c Release

# Run a single test project / single test
dotnet test .\Backend.PriceComparison.Domain.Test\Backend.PriceComparison.Domain.Test.csproj
dotnet test .\backend-price-comparison.sln --filter "FullyQualifiedName~ClientHandlerTests"

# Docker — mock mode (no MySQL/Redis needed)
docker build -t backend-api-eds-client:local .
docker run -d --name backend-api-eds-client -p 8080:8080 `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e UseMockInfrastructure=true `
  backend-api-eds-client:local
```

`MYSQL_CONNECTION` and `REDIS_CONNECTION` environment variables override `ConnectionStrings:MysqlConnection` and `Redis:ConnectionString` respectively. Setting `UseMockInfrastructure=true` swaps EF Core + Redis for the in-memory implementations under `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Mock`, so the API boots without external dependencies.

## Solution scope vs. on-disk projects

The `.sln` contains only six projects: `Backend.PriceComparison.Api`, `Backend.PriceComparison.Application`, `Backend.PriceComparison.Domain`, `Backend.PriceComparison.Infrastructure.Persistence.Mysql`, `Backend.PriceComparison.Api.Tests`, `Backend.PriceComparison.Domain.Test`.

The other top-level directories (`Backend.PriceComparison.Common`, `Backend.PriceComparison.Infrastructure.External.Plemsi`, `Backend.PriceComparison.Infrastructure.External.TNS`, `WorkerServiceBilling`, `LoadTest`, `Poliedro.Client.*`) are **not** referenced by the main solution and are not built by `dotnet build` / CI. Treat them as parked code — don't assume changes there compile against the API or run in CI without first wiring them in.

## Architecture

Hexagonal / Ports & Adapters layered around the Domain. Dependencies point inward only:

```
Api  ─► Application ─► Domain ◄─ Infrastructure.Persistence.Mysql
                                       (adapter side)
Api  ─► Infrastructure.Persistence.Mysql  (composition root only, via DI)
```

- **Domain** (`Backend.PriceComparison.Domain`) — entities (`ClientEntity`, `ClientNaturalPosEntity`, `ClientLegalPosEntity`, `DocumentTypeEntity`), enums, the `Result<TValue, TError>`/`VoidResult` types, and **ports** in `Domain/Ports/` (`IClientRepository`, `IDocumentTypeRepository`, `ICacheService`, `IMessageProvider`). No external dependencies.
- **Application** — MediatR commands/queries + handlers under `Client/Commands` and `Client/Queries`, plus thin `IClientCommandService` / `IClientQueryService` facades that the API calls (the API does not reference MediatR directly). AutoMapper `ClientProfile` maps Command ↔ Entity ↔ DTO. FluentValidation validators live next to the requests they validate and run via `ValidationBehaviour<,>` registered as an `IPipelineBehavior`.
- **Infrastructure.Persistence.Mysql** — adapters that implement the Domain ports: `ClientRepository`, `DocumentTypeRepository` (EF Core + Pomelo MySQL), `RedisCacheService` (StackExchange.Redis, configured via `IOptions<RedisSettings>`), `MessageProvider` (resx-based), plus `Mock/*` in-memory variants used when `UseMockInfrastructure=true`. `ClientDbContext` is the EF aggregate root.
- **Api** — Minimal API host. `Program.cs` wires `AddApplication()` then `AddPersistence(IConfiguration)`. Endpoints in `Endpoints/*.cs` (`MapClientEndpoints`, `MapDocumentTypeEndpoints`, `MapHealthApiEndpoints`) call the Application services and wrap returns in `ApiResponse<T>` / `PagedResponse<T>` from `Common/Wrappers`.

### Result pattern, not exceptions, for business errors

Handlers and repositories return `Result<TValue, Error>` (see `Domain/Common/Results/Result.cs` and `ClientErrorBuilder`). Implicit conversions let you `return entity;` or `return errorBuilder.SomeError();` directly. The API layer translates `IsSuccess` into `200 OK` / `400 BadRequest` (`ApiResponse<T>.SuccessResponse` / `ErrorResponse`). Reserve `throw` for truly exceptional/infrastructure failures — `ExceptionMiddleware` only translates `FluentValidation.ValidationException` (→ 400 with grouped errors) and falls back to 500 for everything else.

### Read path uses Redis-backed caching

Query handlers (`GetAllClientNaturalQueryHandler`, `GetAllClientLegalQueryHandler`, etc.) read through `ICacheService` first using stable keys like `clients:natural:page:{n}:size:{s}`. Command handlers (`CreateClientNaturalPosHandle`, `CreateClientLegalPosHandle`) call `_cacheService.RemoveByPrefixAsync("clients:natural" | "clients:legal", ct)` after a successful write. **When you add a new write path, you must invalidate the matching prefix or stale lists will be served.**

### Authentication is not really authentication

`BearerTokenMiddleware` only checks that an `Authorization: Bearer <token>` header is present — no signature, no expiration, no claims. Public endpoints whitelisted in the middleware: `/health*`, `/api/v1/health*`, `/openapi*`, `/scalar*`, `/index.html`, `/_framework`, `/_vs`, `/css`, `/js`, plus `/` and `/api/v1/dev` in Development. Don't rely on it for real authorization decisions.

### Known schema drift: `DocumentCountry`

`ClientDbContext.OnModelCreating` calls `.Ignore(e => e.DocumentCountry)` on both `ClientNaturalPosEntity` and `ClientLegalPosEntity` because the column does not yet exist in the MySQL schema. The property is accepted on inbound commands but is **not persisted and always reads back null/empty**. If you touch this area, either keep the `Ignore` or also ship the `ALTER TABLE … ADD COLUMN DocumentCountry VARCHAR(255) NULL` migration referenced in the inline TODO at `ClientDbContext.cs:39` and remove the ignores.

## Conventions when adding a new use case

1. Add the request type (Command or Query implementing `IRequest<Result<…, Error>>`) under `Application/Client/{Commands|Queries}/<Feature>/`.
2. Add the matching `Handler` in the same folder; inject Domain ports (`IClientRepository`, `ICacheService`, `IMapper`, `IMessageProvider`) — never EF Core or Redis types directly.
3. Add a `FluentValidation.AbstractValidator<TRequest>` next to the request — it gets picked up by `AddValidatorsFromAssembly` and the `ValidationBehaviour` runs it before the handler.
4. Update `ClientProfile` if a new mapping shape is needed (commands → entities ignore `Id`/navigation props).
5. Expose it through `IClientCommandService` / `IClientQueryService` (or a new facade), then map a Minimal-API endpoint in `Backend.PriceComparison.Api/Endpoints/`. Endpoints call the service, not `IMediator`.
6. If it's a write affecting a cached list, invalidate the corresponding `clients:*` prefix in the handler.

## Configuration keys worth knowing

| Key / env var | Purpose |
| --- | --- |
| `MYSQL_CONNECTION` / `ConnectionStrings:MysqlConnection` | EF Core MySQL connection (Pomelo, server version auto-detected). |
| `REDIS_CONNECTION` / `Redis:ConnectionString` | StackExchange.Redis multiplexer. |
| `Redis:CacheExpirationMinutes` | Default TTL used by `RedisCacheService`. |
| `UseMockInfrastructure` | `true` swaps EF Core + Redis for the in-memory mocks in `Mock/`. |
| `AllowedOrigins` | List of CORS origins; `*` if missing. |
| `ApiPlemsi:*` | Reserved for the (currently out-of-solution) Plemsi integration. |
