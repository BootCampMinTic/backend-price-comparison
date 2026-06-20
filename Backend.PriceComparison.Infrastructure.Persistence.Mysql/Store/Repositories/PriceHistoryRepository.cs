using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

/// <summary>
/// Implementación con Entity Framework Core del repositorio de historial de precios para MySQL.
/// </summary>
internal sealed class PriceHistoryRepository(
    ClientDbContext context,
    ILogger<PriceHistoryRepository> logger) : IPriceHistoryRepository
{
    private readonly ClientDbContext _context = context;

    /// <summary>
    /// Consulta el histórico de precios de un producto, cargando relaciones con producto y supermercado, y ordenado ascendentemente por precio.
    /// </summary>
    public async Task<Result<IEnumerable<PriceHistoryEntity>, Error>> GetPricesByProductAsync(int productId, CancellationToken cancellationToken)
    {
        var entities = await _context.Set<PriceHistoryEntity>()
            .AsNoTracking()
            .Include(ph => ph.Product)
            .Include(ph => ph.Store)
            .Where(ph => ph.ProductId == productId)
            .OrderBy(ph => ph.Price) // Ordenar de menor a mayor precio para comparar
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No se encontró historial de precios para el producto con ID {ProductId}", productId);
            return StoreErrorBuilder.NoRecordsFound("historial de precios");
        }

        return entities;
    }

    /// <summary>
    /// Guarda un nuevo registro de precio en la base de datos de MySQL.
    /// </summary>
    public async Task<Result<VoidResult, Error>> CreateAsync(PriceHistoryEntity entity, CancellationToken cancellationToken)
    {
        await _context.Set<PriceHistoryEntity>().AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("No se pudo persistir el registro de precio para el producto {ProductId} en la tienda {StoreId}", entity.ProductId, entity.StoreId);
            return StoreErrorBuilder.CreationFailed("historial de precios");
        }

        logger.LogInformation("Historial de precio registrado exitosamente con ID {PriceHistoryId}", entity.Id);
        return VoidResult.Instance;
    }
}
