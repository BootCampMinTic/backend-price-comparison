using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    double Price,
    int StoreId,
    int CategoryProductId
) : IRequest<Result<VoidResult, Error>>;
