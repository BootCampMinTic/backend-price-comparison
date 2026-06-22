using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

public sealed record GetCategoryStoreByIdQuery(int Id)
    : IRequest<Result<CategoryStoreDto, Error>>;
