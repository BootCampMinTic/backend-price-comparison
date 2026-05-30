using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using MediatR;

namespace Backend.PriceComparison.Application.Catalog.Commands.UpdateCategoryStore;

public record UpdateCategoryStoreCommand(
    int Id,
    string Description
) : IRequest<Result<VoidResult, Error>>;
