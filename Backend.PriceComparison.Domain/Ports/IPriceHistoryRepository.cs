using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

/// <summary>
/// Interfaz para el repositorio de historial de precios, definiendo el contrato para registrar y comparar precios.
/// </summary>
public interface IPriceHistoryRepository
{
    /// <summary>
    /// Obtiene la lista de registros de precios asociados a un producto en diferentes supermercados, ordenados de forma ascendente.
    /// </summary>
    /// <param name="productId">Identificador del producto.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Colección de registros de historial de precios del producto.</returns>
    Task<Result<IEnumerable<PriceHistoryEntity>, Error>> GetPricesByProductAsync(int productId, CancellationToken cancellationToken);

    /// <summary>
    /// Registra una nueva entrada de precio de un producto en un supermercado.
    /// </summary>
    /// <param name="entity">Entidad de historial de precio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Resultado vacío si es exitoso o un error en caso de fallo.</returns>
    Task<Result<VoidResult, Error>> CreateAsync(PriceHistoryEntity entity, CancellationToken cancellationToken);
}
