# Guia: Capa Infrastructure

Esta guia explica como crear repositorios en la capa **Infrastructure**.

## Paso 1: Crear el repositorio

**Ubicacion:** `Infrastructure.Persistence.Mysql/{Feature}/Repositories/{Entity}Repository.cs`

```csharp
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

    public async Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _context.Products
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.CategoryProduct)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No products found for page {PageNumber} size {PageSize}",
                pageNumber, pageSize);
            return StoreErrorBuilder.NoRecordsFound("product");
        }

        return entities;
    }

    public async Task<Result<ProductEntity, Error>> GetByIdAsync(
        int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.CategoryProduct)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("Product not found by id {ProductId}", id);
            return StoreErrorBuilder.NotFound(id, "Product");
        }

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(
        ProductEntity entity, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist product {ProductName}", entity.Name);
            return StoreErrorBuilder.CreationFailed("product");
        }

        logger.LogInformation("Product created with id {ProductId}", entity.Id);
        return VoidResult.Instance;
    }
}
```

### Reglas para repositorios

| Regla | Ejemplo |
|-------|---------|
| `internal sealed class` | Oculto fuera del assembly |
| Primary constructor con `ClientDbContext` + `ILogger<T>` | DI limpio |
| `AsNoTracking()` en TODAS las lecturas | Optimizacion EF Core |
| `Include()` para cada FK navigation property | Evita N+1 |
| `Skip()/Take()` para paginacion | Paginacion estandar |
| `LogDebug` para "no encontrado" | No es error, es flujo normal |
| `LogWarning` para fallos de persistencia | Alerta real |
| Retornar `StoreErrorBuilder.*` en errores | Errores de dominio |
| No usar excepciones para flujo de negocio | El Result pattern maneja errores |

## Paso 2: Registrar en el DbContext

**Archivo:** `Infrastructure/Context/ClientDbContext.cs`

```csharp
public class ClientDbContext(DbContextOptions options) : DbContext(options)
{
    // DbSets existentes...
    public DbSet<ClientLegalPosEntity> ClientLegalPos { get; set; }

    // NUEVOS DbSets:
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<StoreEntity> Stores { get; set; }
    // ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
        StoreEntityConfiguration(modelBuilder);  // ← Configuracion de mapeo
    }
}
```

### Configuracion de mapeo (fluent API)

```csharp
private static void StoreEntityConfiguration(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<ProductEntity>(entity =>
    {
        entity.ToTable("product");                              // Nombre tabla MySQL
        entity.Property(e => e.Id).HasColumnName("id_product"); // Mapeo columna
        entity.Property(e => e.Name).HasColumnName("name");
        entity.Property(e => e.Price).HasColumnName("price");
        entity.Property(e => e.StoreId).HasColumnName("id_store");
        entity.Property(e => e.CategoryProductId).HasColumnName("id_category_product");

        // FKs
        entity.HasOne(e => e.Store)
            .WithMany()
            .HasForeignKey(e => e.StoreId);

        entity.HasOne(e => e.CategoryProduct)
            .WithMany()
            .HasForeignKey(e => e.CategoryProductId);
    });
}
```

## Paso 3: Registrar en DI

**Archivo:** `Infrastructure/DependencyInjectionService.cs`

### Path real (con MySQL)

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

### Path mock (sin MySQL, `UseMockInfrastructure=true`)

```csharp
services.AddScoped<IProductRepository, MockProductRepository>();
```

## Paso 4: Crear Mock Repository

**Ubicacion:** `Infrastructure/Mock/Mock{Entity}Repository.cs`

```csharp
namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;

public class MockProductRepository : IProductRepository
{
    private readonly List<ProductEntity> _data = new()
    {
        new() { Id = 1, Name = "Arroz (1kg)", Price = 3500,
            StoreId = 1, CategoryProductId = 1,
            Store = new() { Id = 1, Name = "Supermercado La Economia" },
            CategoryProduct = new() { Id = 1, Description = "Abarrotes" } }
    };

    public Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(
        int pageNumber, int pageSize, CancellationToken ct)
    {
        var paged = _data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        if (paged.Count == 0)
            return Task.FromResult<Result<IEnumerable<ProductEntity>, Error>>(
                StoreErrorBuilder.NoRecordsFound("product"));
        return Task.FromResult<Result<IEnumerable<ProductEntity>, Error>>(paged);
    }

    // ... GetByIdAsync, CreateAsync, etc.
}
```

### Reglas para mocks

| Regla |
|-------|
| Retornar datos hardcoded realistas |
| Incluir navigation properties populadas |
| `CreateAsync` asigna ID secuencial (`_data.Count + 1`) |
| Mismo comportamiento de errores que el repo real |

## Verificacion

- [ ] Repositorio implementa el puerto del Domain
- [ ] `AsNoTracking()` en todos los reads
- [ ] `Include()` para cada FK
- [ ] `LogDebug` para "no encontrado", `LogWarning` para fallos
- [ ] DbSet agregado en `ClientDbContext`
- [ ] Mapeo de columnas y FKs en `StoreEntityConfiguration`
- [ ] Registrado en DI (path real + path mock)
- [ ] Mock repository creado con datos de ejemplo
