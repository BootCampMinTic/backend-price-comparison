using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using MediatR;

namespace Backend.PriceComparison.Application.Catalog.Commands.CreateCategoryProduct;

public record CreateCategoryProductCommand(
    string Description
) : IRequest<Result<VoidResult, Error>>;
