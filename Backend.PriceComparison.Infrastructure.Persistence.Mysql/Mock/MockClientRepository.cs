using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;

public class MockClientRepository : IClientRepository
{
    public Task<Result<VoidResult, Error>> CreateClientLegalAsync(ClientLegalPosEntity request, CancellationToken cancellationToken)
    {
        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }

    public Task<Result<VoidResult, Error>> CreateClientNaturalAsync(ClientNaturalPosEntity request, CancellationToken cancellationToken)
    {
        return Task.FromResult<Result<VoidResult, Error>>(VoidResult.Instance);
    }

    public Task<Result<IEnumerable<ClientLegalPosEntity>, Error>> GetAllLegalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var data = new List<ClientLegalPosEntity>
        {
            new() { Id = 1, CompanyName = "Empresa ABC", DocumentNumber = "9001234567", DocumentTypeId = 3, VerificationDigit = 1 },
            new() { Id = 2, CompanyName = "Empresa XYZ", DocumentNumber = "9009876543", DocumentTypeId = 3, VerificationDigit = 7 }
        };

        return Task.FromResult<Result<IEnumerable<ClientLegalPosEntity>, Error>>(data);
    }

    public Task<Result<IEnumerable<ClientNaturalPosEntity>, Error>> GetAllNaturalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var data = new List<ClientNaturalPosEntity>
        {
            new() { Id = 1, Name = "Juan", LastName = "Perez", DocumentNumber = "12345678", DocumentTypeId = 1 },
            new() { Id = 2, Name = "Maria", LastName = "Gomez", DocumentNumber = "87654321", DocumentTypeId = 1 }
        };

        return Task.FromResult<Result<IEnumerable<ClientNaturalPosEntity>, Error>>(data);
    }

    public Task<Result<ClientEntity, Error>> GetByIdAsync(int id, ClientType type, CancellationToken cancellationToken)
    {
        ClientEntity result = type == ClientType.Natural
            ? new ClientNaturalPosEntity { Id = id, Name = "Juan", LastName = "Perez", DocumentNumber = "12345678", DocumentTypeId = 1 }
            : new ClientLegalPosEntity { Id = id, CompanyName = "Empresa ABC", DocumentNumber = "9001234567", DocumentTypeId = 3, VerificationDigit = 1 };

        return Task.FromResult<Result<ClientEntity, Error>>(result);
    }

    public Task<Result<ClientEntity, Error>> GetByDocumentNumberAsync(string documentNumber, ClientType type, CancellationToken cancellationToken)
    {
        ClientEntity result = type == ClientType.Natural
            ? new ClientNaturalPosEntity { Id = 1, Name = "Juan", LastName = "Perez", DocumentNumber = documentNumber, DocumentTypeId = 1 }
            : new ClientLegalPosEntity { Id = 1, CompanyName = "Empresa ABC", DocumentNumber = documentNumber, DocumentTypeId = 3, VerificationDigit = 1 };

        return Task.FromResult<Result<ClientEntity, Error>>(result);
    }
}
