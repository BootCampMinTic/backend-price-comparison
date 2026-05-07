namespace Backend.PriceComparison.Domain.Store.Entities;

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
