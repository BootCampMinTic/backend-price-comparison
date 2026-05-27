using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Application.Store.Commands.CreateSale;

internal sealed class CreateSaleCommandHandler(
    ISaleRepository saleRepository,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<CreateSaleCommand, Result<VoidResult, Error>>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<VoidResult, Error>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<SaleEntity>(request);

        var result = await _saleRepository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync(CacheKeys.SalesPrefix, cancellationToken);

        return VoidResult.Instance;
    }
}
