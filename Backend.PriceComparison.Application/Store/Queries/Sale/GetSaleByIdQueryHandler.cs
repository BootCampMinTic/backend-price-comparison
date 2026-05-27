using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Sale;

internal sealed class GetSaleByIdQueryHandler(
    ISaleRepository saleRepository,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<GetSaleByIdQuery, Result<SaleDto, Error>>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<SaleDto, Error>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.SaleById(request.Id);

        var cached = await _cacheService.GetAsync<SaleDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = _mapper.Map<SaleDto>(result.Value);
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);

        return dto;
    }
}
