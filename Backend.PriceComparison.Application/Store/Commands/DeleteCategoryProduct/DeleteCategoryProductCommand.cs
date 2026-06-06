using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.DeleteCategoryProduct;

public sealed record DeleteCategoryProductCommand(int Id) : IRequest<Result<VoidResult, Error>>;
