using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface ICategoryStoreRepository
{
    Task<Result<IEnumerable<CategoryStoreEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<CategoryStoreEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(CategoryStoreEntity entity, CancellationToken cancellationToken);
}
