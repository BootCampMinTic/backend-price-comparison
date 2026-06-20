using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.RegisterPrice;

/// <summary>
/// Comando CQRS para registrar el precio de un producto en un supermercado en una fecha específica.
/// </summary>
public record RegisterPriceCommand(
    int ProductId,
    int StoreId,
    double Price,
    DateTime Date
) : IRequest<Result<VoidResult, Error>>;
