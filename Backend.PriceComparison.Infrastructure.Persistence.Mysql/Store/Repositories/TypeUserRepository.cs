using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class TypeUserRepository(ClientDbContext context) : ITypeUserRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<TypeUserEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.TypeUsers.AsNoTracking().ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return StoreErrorBuilder.NoRecordsFound("type user");

        return entities;
    }
}
