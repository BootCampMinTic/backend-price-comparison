namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class SaleDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public int StoreId { get; set; }
    public string? StoreName { get; set; }
    public int StateId { get; set; }
    public string? StateDescription { get; set; }
    public DateTime Date { get; set; }
    public List<ProductSaleDto> Products { get; set; } = new();
}
