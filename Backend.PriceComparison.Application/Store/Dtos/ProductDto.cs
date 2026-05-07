namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int StoreId { get; set; }
    public string? StoreName { get; set; }
    public int CategoryProductId { get; set; }
    public string? CategoryProductDescription { get; set; }
}
