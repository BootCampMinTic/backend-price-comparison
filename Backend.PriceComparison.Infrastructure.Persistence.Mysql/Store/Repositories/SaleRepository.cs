using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class SaleRepository(
    ClientDbContext context,
    ILogger<SaleRepository> logger) : ISaleRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<SaleEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _context.Sales
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Store)
            .Include(s => s.State)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No sales found for page {PageNumber} size {PageSize}", pageNumber, pageSize);
            return StoreErrorBuilder.NoRecordsFound("sale");
        }

        return entities;
    }

    public async Task<Result<SaleEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Sales
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Store)
            .Include(s => s.State)
            .Include(s => s.ProductSales)
                .ThenInclude(ps => ps.Product)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("Sale not found by id {SaleId}", id);
            return StoreErrorBuilder.NotFound(id, "Sale");
        }

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(SaleEntity entity, CancellationToken cancellationToken)
    {
        await _context.Sales.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist sale");
            return StoreErrorBuilder.CreationFailed("sale");
        }

        logger.LogInformation("Sale created with id {SaleId}", entity.Id);
        return VoidResult.Instance;
    }
}
