namespace Backend.PriceComparison.Application.Store.Dtos;

/// <summary>
/// Objeto de transferencia de datos (DTO) que representa un registro de historial de precio de un producto en un supermercado.
/// </summary>
public sealed class PriceHistoryDto
{
    /// <summary>
    /// Identificador único del registro de historial.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Identificador del supermercado.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Nombre del supermercado.
    /// </summary>
    public string? StoreName { get; set; }

    /// <summary>
    /// Precio del producto en este supermercado.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Fecha de registro del precio.
    /// </summary>
    public DateTime Date { get; set; }
}
