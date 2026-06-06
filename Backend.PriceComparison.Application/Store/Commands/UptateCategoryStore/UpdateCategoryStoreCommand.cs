using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.UpdateCategoryStore;

public sealed record UpdateCategoryStoreCommand(int Id, string Description) : IRequest<Result<VoidResult, Error>>;