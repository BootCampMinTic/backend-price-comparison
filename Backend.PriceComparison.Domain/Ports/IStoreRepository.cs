using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface IStoreRepository
{
    Task<Result<IEnumerable<StoreEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Result<StoreEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(StoreEntity entity, CancellationToken cancellationToken);
}
