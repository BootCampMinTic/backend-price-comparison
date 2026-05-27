using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface ICategoryProductRepository
{
    Task<Result<IEnumerable<CategoryProductEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<CategoryProductEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(CategoryProductEntity entity, CancellationToken cancellationToken);
}
