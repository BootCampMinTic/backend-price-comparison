using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface IProductRepository
{
    Task<Result<IEnumerable<ProductEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Result<IEnumerable<ProductEntity>, Error>> GetByStoreAsync(int storeId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Result<ProductEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(ProductEntity entity, CancellationToken cancellationToken);
}
