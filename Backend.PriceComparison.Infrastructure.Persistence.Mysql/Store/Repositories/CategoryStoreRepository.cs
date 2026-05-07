using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class CategoryStoreRepository(
    ClientDbContext context,
    ILogger<CategoryStoreRepository> logger) : ICategoryStoreRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<CategoryStoreEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.CategoryStores.AsNoTracking().ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return StoreErrorBuilder.NoRecordsFound("category store");

        return entities;
    }

    public async Task<Result<CategoryStoreEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoryStores.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("Category store not found by id {CategoryStoreId}", id);
            return StoreErrorBuilder.NotFound(id, "Category store");
        }

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(CategoryStoreEntity entity, CancellationToken cancellationToken)
    {
        await _context.CategoryStores.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist category store");
            return StoreErrorBuilder.CreationFailed("category store");
        }

        logger.LogInformation("Category store created with id {CategoryStoreId}", entity.Id);
        return VoidResult.Instance;
    }
}
