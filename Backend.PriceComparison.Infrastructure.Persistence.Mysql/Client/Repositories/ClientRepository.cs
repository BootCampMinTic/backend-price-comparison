using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Client.Repositories
{
    internal class ClientRepository(ClientDbContext context) : IClientRepository
    {
        public async Task<Result<VoidResult, Error>> CreateClientLegalAsync(ClientLegalPosEntity request, CancellationToken cancellationToken)
        {
            await context.ClientLegalPos.AddAsync(request, cancellationToken);
            var saved = await context.SaveChangesAsync(cancellationToken) > 0;
            if (!saved)
                return ClientErrorBuilder.ClientLegalCreationException();

            return VoidResult.Instance;
        }

        public async Task<Result<VoidResult, Error>> CreateClientNaturalAsync(ClientNaturalPosEntity request, CancellationToken cancellationToken)
        {
            await context.ClientNaturalPos.AddAsync(request, cancellationToken);
            var saved = await context.SaveChangesAsync(cancellationToken) > 0;
            if (!saved)
                return ClientErrorBuilder.ClientNaturalCreationException();

            return VoidResult.Instance;
        }

        public async Task<Result<IEnumerable<ClientLegalPosEntity>, Error>> GetAllLegalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var entities = await context.ClientLegalPos
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return ClientErrorBuilder.NoDocumentTypeRecordsFoundException();

            return entities;
        }

        public async Task<Result<IEnumerable<ClientNaturalPosEntity>, Error>> GetAllNaturalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var entities = await context.ClientNaturalPos
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return ClientErrorBuilder.NoDocumentTypeRecordsFoundException();

            return entities;
        }

        public async Task<Result<ClientEntity, Error>> GetByIdAsync(int id, ClientType type, CancellationToken cancellationToken)
        {
            ClientEntity? client = type switch
            {
                ClientType.Legal => await context.ClientLegalPos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken),
                ClientType.Natural => await context.ClientNaturalPos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken),
                _ => null
            };

            if (client is null)
                return ClientErrorBuilder.ClientNotFoundException(id);

            return client;
        }

        public async Task<Result<ClientEntity, Error>> GetByDocumentNumberAsync(string documentNumber, ClientType type, CancellationToken cancellationToken)
        {
            ClientEntity? client = type switch
            {
                ClientType.Legal => await context.ClientLegalPos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber, cancellationToken),
                ClientType.Natural => await context.ClientNaturalPos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber, cancellationToken),
                _ => null
            };

            if (client is null)
                return ClientErrorBuilder.ClientNotFoundException(documentNumber);

            return client;
        }
    }
}
