# CategoryStore Entity Specification

## Database Schema

**Table:** `category_store`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_category_store` | `int(11)` | NO | PRI | auto_increment |
| `description` | `varchar(45)` | NO | UNI | |

**Relationships:**
- `store.id_category_store` → `category_store.id_category_store` (one category has many stores)

---

## C# Entity

**Namespace:** `Backend.PriceComparison.Domain.Store.Entities`

```csharp
public sealed class CategoryStoreEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<CategoryStoreEntity>(entity =>
{
    entity.ToTable("category_store");
    entity.Property(e => e.Id).HasColumnName("id_category_store");
    entity.Property(e => e.Description).HasColumnName("description");
});
```

---

## Port

```csharp
public interface ICategoryStoreRepository
{
    Task<Result<IEnumerable<CategoryStoreEntity>, Error>> GetAllAsync(CancellationToken ct);
    Task<Result<CategoryStoreEntity, Error>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<VoidResult, Error>> CreateAsync(CategoryStoreEntity entity, CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/CategoryStoreRepository.cs`
- Same pattern as `CategoryProductRepository`

**Mock:** `MockCategoryStoreRepository.cs` - Hardcoded (Supermercado, Tienda de barrio, Mayorista).

---

## Application

### Queries

**GetAll:** Cache key `CacheKeys.CategoryStoresAll` (`"categorystores:all"`)

**GetById:** `GetCategoryStoreByIdQuery(int Id)` — no cache

### Command

**Create:** `CreateCategoryStoreCommand(string Description)`
- Handler invalidates `CacheKeys.CategoryStoresAll` on success

**Validator:**
- `Description.NotEmpty()`

---

## API Endpoints

```
GET    /api/v1/category-stores         → GetAll
GET    /api/v1/category-stores/{id}    → GetById
POST   /api/v1/category-stores         → Create
```

Tags: `Catalog`.

---

## Cache

| Key | Pattern | Invalidated by |
|-----|---------|----------------|
| `categorystores:all` | Constant | Create command |

---

## Notes

- Used as FK by `StoreEntity`.
- `UNIQUE` constraint on `description` — duplicate descriptions rejected by MySQL.
