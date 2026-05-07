using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface ISaleRepository
{
    Task<Result<IEnumerable<SaleEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Result<SaleEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(SaleEntity entity, CancellationToken cancellationToken);
}
