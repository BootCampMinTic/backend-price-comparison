# Product Entity Specification

## Database Schema

**Table:** `product`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_product` | `int(11)` | NO | PRI | auto_increment |
| `name` | `varchar(45)` | NO | | |
| `price` | `double` | YES | | |
| `id_store` | `int(11)` | NO | MUL | FK → store |
| `id_category_product` | `int(11)` | NO | MUL | FK → category_product |

**Relationships:**
- **FK:** `id_store` → `store.id_store`
- **FK:** `id_category_product` → `category_product.id_category_product`
- **HasMany:** `product_sale` (this product in sales)

---

## C# Entity

```csharp
public sealed class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    public int CategoryProductId { get; set; }
    public CategoryProductEntity? CategoryProduct { get; set; }
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<ProductEntity>(entity =>
{
    entity.ToTable("product");
    entity.Property(e => e.Id).HasColumnName("id_product");
    entity.Property(e => e.Name).HasColumnName("name");
    entity.Property(e => e.Price).HasColumnName("price");
    entity.Property(e => e.StoreId).HasColumnName("id_store");
    entity.Property(e => e.CategoryProductId).HasColumnName("id_category_product");
    entity.HasOne(e => e.Store).WithMany().HasForeignKey(e => e.StoreId);
    entity.HasOne(e => e.CategoryProduct).WithMany().HasForeignKey(e => e.CategoryProductId);
});
```

---

## Port

```csharp
public interface IProductRepository
{
    Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(int page, int size, CancellationToken ct);
    Task<Result<IEnumerable<ProductEntity>, Error>> GetByStoreAsync(int storeId, int page, int size, CancellationToken ct);
    Task<Result<ProductEntity, Error>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<VoidResult, Error>> CreateAsync(ProductEntity entity, CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/ProductRepository.cs`
- All reads use `AsNoTracking().Include(p => p.Store).Include(p => p.CategoryProduct)`
- `GetByStoreAsync`: filters by `StoreId`
- `CreateAsync`: standard AddAsync + SaveChangesAsync

**Mock:** `MockProductRepository.cs` — 3 products (Arroz, Leche, Gaseosa) across 2 stores.

---

## Application

### Queries

**GetAll:**
```csharp
public record GetAllProductsQuery : IRequest<Result<IEnumerable<ProductDto>, Error>>
{ PageNumber = 1; PageSize = 10; }
```
Cache: `CacheKeys.ProductsPage(page, size)` → `"products:page:{n}:size:{s}"`

**GetById:** `GetProductByIdQuery(int Id)` — Cache: `CacheKeys.ProductById(id)` → `"product:{id}"`

**GetByStore:**
```csharp
public record GetProductsByStoreQuery(int StoreId, int PageNumber, int PageSize) : ...
```
Cache: `CacheKeys.ProductsByStore(storeId, page, size)` → `"products:store:{id}:page:{n}:size:{s}"`

### Command

**Create:**
```csharp
public record CreateProductCommand(
    string Name, double Price, int StoreId, int CategoryProductId
) : IRequest<Result<VoidResult, Error>>;
```

**Handler:** Invalidates `CacheKeys.ProductsPrefix` (`"products"`) on success.

**Validator:**
- `Name.NotEmpty()`
- `Price.GreaterThan(0)`
- `StoreId.GreaterThan(0)`
- `CategoryProductId.GreaterThan(0)`

---

## API Endpoints

```
GET    /api/v1/products?pageNumber=1&pageSize=10        → GetAll (paginado)
GET    /api/v1/products/{id}                              → GetById
GET    /api/v1/stores/{storeId}/products?pageNumber=1&..  → GetByStore (paginado)
POST   /api/v1/products                                   → Create
```

Tags: `Product`.

---

## ProductDto

```csharp
public sealed class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int StoreId { get; set; }
    public string? StoreName { get; set; }              // flattened
    public int CategoryProductId { get; set; }
    public string? CategoryProductDescription { get; set; } // flattened
}
```

---

## Cache Strategy

| Key Pattern | Invalidated by |
|-------------|----------------|
| `products:*` (all product caches) | Create command removes by prefix |

---

## Notes

- Price is `double` in MySQL — consider `decimal` for financial precision in future.
- Products are core to the price comparison feature. The `GetByStore` endpoint is used to compare prices across stores.
