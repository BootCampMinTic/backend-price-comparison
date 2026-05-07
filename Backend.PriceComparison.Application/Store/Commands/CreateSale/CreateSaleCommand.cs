using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateSale;

public record CreateSaleCommand(
    int UserId,
    int StoreId,
    int StateId,
    DateTime Date,
    List<int> ProductIds
) : IRequest<Result<VoidResult, Error>>;
