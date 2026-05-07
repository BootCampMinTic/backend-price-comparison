using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Domain.Ports;

public interface IUserRepository
{
    Task<Result<IEnumerable<UserEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<UserEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> CreateAsync(UserEntity entity, CancellationToken cancellationToken);
}
