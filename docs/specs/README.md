# Technical Specifications

Specs techniques detalladas para cada entidad de la base de datos `eduar_demo`.

Cada spec incluye: schema MySQL, entidad C#, puerto (interface), repositorio, queries/commands, endpoints, cache, y notas de implementacion.

## Catalogs (read-only)

| # | Entidad | Tabla MySQL | Spec |
|---|---------|-------------|------|
| 1 | State | `state` | [01-state.md](01-state.md) |
| 2 | TypeUser | `type_user` | [02-typeuser.md](02-typeuser.md) |

## Catalogs (CRUD)

| # | Entidad | Tabla MySQL | Spec |
|---|---------|-------------|------|
| 3 | CategoryProduct | `category_product` | [03-categoryproduct.md](03-categoryproduct.md) |
| 4 | CategoryStore | `category_store` | [04-categorystore.md](04-categorystore.md) |

## Core Entities

| # | Entidad | Tabla MySQL | Spec |
|---|---------|-------------|------|
| 5 | Store | `store` | [05-store.md](05-store.md) |
| 6 | Product | `product` | [06-product.md](06-product.md) |
| 7 | User | `user` | [07-user.md](07-user.md) |

## Transactional

| # | Entidad | Tabla MySQL | Spec |
|---|---------|-------------|------|
| 8 | Sale | `sale` | [08-sale.md](08-sale.md) |
| 9 | ProductSale | `product_sale` | [09-productsale.md](09-productsale.md) |

## Relational Diagram

```
type_user 1──M user M──1 sale 1──M product_sale M──1 product
                                    ↑                       ↑
state 1──M sale                   store 1──M product
                                    ↑
category_store 1──M store       category_product 1──M product
```

## Tech Stack

- **Runtime:** .NET 10
- **API:** Minimal API + Scalar (OpenAPI)
- **ORM:** EF Core 9 + Pomelo MySQL
- **Cache:** Redis (StackExchange.Redis) + InMemory fallback
- **Mediator:** MediatR 14
- **Validation:** FluentValidation
- **Mapping:** AutoMapper 14
- **Pattern:** Hexagonal / Ports & Adapters
- **Result:** `Result<TValue, Error>` + `VoidResult` (no exceptions for business errors)

## Conventions

- `sealed` classes on all handlers and repositories
- Primary constructors for DI
- `AsNoTracking()` on all read queries
- `Include()` for FK navigation properties in reads
- `ICacheService` with cache-aside pattern in query handlers
- `RemoveByPrefixAsync()` for list cache invalidation in command handlers
- FluentValidation validators co-located with commands
- AutoMapper profiles in `Application/Store/Mappers/`
- XML doc comments on commands and queries
