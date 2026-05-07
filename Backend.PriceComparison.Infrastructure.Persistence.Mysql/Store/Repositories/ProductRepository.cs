using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class ProductRepository(
    ClientDbContext context,
    ILogger<ProductRepository> logger) : IProductRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _context.Products
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.CategoryProduct)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No products found for page {PageNumber} size {PageSize}", pageNumber, pageSize);
            return StoreErrorBuilder.NoRecordsFound("product");
        }

        return entities;
    }

    public async Task<Result<IEnumerable<ProductEntity>, Error>> GetByStoreAsync(int storeId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _context.Products
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.CategoryProduct)
            .Where(p => p.StoreId == storeId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No products found for store {StoreId}", storeId);
            return StoreErrorBuilder.NoRecordsFound("product");
        }

        return entities;
    }

    public async Task<Result<ProductEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.CategoryProduct)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("Product not found by id {ProductId}", id);
            return StoreErrorBuilder.NotFound(id, "Product");
        }

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(ProductEntity entity, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist product {ProductName}", entity.Name);
            return StoreErrorBuilder.CreationFailed("product");
        }

        logger.LogInformation("Product created with id {ProductId}", entity.Id);
        return VoidResult.Instance;
    }
}
