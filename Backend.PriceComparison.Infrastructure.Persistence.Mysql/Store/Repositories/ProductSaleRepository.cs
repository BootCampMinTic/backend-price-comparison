using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class ProductSaleRepository(
    ClientDbContext context,
    ILogger<ProductSaleRepository> logger) : IProductSaleRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<ProductSaleEntity>, Error>> GetBySaleIdAsync(int saleId, CancellationToken cancellationToken)
    {
        var entities = await _context.ProductSales
            .AsNoTracking()
            .Include(ps => ps.Product)
            .Where(ps => ps.SaleId == saleId)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No product sales found for sale {SaleId}", saleId);
            return StoreErrorBuilder.NoRecordsFound("product sale");
        }

        return entities;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(ProductSaleEntity entity, CancellationToken cancellationToken)
    {
        await _context.ProductSales.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist product sale");
            return StoreErrorBuilder.CreationFailed("product sale");
        }

        logger.LogInformation("Product sale created with id {ProductSaleId}", entity.Id);
        return VoidResult.Instance;
    }
}
