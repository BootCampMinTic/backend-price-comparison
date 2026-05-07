using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class StoreRepository(
    ClientDbContext context,
    ILogger<StoreRepository> logger) : IStoreRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<StoreEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _context.Stores
            .AsNoTracking()
            .Include(s => s.CategoryStore)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No stores found for page {PageNumber} size {PageSize}", pageNumber, pageSize);
            return StoreErrorBuilder.NoRecordsFound("store");
        }

        return entities;
    }

    public async Task<Result<StoreEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Stores
            .AsNoTracking()
            .Include(s => s.CategoryStore)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("Store not found by id {StoreId}", id);
            return StoreErrorBuilder.NotFound(id, "Store");
        }

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(StoreEntity entity, CancellationToken cancellationToken)
    {
        await _context.Stores.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist store {StoreName}", entity.Name);
            return StoreErrorBuilder.CreationFailed("store");
        }

        logger.LogInformation("Store created with id {StoreId}", entity.Id);
        return VoidResult.Instance;
    }
}
