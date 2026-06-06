using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

public sealed record GetAllCategoryStoresQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<Result<IEnumerable<CategoryStoreDto>, Error>>;