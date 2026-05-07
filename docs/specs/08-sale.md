# Sale Entity Specification

## Database Schema

**Table:** `sale`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_sale` | `int(11)` | NO | PRI | auto_increment |
| `id_user` | `int(11)` | NO | MUL | FK → user |
| `id_store` | `int(11)` | NO | MUL | FK → store |
| `id_state` | `int(11)` | NO | MUL | FK → state |
| `date` | `datetime` | NO | | |

**Relationships:**
- **FK:** `id_user` → `user.id_user`
- **FK:** `id_store` → `store.id_store`
- **FK:** `id_state` → `state.id_state`
- **HasMany:** `product_sale` (products in this sale)

---

## C# Entity

```csharp
public sealed class SaleEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserEntity? User { get; set; }
    public int StoreId { get; set; }
    public StoreEntity? Store { get; set; }
    public int StateId { get; set; }
    public StateEntity? State { get; set; }
    public DateTime Date { get; set; }
    public ICollection<ProductSaleEntity> ProductSales { get; set; } = new List<ProductSaleEntity>();
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<SaleEntity>(entity =>
{
    entity.ToTable("sale");
    entity.Property(e => e.Id).HasColumnName("id_sale");
    entity.Property(e => e.UserId).HasColumnName("id_user");
    entity.Property(e => e.StoreId).HasColumnName("id_store");
    entity.Property(e => e.StateId).HasColumnName("id_state");
    entity.Property(e => e.Date).HasColumnName("date");
    entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
    entity.HasOne(e => e.Store).WithMany().HasForeignKey(e => e.StoreId);
    entity.HasOne(e => e.State).WithMany().HasForeignKey(e => e.StateId);
    entity.HasMany(e => e.ProductSales).WithOne(ps => ps.Sale).HasForeignKey(ps => ps.SaleId);
});
```

---

## Port

```csharp
public interface ISaleRepository
{
    Task<Result<IEnumerable<SaleEntity>, Error>> GetAllAsync(int page, int size, CancellationToken ct);
    Task<Result<SaleEntity, Error>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<VoidResult, Error>> CreateAsync(SaleEntity entity, CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/SaleRepository.cs`
- `GetAllAsync`: `AsNoTracking().Include(s => s.User).Include(s => s.Store).Include(s => s.State).Skip/Take`
- `GetByIdAsync`: Same includes + `.Include(s => s.ProductSales).ThenInclude(ps => ps.Product)` — loads full nested graph
- `CreateAsync`: Saves the entire aggregate (Sale + ProductSales) in one transaction via `SaveChangesAsync()`

**Mock:** `MockSaleRepository.cs` — 1 sale with 2 products (Arroz + Leche).

---

## Application

### Queries

**GetAll:**
```csharp
public record GetAllSalesQuery : IRequest<Result<IEnumerable<SaleDto>, Error>>
{ PageNumber = 1; PageSize = 10; }
```
Cache: `CacheKeys.SalesPage(page, size)` → `"sales:page:{n}:size:{s}"`

**GetById:**
```csharp
public record GetSaleByIdQuery(int Id) : IRequest<Result<SaleDto, Error>>;
```
Cache: `CacheKeys.SaleById(id)` → `"sale:{id}"`

### Command

**Create:**
```csharp
public record CreateSaleCommand(
    int UserId, int StoreId, int StateId, DateTime Date, List<int> ProductIds
) : IRequest<Result<VoidResult, Error>>;
```

**Handler:**
1. Maps command → `SaleEntity` (ignores Id, nav props, ProductSales)
2. Generates sequential IDs for ProductSale entities:
```csharp
int psCounter = 1;
foreach (var productId in request.ProductIds)
{
    sale.ProductSales.Add(new ProductSaleEntity
    {
        Id = psCounter++,
        ProductId = productId,
        Sale = sale
    });
}
```
3. Calls `_saleRepository.CreateAsync(sale)` — EF Core saves Sale + all ProductSales in one transaction
4. On success: `_cacheService.RemoveByPrefixAsync(CacheKeys.SalesPrefix, ct)`

**Validator:**
- `UserId.GreaterThan(0)`
- `StoreId.GreaterThan(0)`
- `StateId.GreaterThan(0)`
- `ProductIds.NotEmpty()`

---

## API Endpoints

```
GET    /api/v1/sales?pageNumber=1&pageSize=10   → GetAll (paginado)
GET    /api/v1/sales/{id}                         → GetById (con productos)
POST   /api/v1/sales                              → Create (con ProductIds[])
```

Tags: `Sale`.

**POST Body Example:**
```json
{
  "userId": 2,
  "storeId": 1,
  "stateId": 1,
  "date": "2026-05-06T10:00:00Z",
  "productIds": [1, 2, 3]
}
```

---

## SaleDto

```csharp
public sealed class SaleDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }          // flattened
    public int StoreId { get; set; }
    public string? StoreName { get; set; }         // flattened
    public int StateId { get; set; }
    public string? StateDescription { get; set; }  // flattened
    public DateTime Date { get; set; }
    public List<ProductSaleDto> Products { get; set; }
}
```

**ProductSaleDto:**
```csharp
public sealed class ProductSaleDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public double ProductPrice { get; set; }
    public int SaleId { get; set; }
}
```

AutoMapper config:
```csharp
CreateMap<SaleEntity, SaleDto>()
    .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.Name : null))
    .ForMember(d => d.StoreName, o => o.MapFrom(s => s.Store != null ? s.Store.Name : null))
    .ForMember(d => d.StateDescription, o => o.MapFrom(s => s.State != null ? s.State.Description : null))
    .ForMember(d => d.Products, o => o.MapFrom(s => s.ProductSales));

CreateMap<ProductSaleEntity, ProductSaleDto>()
    .ForMember(d => d.ProductName, o => o.MapFrom(ps => ps.Product != null ? ps.Product.Name : null))
    .ForMember(d => d.ProductPrice, o => o.MapFrom(ps => ps.Product != null ? ps.Product.Price : 0));
```

---

## Cache Strategy

| Key Pattern | Invalidated by |
|-------------|----------------|
| `sales:*` (all sale caches) | Create command removes by prefix |

---

## Notes

- This is the core transactional entity. One sale = one shopping trip at one store.
- ProductSale IDs are generated manually because the MySQL column lacks `AUTO_INCREMENT`. See [ProductSale spec](./08-productsale.md).
- GetById returns the full nested graph: Sale → User, Store, State, ProductSales → Product.
