using System.Collections.Concurrent;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;

internal sealed class MockCategoryStoreRepository : ICategoryStoreRepository
{
    private readonly ConcurrentDictionary<int, CategoryStoreEntity> _store = new();
    private int _nextId = 1;

    public Task<Result<IEnumerable<CategoryStoreEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = _store.Values
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        if (entities.Count == 0)
            return Task.FromResult(Result<IEnumerable<CategoryStoreEntity>, Error>.Failure(
                CategoryErrorBuilder.NoRecordsFound("category store")));

        return Task.FromResult(Result<IEnumerable<CategoryStoreEntity>, Error>.Success(entities.AsEnumerable()));
    }

    public Task<Result<CategoryStoreEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (_store.TryGetValue(id, out var entity))
            return Task.FromResult<Result<CategoryStoreEntity, Error>>(entity);

        return Task.FromResult<Result<CategoryStoreEntity, Error>>(
            CategoryErrorBuilder.NotFound(id, "CategoryStore"));
    }

    public Task<Result<VoidResult, Error>> CreateAsync(CategoryStoreEntity entity, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        entity.Id = id;
        _store[id] = entity;
        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }

    public Task<Result<VoidResult, Error>> UpdateAsync(CategoryStoreEntity entity, CancellationToken cancellationToken)
    {
        if (!_store.ContainsKey(entity.Id))
            return Task.FromResult<Result<VoidResult, Error>>(
                CategoryErrorBuilder.NotFound(entity.Id, "CategoryStore"));

        _store[entity.Id] = entity;
        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }

    public Task<Result<VoidResult, Error>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (!_store.TryRemove(id, out _))
            return Task.FromResult<Result<VoidResult, Error>>(
                CategoryErrorBuilder.NotFound(id, "CategoryStore"));

        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }
}
