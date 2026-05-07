# Store Entity Specification

## Database Schema

**Table:** `store`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_store` | `int(11)` | NO | PRI | auto_increment |
| `name` | `varchar(100)` | NO | | |
| `address` | `varchar(100)` | NO | | |
| `phone` | `varchar(45)` | YES | | |
| `id_category_store` | `int(11)` | NO | MUL | FK → category_store |

**Relationships:**
- **FK:** `id_category_store` → `category_store.id_category_store`
- **HasMany:** `product` (products sold in this store)
- **HasMany:** `sale` (sales made in this store)

---

## C# Entity

```csharp
public sealed class StoreEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int CategoryStoreId { get; set; }
    public CategoryStoreEntity? CategoryStore { get; set; }
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<StoreEntity>(entity =>
{
    entity.ToTable("store");
    entity.Property(e => e.Id).HasColumnName("id_store");
    entity.Property(e => e.Name).HasColumnName("name");
    entity.Property(e => e.Address).HasColumnName("address");
    entity.Property(e => e.Phone).HasColumnName("phone");
    entity.Property(e => e.CategoryStoreId).HasColumnName("id_category_store");
    entity.HasOne(e => e.CategoryStore).WithMany().HasForeignKey(e => e.CategoryStoreId);
});
```

---

## Port

```csharp
public interface IStoreRepository
{
    Task<Result<IEnumerable<StoreEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<Result<StoreEntity, Error>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<VoidResult, Error>> CreateAsync(StoreEntity entity, CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/StoreRepository.cs`
- `GetAllAsync`: `AsNoTracking().Include(s => s.CategoryStore).Skip/Take`
- `GetByIdAsync`: `AsNoTracking().Include(s => s.CategoryStore).FirstOrDefaultAsync()`
- `CreateAsync`: `AddAsync()` + `SaveChangesAsync()`, logs Name on warning

**Mock:** `MockStoreRepository.cs` — Hardcoded (Supermercado La Economia, Tienda Don Pedro).

---

## Application

### Queries

**GetAll:**
```csharp
public record GetAllStoresQuery : IRequest<Result<IEnumerable<StoreDto>, Error>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```
- Cache key: `CacheKeys.StoresPage(page, size)` → `"stores:page:{n}:size:{s}"`
- Uses AutoMapper: `StoreEntity` → `StoreDto` (maps `CategoryStore.Description` → `CategoryStoreDescription`)

**GetById:**
```csharp
public record GetStoreByIdQuery(int Id) : IRequest<Result<StoreDto, Error>>;
```
- Cache key: `CacheKeys.StoreById(id)` → `"store:{id}"`

### Command

**Create:**
```csharp
public record CreateStoreCommand(
    string Name, string Address, string? Phone, int CategoryStoreId
) : IRequest<Result<VoidResult, Error>>;
```

**Handler:**
1. Maps command → `StoreEntity` (ignores `Id`, `CategoryStore` nav prop)
2. Calls `_repo.CreateAsync(entity)`
3. On success: `_cacheService.RemoveByPrefixAsync(CacheKeys.StoresPrefix, ct)` → removes all `"stores:*"` keys

**Validator:**
- `Name.NotEmpty()`
- `Address.NotEmpty()`
- `CategoryStoreId.GreaterThan(0)`

---

## API Endpoints

```
GET    /api/v1/stores?pageNumber=1&pageSize=10   → GetAll (paginado)
GET    /api/v1/stores/{id}                         → GetById
POST   /api/v1/stores                              → Create
```

Tags: `Store`. All require `Authorization: Bearer <token>`.

**Success Response (POST):**
```json
{ "success": true, "message": "Store created successfully", "data": {} }
```

---

## Cache Strategy

| Key Pattern | Example | Invalidated by |
|-------------|---------|----------------|
| `stores:page:{n}:size:{s}` | `stores:page:1:size:10` | Prefix `stores:` on Create |
| `store:{id}` | `store:5` | Direct key removal on Create |

---

## StoreDto

```csharp
public sealed class StoreDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string? Phone { get; set; }
    public int CategoryStoreId { get; set; }
    public string? CategoryStoreDescription { get; set; }  // flattened from FK
}
```

AutoMapper rule in `StoreProfile`:
```csharp
.ForMember(dest => dest.CategoryStoreDescription,
    opt => opt.MapFrom(src => src.CategoryStore != null ? src.CategoryStore.Description : null))
```

---

## Notes

- CategoryStore is loaded via `Include()` in repository queries.
- Paginated queries use standard `Skip((page-1)*size).Take(size)`.
- All read queries use `AsNoTracking()`.
