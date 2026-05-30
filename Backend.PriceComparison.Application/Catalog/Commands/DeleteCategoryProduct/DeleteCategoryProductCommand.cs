using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using MediatR;

namespace Backend.PriceComparison.Application.Catalog.Commands.DeleteCategoryProduct;

public record DeleteCategoryProductCommand(
    int Id
) : IRequest<Result<VoidResult, Error>>;
