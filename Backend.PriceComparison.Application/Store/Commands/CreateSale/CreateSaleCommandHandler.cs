using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.CreateSale;

public sealed class CreateSaleCommandHandler(
    ISaleRepository _saleRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateSaleCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateSaleCommand request,
        CancellationToken cancellationToken)
    {
        var sale = _mapper.Map<SaleEntity>(request);
        foreach (var productId in request.ProductIds)
        {
            sale.ProductSales.Add(new ProductSaleEntity { ProductId = productId, Sale = sale });
        }

        var result = await _saleRepository.CreateAsync(sale, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync(CacheKeys.SalesPrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
