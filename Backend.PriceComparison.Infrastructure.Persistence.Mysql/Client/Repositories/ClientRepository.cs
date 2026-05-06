using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Client.Repositories
{
    internal sealed class ClientRepository(
        ClientDbContext context,
        ILogger<ClientRepository> logger) : IClientRepository
    {
        public async Task<Result<VoidResult, Error>> CreateClientLegalAsync(ClientLegalPosEntity request, CancellationToken cancellationToken)
        {
            await context.ClientLegalPos.AddAsync(request, cancellationToken);
            var saved = await context.SaveChangesAsync(cancellationToken) > 0;
            if (!saved)
            {
                logger.LogWarning("Failed to persist legal client with document {DocumentNumber}", request.DocumentNumber);
                return ClientErrorBuilder.ClientLegalCreationException();
            }

            logger.LogInformation("Legal client created with id {ClientId}", request.Id);
            return VoidResult.Instance;
        }

        public async Task<Result<VoidResult, Error>> CreateClientNaturalAsync(ClientNaturalPosEntity request, CancellationToken cancellationToken)
        {
            await context.ClientNaturalPos.AddAsync(request, cancellationToken);
            var saved = await context.SaveChangesAsync(cancellationToken) > 0;
            if (!saved)
            {
                logger.LogWarning("Failed to persist natural client with document {DocumentNumber}", request.DocumentNumber);
                return ClientErrorBuilder.ClientNaturalCreationException();
            }

            logger.LogInformation("Natural client created with id {ClientId}", request.Id);
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
            {
                logger.LogDebug("No legal clients found for page {PageNumber} size {PageSize}", pageNumber, pageSize);
                return ClientErrorBuilder.NoDocumentTypeRecordsFoundException();
            }

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
            {
                logger.LogDebug("No natural clients found for page {PageNumber} size {PageSize}", pageNumber, pageSize);
                return ClientErrorBuilder.NoDocumentTypeRecordsFoundException();
            }

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
            {
                logger.LogInformation("Client not found by id {ClientId} (type {ClientType})", id, type);
                return ClientErrorBuilder.ClientNotFoundException(id);
            }

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
            {
                logger.LogInformation("Client not found by document {DocumentNumber} (type {ClientType})", documentNumber, type);
                return ClientErrorBuilder.ClientNotFoundException(documentNumber);
            }

            return client;
        }
    }
}
