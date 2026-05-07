# ProductSale Entity Specification

## Database Schema

**Table:** `product_sale`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_product_sale` | `int(11)` | NO | PRI | **NO AUTO_INCREMENT** |
| `id_product` | `int(11)` | NO | MUL | FK → product |
| `id_sale` | `int(11)` | NO | MUL | FK → sale |

> **CRITICAL:** The primary key `id_product_sale` does **NOT** have `AUTO_INCREMENT`.
> IDs must be generated manually by the application until this is fixed.

**Recommended fix:**
```sql
ALTER TABLE product_sale MODIFY COLUMN id_product_sale INT AUTO_INCREMENT;
```

**Relationships:**
- **FK:** `id_product` → `product.id_product`
- **FK:** `id_sale` → `sale.id_sale`

---

## C# Entity

```csharp
public sealed class ProductSaleEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public int SaleId { get; set; }
    public SaleEntity? Sale { get; set; }
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<ProductSaleEntity>(entity =>
{
    entity.ToTable("product_sale");
    entity.Property(e => e.Id).HasColumnName("id_product_sale");
    entity.Property(e => e.ProductId).HasColumnName("id_product");
    entity.Property(e => e.SaleId).HasColumnName("id_sale");
    entity.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
    // Sale relationship is configured from SaleEntity side:
    // entity.HasMany(e => e.ProductSales).WithOne(ps => ps.Sale).HasForeignKey(ps => ps.SaleId);
});
```

---

## Port

```csharp
public interface IProductSaleRepository
{
    Task<Result<IEnumerable<ProductSaleEntity>, Error>> GetBySaleIdAsync(int saleId, CancellationToken ct);
    Task<Result<VoidResult, Error>> CreateAsync(ProductSaleEntity entity, CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/ProductSaleRepository.cs`
- `GetBySaleIdAsync`: `AsNoTracking().Include(ps => ps.Product).Where(ps => ps.SaleId == saleId)`
- `CreateAsync`: standard

**Mock:** `MockProductSaleRepository.cs` — minimal stub.

---

## Application

ProductSale entities are **not created via their own command**. They are created as child entities of `SaleEntity` in the `CreateSaleCommandHandler`:

```csharp
int psCounter = 1;
foreach (var productId in request.ProductIds)
{
    sale.ProductSales.Add(new ProductSaleEntity
    {
        Id = psCounter++,       // manual ID generation (no AUTO_INCREMENT)
        ProductId = productId,
        Sale = sale             // EF Core sets SaleId via navigation
    });
}
```

When the `SaleRepository.CreateAsync()` saves the aggregate, EF Core automatically saves all `ProductSaleEntity` children in the same transaction.

---

## API Endpoints

No dedicated endpoints. ProductSales are returned as nested data within the `GET /api/v1/sales/{id}` response.

---

## Data Flow

```
POST /api/v1/sales
  Body: { userId, storeId, stateId, date, productIds: [1, 2, 3] }
  ↓
CreateSaleCommandHandler
  ↓ Maps command → SaleEntity
  ↓ Creates ProductSaleEntity per productId
  ↓ sale.ProductSales.Add(...)
  ↓
SaleRepository.CreateAsync(sale)
  ↓ EF Core saves sale + all product_sale rows in one transaction
  ↓
Cache invalidation: sales:*
  ↓
Response: { success: true, message: "Sale created successfully" }
```

---

## Notes

1. **AUTO_INCREMENT fix is critical.** Without it, concurrent sale creation will cause PK collisions. Run the ALTER TABLE as soon as possible.
2. ProductSale is a pure junction table (many-to-many between Sale and Product).
3. No cache keys needed — ProductSales are always read as part of a Sale (via `GetSaleByIdQuery`).
4. The `Product` navigation property is eager-loaded via `ThenInclude()` in `SaleRepository.GetByIdAsync`.
