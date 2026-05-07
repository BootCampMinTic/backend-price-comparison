using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface ITypeUserRepository
{
    Task<Result<IEnumerable<TypeUserEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
}
