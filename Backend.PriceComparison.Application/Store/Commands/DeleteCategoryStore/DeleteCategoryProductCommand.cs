using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.DeleteCategoryStore;

public sealed record DeleteCategoryStoreCommand(int Id) : IRequest<Result<VoidResult, Error>>;