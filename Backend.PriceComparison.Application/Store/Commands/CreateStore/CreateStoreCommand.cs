using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateStore;

public record CreateStoreCommand(
    string Name,
    string Address,
    string? Phone,
    int CategoryStoreId
) : IRequest<Result<VoidResult, Error>>;
