using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using MediatR;

namespace Backend.PriceComparison.Application.Catalog.Commands.DeleteCategoryStore;

public record DeleteCategoryStoreCommand(
    int Id
) : IRequest<Result<VoidResult, Error>>;
