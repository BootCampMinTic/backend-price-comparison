# Backend Price Comparison - Wiki

Bienvenido a la wiki del proyecto **backend-price-comparison**. Esta guia esta disenada para desarrolladores nuevos que necesitan entender e implementar features siguiendo la arquitectura hexagonal del proyecto.

## Arquitectura

El proyecto sigue **Hexagonal (Ports & Adapters)** con las dependencias apuntando hacia adentro:

```
Api  ──► Application ──► Domain ◄── Infrastructure (adapters)
Api  ──► Infrastructure  (solo en composition root / DI)
```

### Capas

| Capa | Proyecto .NET | Responsabilidad |
|------|--------------|-----------------|
| **Domain** | `Backend.PriceComparison.Domain` | Entidades, puertos (interfaces), enums, Result pattern |
| **Application** | `Backend.PriceComparison.Application` | MediatR commands/queries/handlers, validators, DTOs, AutoMapper |
| **Infrastructure** | `Infrastructure.Persistence.Mysql` | Repositorios EF Core, Redis cache, mocks |
| **API** | `Backend.PriceComparison.Api` | Minimal API endpoints, middleware, health checks |

## Guias paso a paso

1. [Arquitectura Hexagonal](Arquitectura-Hexagonal) - Explicacion de la arquitectura
2. [Guia: Capa Domain](Guia-Domain) - Como crear entidades y puertos
3. [Guia: Capa Application](Guia-Application) - Como crear queries, commands, handlers y validators
4. [Guia: Capa Infrastructure](Guia-Infrastructure) - Como crear repositorios
5. [Guia: Capa API](Guia-API) - Como crear endpoints
6. [Checklist Nueva Entidad](Checklist-Nueva-Entidad) - Lista completa de verificacion
7. [Convenciones](Convenciones) - Convenciones de codigo del proyecto

## Ejemplo practico

Sigue la implementacion de `StoreEntity` como referencia:
- [Spec: Store](https://github.com/BootCampMinTic/backend-price-comparison/blob/main/docs/specs/05-store.md)

## Herramientas

Usa el skill **verificador-hexagonal** para validar que no falte nada al implementar una nueva entidad:

```
npx skills add verificador-hexagonal
```
