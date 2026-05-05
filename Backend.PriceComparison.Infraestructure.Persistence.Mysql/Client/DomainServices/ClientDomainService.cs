using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infraestructure.Persistence.Mysql.Client.DomainServices
{
    internal class ClientDomainService(ClientDbContext context) : IClientDomainService
    {
        public async Task<Result<VoidResult, Error>> CreateClientLegalAsync(ClientLegalPosEntity request, CancellationToken cancellationToken)
        {
            await context.ClientLegalPos.AddAsync(request, cancellationToken);
            var result = await context.SaveChangesAsync() > 0;
            if (!result)
                return ClienErrorBuilder.ClientLegalCreationException();

            return VoidResult.Instance;
        }

        public async Task<Result<VoidResult, Error>> CreateClientNaturalAsync(ClientNaturalPosEntity request, CancellationToken cancellationToken)
        {
            await context.ClientNaturalPos.AddAsync(request, cancellationToken);
            var result = await context.SaveChangesAsync() > 0;
            if (!result)
                return ClienErrorBuilder.ClientNaturalCreationException();

            return VoidResult.Instance;
        }

        public async Task<Result<IEnumerable<ClientLegalPosEntity>, Error>> GetAllLegalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var entities = await context.ClientLegalPos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return ClienErrorBuilder.NoDocumentTypeRecordsFoundException();

            return entities;
        }

        public async Task<Result<IEnumerable<ClientNaturalPosEntity>, Error>> GetAllNaturalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var entities = await context.ClientNaturalPos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return ClienErrorBuilder.NoDocumentTypeRecordsFoundException();

            return entities;
        }

        public async Task<Result<ClientEntity, Error>> GetByIdAsync(int id, ClientType type, CancellationToken cancellationToken)
        {
            ClientEntity client = new ClientEntity();

            if (type == ClientType.Legal && await LegalClientExists(id, cancellationToken))
            {
                client = await context.ClientLegalPos.FirstAsync(c => c.Id == id);
            }
            else if(type == ClientType.Natural && await NaturalClientExists(id, cancellationToken))
            {
                client = await context.ClientNaturalPos.FirstAsync(c => c.Id == id);
            }
            else
            {
                return ClienErrorBuilder.ClienNotFoundException(id);
            }

            return client;
        }

        public async Task<Result<ClientEntity, Error>> GetByDocumentNumberAsync(string documentNumber, ClientType type, CancellationToken cancellationToken)
        {
            ClientEntity client = new ClientEntity();

            if (type == ClientType.Legal && await LegalClientExists(documentNumber, cancellationToken))
            {
                client = await context.ClientLegalPos.FirstAsync(c => c.DocumentNumber == documentNumber);
            }
            else if (type == ClientType.Natural && await NaturalClientExists(documentNumber, cancellationToken))
            {
                client = await context.ClientNaturalPos.FirstAsync(c => c.DocumentNumber == documentNumber);
            }
            else
            {
                return ClienErrorBuilder.ClienNotFoundException(documentNumber);
            }

            return client;
        }

        private async Task<bool> LegalClientExists(int id, CancellationToken cancellationToken)
        {
            return await context.ClientLegalPos
                .AsNoTracking()
                .AnyAsync(c => c.Id == id, cancellationToken);
        }

        private async Task<bool> NaturalClientExists(int id, CancellationToken cancellationToken)
        {
            return await context.ClientNaturalPos
                .AsNoTracking()
                .AnyAsync(c => c.Id == id, cancellationToken);
        }

        private async Task<bool> LegalClientExists(string documentNumber, CancellationToken cancellationToken)
        {
            return await context.ClientLegalPos
                .AsNoTracking()
                .AnyAsync(c => c.DocumentNumber == documentNumber, cancellationToken);
        }

        private async Task<bool> NaturalClientExists(string documentNumber, CancellationToken cancellationToken)
        {
            return await context.ClientNaturalPos
                .AsNoTracking()
                .AnyAsync(c => c.DocumentNumber == documentNumber, cancellationToken);
        }   
    }
}
