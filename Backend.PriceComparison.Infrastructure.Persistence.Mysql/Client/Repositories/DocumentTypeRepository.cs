using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Client.Repositories
{
    public class DocumentTypeRepository(ClientDbContext context) : IDocumentTypeRepository
    {
        private readonly ClientDbContext _context = context;

        public async Task<Result<IEnumerable<DocumentTypeEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.DocumentTypes.ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return ClientErrorBuilder.NoDocumentTypeRecordsFoundException();

            return entities;
        }
    }
}
