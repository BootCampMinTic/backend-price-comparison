namespace Backend.PriceComparison.Domain.Store.Entities;

public sealed class StoreEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int CategoryStoreId { get; set; }
    public CategoryStoreEntity? CategoryStore { get; set; }
}
