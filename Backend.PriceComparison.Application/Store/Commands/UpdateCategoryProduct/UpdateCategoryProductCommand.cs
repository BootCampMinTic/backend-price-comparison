using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.UpdateCategoryProduct;

public sealed record UpdateCategoryProductCommand(int Id, string Description) : IRequest<Result<VoidResult, Error>>;
