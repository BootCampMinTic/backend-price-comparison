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
    private readonly ILogger<CategoryStoreRepository> _logger = logger;

    public async Task<Result<IEnumerable<CategoryStoreEntity>, Error>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var entities = await _context.CategoryStores
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            return StoreErrorBuilder.NoRecordsFound("category store");
        }

        return entities;
    }

    public async Task<Result<CategoryStoreEntity, Error>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var entity = await _context.CategoryStores
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (entity is null)
        {
            _logger.LogDebug("Category store not found by id {CategoryId}", id);
            return StoreErrorBuilder.NotFound(id, "Category Store");
        }

        return entity;
    }

    public Task<Result<VoidResult, Error>> CreateAsync(
        CategoryStoreEntity entity,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<VoidResult, Error>> UpdateAsync(
        CategoryStoreEntity entity,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<VoidResult, Error>> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}