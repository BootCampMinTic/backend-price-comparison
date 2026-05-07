# CategoryProduct Entity Specification

## Database Schema

**Table:** `category_product`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_category_product` | `int(11)` | NO | PRI | auto_increment |
| `descrption` | `varchar(45)` | NO | | |

> **WARNING:** Column name has a typo in MySQL: `descrption` instead of `description`.
> The C# entity maps via `[Column("descrption")]`.

**Relationships:**
- `product.id_category_product` → `category_product.id_category_product`

---

## C# Entity

**Namespace:** `Backend.PriceComparison.Domain.Store.Entities`

```csharp
public sealed class CategoryProductEntity
{
    public int Id { get; set; }

    [Column("descrption")]
    public string Description { get; set; } = string.Empty;
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<CategoryProductEntity>(entity =>
{
    entity.ToTable("category_product");
    entity.Property(e => e.Id).HasColumnName("id_category_product");
    entity.Property(e => e.Description).HasColumnName("descrption"); // typo en MySQL
});
```

---

## Port (Domain Interface)

**File:** `Domain/Ports/ICategoryProductRepository.cs`

```csharp
public interface ICategoryProductRepository
{
    Task<Result<IEnumerable<CategoryProductEntity>, Error>> GetAllAsync(CancellationToken ct);
    Task<Result<CategoryProductEntity, Error>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<VoidResult, Error>> CreateAsync(CategoryProductEntity entity, CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/CategoryProductRepository.cs`
- Injects `ClientDbContext` + `ILogger<CategoryProductRepository>`
- `GetAllAsync`: `AsNoTracking()`, returns `StoreErrorBuilder.NoRecordsFound("category product")` on empty
- `GetByIdAsync`: `AsNoTracking().FirstOrDefaultAsync()`, returns `StoreErrorBuilder.NotFound(id, "Category product")`
- `CreateAsync`: `AddAsync()` + `SaveChangesAsync()`, logs info/warning

**Mock:** `MockCategoryProductRepository.cs` - Hardcoded list (Abarrotes, Lacteos, Bebidas).

---

## Application

### Queries

**GetAll:**
```csharp
public record GetAllCategoryProductsQuery : IRequest<Result<IEnumerable<CategoryProductDto>, Error>>;
```
Cache key: `CacheKeys.CategoryProductsAll` (`"categoryproducts:all"`)

**GetById:**
```csharp
public record GetCategoryProductByIdQuery(int Id) : IRequest<Result<CategoryProductDto, Error>>;
```
No cache (single record).

### Commands

**Create:**
```csharp
public record CreateCategoryProductCommand(string Description) : IRequest<Result<VoidResult, Error>>;
```

**Handler** - Injects `ICategoryProductRepository` + `IMapper` + `ICacheService`:
1. Maps command → `CategoryProductEntity`
2. Calls `_repo.CreateAsync(entity)`
3. On success: `_cacheService.RemoveAsync(CacheKeys.CategoryProductsAll, ct)`
4. Returns `VoidResult`

**Validator** (`CreateCategoryProductCommandValidator`):
- `Description.NotEmpty()`

---

## API Endpoints

```
GET    /api/v1/category-products        → GetAll
GET    /api/v1/category-products/{id}   → GetById
POST   /api/v1/category-products        → Create
```

Tags: `Catalog`.

---

## Cache

| Key | Pattern | Invalidated by |
|-----|---------|----------------|
| `categoryproducts:all` | Constant | Create command |

---

## Notes

- CRUD-ready entity (full create supported, delete/update can be added later).
- The column typo `descrption` should be fixed via `ALTER TABLE category_product CHANGE COLUMN descrption description VARCHAR(45) NOT NULL;` when possible.
