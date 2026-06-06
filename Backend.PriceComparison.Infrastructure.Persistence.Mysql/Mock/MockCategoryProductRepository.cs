using System.Collections.Concurrent;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;

internal sealed class MockCategoryProductRepository : ICategoryProductRepository
{
    private readonly ConcurrentDictionary<int, CategoryProductEntity> _store = new();
    private int _nextId = 1;

    public Task<Result<IEnumerable<CategoryProductEntity>, Error>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = _store.Values
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        if (entities.Count == 0)
            return Task.FromResult(Result<IEnumerable<CategoryProductEntity>, Error>.Failure(
                CategoryErrorBuilder.NoRecordsFound("category product")));

        return Task.FromResult(Result<IEnumerable<CategoryProductEntity>, Error>.Success(entities.AsEnumerable()));
    }

    public Task<Result<CategoryProductEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (_store.TryGetValue(id, out var entity))
            return Task.FromResult<Result<CategoryProductEntity, Error>>(entity);

        return Task.FromResult<Result<CategoryProductEntity, Error>>(
            CategoryErrorBuilder.NotFound(id, "CategoryProduct"));
    }

    public Task<Result<VoidResult, Error>> CreateAsync(CategoryProductEntity entity, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        entity.Id = id;
        _store[id] = entity;
        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }

    public Task<Result<VoidResult, Error>> UpdateAsync(CategoryProductEntity entity, CancellationToken cancellationToken)
    {
        if (!_store.ContainsKey(entity.Id))
            return Task.FromResult<Result<VoidResult, Error>>(
                CategoryErrorBuilder.NotFound(entity.Id, "CategoryProduct"));

        _store[entity.Id] = entity;
        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }

    public Task<Result<VoidResult, Error>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (!_store.TryRemove(id, out _))
            return Task.FromResult<Result<VoidResult, Error>>(
                CategoryErrorBuilder.NotFound(id, "CategoryProduct"));

        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }
}
