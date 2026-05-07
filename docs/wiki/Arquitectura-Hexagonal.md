# Arquitectura Hexagonal (Ports & Adapters)

## Diagrama de capas

```
┌─────────────────────────────────────────────────────────┐
│                      API (Minimal API)                   │
│  Endpoints, Middleware, HealthChecks, Wrappers           │
│  Depende de: Application + Infrastructure (DI only)      │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                    Application                           │
│  Commands, Queries, Handlers, Validators, DTOs, Mappers │
│  Depende de: Domain ports (nunca Infrastructure)        │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                       Domain                             │
│  Entities, Enums, Ports (interfaces), Result<T,E>       │
│  NO depende de nadie. Es el centro de la arquitectura.  │
└──────────────────────▲──────────────────────────────────┘
                       │
┌──────────────────────┴──────────────────────────────────┐
│            Infrastructure (adapters)                     │
│  Repositories, DbContext, Redis, Mocks, MessageProvider  │
│  Implementa los puertos del Domain                       │
└─────────────────────────────────────────────────────────┘
```

## Regla de dependencias

```
API ──► Application ──► Domain (puertos)
API ──► Infrastructure (solo DI / composition root)
Infrastructure ──► Domain (implementa puertos)
```

**NUNCA:**
- ❌ Application no referencia Infrastructure
- ❌ Domain no referencia Application ni Infrastructure
- ❌ API no usa tipos de Infrastructure fuera del DI

## Flujo de una request

```
1. HTTP Request → API Endpoint
2. Endpoint → MediatR (Command/Query)
3. MediatR → ValidationBehaviour (FluentValidation)
4. Handler → Domain Port (interface)
5. Infrastructure Repository → MySQL/Redis
6. Result<T,E> ← Repository
7. Handler → Result<T,E> (cache, mapping)
8. Endpoint → HTTP Response (ApiResponse/PagedResponse)
```

## Patron Result (no excepciones para negocio)

```csharp
// Los handlers y repositorios retornan Result<TValue, Error>
// Nunca lanzan excepciones para errores de negocio esperados.

Result<VoidResult, Error> CreateAsync(Entity e)
{
    await _context.AddAsync(e);
    if (await _context.SaveChangesAsync() > 0)
        return VoidResult.Instance;    // éxito
    return StoreErrorBuilder.CreationFailed("entity"); // error
}

// En el handler:
if (!result.IsSuccess)
    return result.Error!;   // propaga el error
```

## Inyeccion de dependencias

Cada capa tiene su propio `DependencyInjectionService`:

```csharp
// Program.cs
builder.Services
    .AddApplication()       // MediatR, AutoMapper, Validators, Services
    .AddPersistence(config); // DbContext, Redis, Repositories
```

El API nunca instancia repositorios directamente. Siempre usa los puertos del Domain.
