using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using MediatR;

namespace Backend.PriceComparison.Application.Catalog.Commands.CreateCategoryStore;

public record CreateCategoryStoreCommand(
    string Description
) : IRequest<Result<VoidResult, Error>>;
