# State Entity Specification

## Database Schema

**Table:** `state`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_state` | `int(11)` | NO | PRI | auto_increment |
| `description` | `varchar(45)` | NO | | |

**Relationships:**
- `sale.id_state` → `state.id_state` (one state has many sales)

**Seed Data:**
| id_state | description |
|----------|-------------|
| 1 | Pendiente |
| 2 | Completada |
| 3 | Cancelada |

---

## C# Entity

**Namespace:** `Backend.PriceComparison.Domain.Store.Entities`

```csharp
public sealed class StateEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
}
```

**EF Core Mapping (in `ClientDbContext.StoreEntityConfiguration`):**

```csharp
modelBuilder.Entity<StateEntity>(entity =>
{
    entity.ToTable("state");
    entity.Property(e => e.Id).HasColumnName("id_state");
    entity.Property(e => e.Description).HasColumnName("description");
});
```

---

## Port (Domain Interface)

**File:** `Domain/Ports/IStateRepository.cs`

```csharp
public interface IStateRepository
{
    Task<Result<IEnumerable<StateEntity>, Error>> GetAllAsync(CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/StateRepository.cs`

```csharp
internal sealed class StateRepository(ClientDbContext context) : IStateRepository
{
    public async Task<Result<IEnumerable<StateEntity>, Error>> GetAllAsync(CancellationToken ct)
    {
        var entities = await _context.States.AsNoTracking().ToListAsync(ct);
        if (entities.Count == 0)
            return StoreErrorBuilder.NoRecordsFound("state");
        return entities;
    }
}
```

**Mock:** `Infrastructure/Mock/MockStateRepository.cs` - Returns hardcoded list (Pendiente, Completada, Cancelada).

---

## Application

### Query
**File:** `Application/Store/Queries/State/GetAllStatesQuery.cs`

```csharp
public record GetAllStatesQuery : IRequest<Result<IEnumerable<StateDto>, Error>>;
```

### Handler
**File:** `Application/Store/Queries/State/GetAllStatesQueryHandler.cs`

- Injects `IStateRepository` + `ICacheService`
- Cache key: `CacheKeys.StatesAll` (`"states:all"`)
- Cache-aside pattern: try Redis → miss → repo → cache

### DTO

```csharp
public sealed class StateDto { int Id; string Description; }
```

---

## API Endpoint

**File:** `Api/Endpoints/CatalogEndpoints.cs`

```
GET /api/v1/states
```

Returns `IEnumerable<StateDto>`. No auth required in dev mode. Tags: `Catalog`.

---

## Cache

| Key | Pattern | Invalidated by |
|-----|---------|----------------|
| `states:all` | Constant | N/A (read-only catalog) |

---

## DI Registration

**Infrastructure DI:**
```csharp
services.AddScoped<IStateRepository, StateRepository>();       // real
services.AddScoped<IStateRepository, MockStateRepository>();   // mock mode
```

**Application DI:** Auto-registered by MediatR via `Assembly.GetExecutingAssembly()`.

---

## Notes

- Read-only catalog entity. No create/update/delete endpoints.
- Used as FK by `SaleEntity`.
