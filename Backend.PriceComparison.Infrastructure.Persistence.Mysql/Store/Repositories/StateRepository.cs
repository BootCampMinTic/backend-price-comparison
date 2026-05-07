using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class StateRepository(ClientDbContext context) : IStateRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<StateEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.States.AsNoTracking().ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return StoreErrorBuilder.NoRecordsFound("state");

        return entities;
    }
}
