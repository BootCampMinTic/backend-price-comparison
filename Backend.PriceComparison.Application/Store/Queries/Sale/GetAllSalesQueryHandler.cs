using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Sale;

internal sealed class GetAllSalesQueryHandler(
    ISaleRepository saleRepository,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<GetAllSalesQuery, Result<IEnumerable<SaleDto>, Error>>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<IEnumerable<SaleDto>, Error>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.SalesPage(request.PageNumber, request.PageSize);

        var cached = await _cacheService.GetAsync<IEnumerable<SaleDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _saleRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = _mapper.Map<IEnumerable<SaleDto>>(result.Value);
        await _cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);

        return dtos.ToList();
    }
}
