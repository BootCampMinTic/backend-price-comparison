using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using MediatR;

namespace Backend.PriceComparison.Application.Catalog.Commands.UpdateCategoryProduct;

public record UpdateCategoryProductCommand(
    int Id,
    string Description
) : IRequest<Result<VoidResult, Error>>;
