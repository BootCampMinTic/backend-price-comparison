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
    private readonly ILogger<CategoryProductRepository> _logger = logger;

    public async Task<Result<IEnumerable<CategoryProductEntity>, Error>> GetAllAsync(
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
{
    var entities = await _context.CategoryProducts
        .AsNoTracking()
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    if (entities.Count == 0)
    {
        return StoreErrorBuilder.NoRecordsFound("category product");
    }

    return entities;
}
public async Task<Result<CategoryProductEntity, Error>> GetByIdAsync(
    int id,
    CancellationToken cancellationToken)
{
    var entity = await _context.CategoryProducts
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    if (entity is null)
    {
        _logger.LogDebug("Category product not found by id {CategoryId}", id);
        return StoreErrorBuilder.NotFound(id, "Category Product");
    }

    return entity;
}
public async Task<Result<VoidResult, Error>> CreateAsync(
    CategoryProductEntity entity,
    CancellationToken cancellationToken)
{
    await _context.CategoryProducts.AddAsync(entity, cancellationToken);

    var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

    if (!saved)
    {
        return StoreErrorBuilder.CreationFailed("category product");
    }

    return VoidResult.Instance;
}

public Task<Result<VoidResult, Error>> UpdateAsync(
    CategoryProductEntity entity,
    CancellationToken cancellationToken)
{
    throw new NotImplementedException();
}

public Task<Result<VoidResult, Error>> DeleteAsync(
    int id,
    CancellationToken cancellationToken)
{
    throw new NotImplementedException();
}}