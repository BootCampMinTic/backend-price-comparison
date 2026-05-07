using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.PriceComparison.Domain.Store.Entities;

public sealed class CategoryProductEntity
{
    public int Id { get; set; }

    [Column("descrption")]
    public string Description { get; set; } = string.Empty;
}
