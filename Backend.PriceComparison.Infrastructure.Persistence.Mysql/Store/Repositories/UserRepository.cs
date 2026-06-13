using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Store.Repositories;

internal sealed class UserRepository(
    ClientDbContext context,
    ILogger<UserRepository> logger) : IUserRepository
{
    private readonly ClientDbContext _context = context;

    public async Task<Result<IEnumerable<UserEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _context.Users
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
        {
            logger.LogDebug("No users found for page {PageNumber} size {PageSize}", pageNumber, pageSize);
            return UserErrorBuilder.NoRecordsFound("user");
        }

        return entities;
    }

    public async Task<Result<UserEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (entity is null)
        {
            logger.LogDebug("User not found by id {UserId}", id);
            return UserErrorBuilder.NotFound(id, "User");
        }

        return entity;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(UserEntity entity, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(entity, cancellationToken);
        var saved = await _context.SaveChangesAsync(cancellationToken) > 0;

        if (!saved)
        {
            logger.LogWarning("Failed to persist user {UserName}", entity.Name);
            return UserErrorBuilder.CreationFailed("user");
        }

        logger.LogInformation("User created with id {UserId}", entity.Id);
        return VoidResult.Instance;
    }
}
