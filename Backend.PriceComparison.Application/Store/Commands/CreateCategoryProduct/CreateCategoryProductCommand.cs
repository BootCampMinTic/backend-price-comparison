using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;

public sealed record CreateCategoryProductCommand(string Description) : IRequest<Result<VoidResult, Error>>;
