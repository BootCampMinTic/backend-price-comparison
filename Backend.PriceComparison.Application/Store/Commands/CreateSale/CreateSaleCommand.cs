using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateSale;

public record CreateSaleCommand(
    DateTime Date,
    double Total,
    int UserId,
    int StateId) : IRequest<Result<VoidResult, Error>>;
