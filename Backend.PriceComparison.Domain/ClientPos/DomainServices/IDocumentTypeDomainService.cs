using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Domain.ClientPos.DomainServices
{
    public interface IDocumentTypeDomainService
    {
        Task<Result<IEnumerable<DocumentTypeEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
    }
}
