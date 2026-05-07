namespace Backend.PriceComparison.Domain.Store.Entities;

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
