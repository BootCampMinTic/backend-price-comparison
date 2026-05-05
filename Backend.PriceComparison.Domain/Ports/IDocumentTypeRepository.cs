using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Domain.Ports
{
    /// <summary>
    /// Persistence port for the document type catalog (CC, CE, NIT, ...).
    /// </summary>
    public interface IDocumentTypeRepository
    {
        /// <summary>Returns every document type registered in the catalog.</summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The full collection of document types or an <see cref="Error"/> when the catalog is empty.</returns>
        Task<Result<IEnumerable<DocumentTypeEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
    }
}
