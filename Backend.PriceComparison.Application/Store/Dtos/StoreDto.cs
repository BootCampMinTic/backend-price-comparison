namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class StoreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int CategoryStoreId { get; set; }
    public string? CategoryStoreDescription { get; set; }
}
