# Guia: Capa Domain

Esta guia explica como crear entidades y puertos en la capa **Domain**.

## Paso 1: Crear la entidad

**Ubicacion:** `Backend.PriceComparison.Domain/{Feature}/Entities/`

```csharp
// Ejemplo: Domain/Store/Entities/ProductEntity.cs
namespace Backend.PriceComparison.Domain.Store.Entities;

public sealed class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }

    // FK properties (usa el nombre de la columna MySQL sin "id_")
    public int StoreId { get; set; }

    // Navigation properties (opcional, solo para EF Core Include())
    public StoreEntity? Store { get; set; }
}
```

### Reglas para entidades

| Regla | Ejemplo |
|-------|---------|
| Usar `sealed` | `public sealed class` |
| Propiedades con `{ get; set; }` | EF Core requiere setters publicos |
| Strings inicializados con `= string.Empty` | Evita CS8618 warnings |
| FK nombradas como `{Entity}Id` | `StoreId`, `CategoryProductId` |
| Navigation props como `{Entity}?` | `StoreEntity? Store` |
| No usar `[Key]`, `[Table]` | Se mapea en DbContext via fluent API |
| XML doc si aporta valor | `/// <summary>` |

## Paso 2: Crear el puerto (interface)

**Ubicacion:** `Backend.PriceComparison.Domain/Ports/`

```csharp
// Ejemplo: Domain/Ports/IProductRepository.cs
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface IProductRepository
{
    Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<Result<ProductEntity, Error>> GetByIdAsync(
        int id, CancellationToken cancellationToken);

    Task<Result<VoidResult, Error>> CreateAsync(
        ProductEntity entity, CancellationToken cancellationToken);
}
```

### Reglas para puertos

| Regla | Ejemplo |
|-------|---------|
| Nombre: `I{Entity}Repository` | `IProductRepository` |
| Usar `Result<T, Error>` para retornos | Nunca `Task<T>` directo ni excepciones |
| Crear: retorna `Result<VoidResult, Error>` | `VoidResult` = operacion exitosa sin datos |
| Leer: retorna `Result<IEnumerable<T>, Error>` | `IEnumerable` (lazy no funciona bien con EF) |
| Siempre incluir `CancellationToken` | Ultimo parametro |
| XML doc en cada metodo | `/// <summary>` |

## Paso 3: Crear ErrorBuilder (si es necesario)

Si la entidad tiene errores de negocio especificos:

**Ubicacion:** `Domain/Common/Results/Errors/StoreErrorBuilder.cs`

```csharp
public static class StoreErrorBuilder
{
    public static Error NotFound(int id, string entity) => Error.CreateInstance(
        "StoreRecordNotFound",
        $"{entity} with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error NoRecordsFound(string entity) => Error.CreateInstance(
        "StoreRecordNotFound",
        $"No {entity} records were found.",
        HttpStatusCode.NotFound);
}
```

### Reglas para ErrorBuilder

- `NotFound` → `HttpStatusCode.NotFound` (404)
- `CreationFailed` → `HttpStatusCode.InternalServerError` (500)
- Usar `Error.CreateInstance(code, description, httpStatusCode)`
- Codigos de error en formato `PascalCase`: `"StoreRecordNotFound"`

## Verificacion

Antes de continuar a Application, verifica:

- [ ] Entidad con `sealed class`, propiedades publicas
- [ ] Puerto con `Result<T, Error>` en cada metodo
- [ ] Puerto con `CancellationToken` en cada metodo
- [ ] ErrorBuilder con metodos estaticos (si aplica)
- [ ] Sin referencias a otros proyectos (solo Domain internals)
