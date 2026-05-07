using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class CategoryProductRepository(
    ClientDbContext context,
    ILogger<CategoryProductRepository> logger) : ICategoryProductRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<CategoryProductEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.CategoryProducts.AsNoTracking().ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return StoreErrorBuilder.NoRecordsFound("category product");

        return entities;
    }

    public async Task<Result<CategoryProductEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.CategoryProducts.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("Category product not found by id {CategoryProductId}", id);
            return StoreErrorBuilder.NotFound(id, "Category product");
        }

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(CategoryProductEntity entity, CancellationToken cancellationToken)
    {
        await _context.CategoryProducts.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist category product");
            return StoreErrorBuilder.CreationFailed("category product");
        }

        logger.LogInformation("Category product created with id {CategoryProductId}", entity.Id);
        return VoidResult.Instance;
    }
}
