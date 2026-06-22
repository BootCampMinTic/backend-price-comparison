namespace Backend.PriceComparison.Domain.Store.Entities;

public sealed class SaleEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public double Total { get; set; }
    public int UserId { get; set; }
    public int StateId { get; set; }
}
