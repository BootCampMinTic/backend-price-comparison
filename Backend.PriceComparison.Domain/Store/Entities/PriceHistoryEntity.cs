using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Store.Entities;

/// <summary>
/// Representa el historial de precios de un producto específico en un supermercado determinado.
/// </summary>
public sealed class PriceHistoryEntity
{
    /// <summary>
    /// Identificador único del registro de historial de precio.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto asociado.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Navegación al producto asociado.
    /// </summary>
    public ProductEntity? Product { get; set; }

    /// <summary>
    /// Identificador del supermercado (tienda) asociado.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Navegación al supermercado asociado.
    /// </summary>
    public StoreEntity? Store { get; set; }

    /// <summary>
    /// Precio registrado para el producto en el supermercado.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Fecha y hora en la que se registró el precio.
    /// </summary>
    public DateTime Date { get; set; }
}
