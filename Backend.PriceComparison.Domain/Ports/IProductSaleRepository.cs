using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface IProductSaleRepository
{
    Task<Result<IEnumerable<ProductSaleEntity>, Error>> GetBySaleIdAsync(int saleId, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(ProductSaleEntity entity, CancellationToken cancellationToken);
}
