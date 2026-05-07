namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class ProductSaleDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public double ProductPrice { get; set; }
    public int SaleId { get; set; }
}
