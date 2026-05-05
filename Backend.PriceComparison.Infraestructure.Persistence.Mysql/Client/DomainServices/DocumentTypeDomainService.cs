using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Errors.Client;
using Backend.PriceComparison.Domain.ClientPos.DomainServices;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infraestructure.Persistence.Mysql.Client.DomainServices
{
    public class DocumentTypeDomainService: IDocumentTypeDomainService
    {
        private readonly ClientDbContext _context;

        public DocumentTypeDomainService(ClientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<DocumentTypeEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.DocumentTypes.ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return ClienErrorBuilder.NoDocumentTypeRecordsFoundException();

            return entities;
        }
    }
}
