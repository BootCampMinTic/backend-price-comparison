namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class SaleDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public double Total { get; set; }
    public int UserId { get; set; }
    public int StateId { get; set; }
}
