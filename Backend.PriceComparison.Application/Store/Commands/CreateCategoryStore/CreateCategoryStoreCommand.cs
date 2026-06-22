using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;

public sealed record CreateCategoryStoreCommand(string Description) : IRequest<Result<VoidResult, Error>>;
