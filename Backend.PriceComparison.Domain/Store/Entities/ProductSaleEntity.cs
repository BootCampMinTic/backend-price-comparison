namespace Backend.PriceComparison.Domain.Store.Entities;

public sealed class ProductSaleEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public int SaleId { get; set; }
    public SaleEntity? Sale { get; set; }
}
