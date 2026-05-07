using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Sale;

public sealed class GetSaleByIdQueryHandler(
    ISaleRepository _saleRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetSaleByIdQuery, Result<SaleDto, Error>>
{
    public async Task<Result<SaleDto, Error>> Handle(
        GetSaleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.SaleById(request.Id);
        var cached = await _cacheService.GetAsync<SaleDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = _mapper.Map<SaleDto>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
