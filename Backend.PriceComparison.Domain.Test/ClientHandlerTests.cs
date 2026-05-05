using AutoMapper;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Application.Client.Mappers;
using Backend.PriceComparison.Application.Client.Queries.Client;
using Backend.PriceComparison.Application.Common.Interfaces;
using Backend.PriceComparison.Domain.ClientPos.DomainServices;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;
using System.Net;

namespace Backend.PriceComparison.Domain.Test;

public class ClientHandlerTests
{
    private readonly IMapper _mapper = new MapperConfiguration(config => config.AddProfile<ClientProfile>()).CreateMapper();

    [Fact]
    public async Task CreateClientNaturalPosHandle_WhenDomainSucceeds_RemovesNaturalClientCache()
    {
        var domain = new FakeClientDomainService();
        var cache = new FakeCacheService();
        var handler = new CreateClientNaturalPosHandle(domain, _mapper, cache);
        var command = new CreateClientNaturalPosCommand(
            "Ana",
            null,
            "Lopez",
            null,
            "123",
            "ana@example.com",
            1,
            "CO");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(domain.CreatedNaturalClient);
        Assert.Equal("123", domain.CreatedNaturalClient.DocumentNumber);
        Assert.Contains("clients:natural", cache.RemovedPrefixes);
    }

    [Fact]
    public async Task CreateClientLegalPosHandle_WhenDomainFails_DoesNotRemoveCache()
    {
        var domain = new FakeClientDomainService
        {
            CreateLegalResult = Error.CreateInstance("CreateError", "Creation failed.", HttpStatusCode.InternalServerError)
        };
        var cache = new FakeCacheService();
        var handler = new CreateClientLegalPosHandle(domain, _mapper, cache);
        var command = new CreateClientLegalPosCommand(
            "Empresa SAS",
            1,
            "900123",
            "billing@example.com",
            true,
            false,
            false,
            false,
            1,
            false,
            "CO");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CreateError", result.Error?.Code);
        Assert.Empty(cache.RemovedPrefixes);
    }

    [Fact]
    public async Task GetClientByIdQueryHandler_WhenCacheHit_ReturnsCachedClientWithoutCallingDomain()
    {
        var domain = new FakeClientDomainService();
        var cache = new FakeCacheService();
        var cached = new ClientDto { Id = 20, DocumentNumber = "cached" };
        await cache.SetAsync("client:Natural:20", cached);
        var handler = new GetClientByIdQueryHandler(domain, _mapper, cache);

        var result = await handler.Handle(
            new GetClientByIdQuery { Id = 20, Type = ClientType.Natural },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("cached", result.Value?.DocumentNumber);
        Assert.Equal(0, domain.GetByIdCalls);
    }

    [Fact]
    public async Task GetClientByIdQueryHandler_WhenCacheMiss_CallsDomainAndStoresCache()
    {
        var domain = new FakeClientDomainService
        {
            GetByIdResult = new ClientNaturalPosEntity
            {
                Id = 5,
                DocumentNumber = "123",
                Name = "Ana",
                DocumentTypeId = 1
            }
        };
        var cache = new FakeCacheService();
        var handler = new GetClientByIdQueryHandler(domain, _mapper, cache);

        var result = await handler.Handle(
            new GetClientByIdQuery { Id = 5, Type = ClientType.Natural },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("123", result.Value?.DocumentNumber);
        Assert.Equal(1, domain.GetByIdCalls);

        var cached = await cache.GetAsync<ClientDto>("client:Natural:5");
        Assert.NotNull(cached);
        Assert.Equal("123", cached.DocumentNumber);
    }

    [Fact]
    public async Task GetAllClientNaturalQueryHandler_WhenCacheMiss_ReturnsMappedClientsAndStoresCache()
    {
        var domain = new FakeClientDomainService
        {
            NaturalClients =
            [
                new ClientNaturalPosEntity { Id = 1, DocumentNumber = "100", Name = "Ana", DocumentTypeId = 1 },
                new ClientNaturalPosEntity { Id = 2, DocumentNumber = "200", Name = "Luis", DocumentTypeId = 1 }
            ]
        };
        var cache = new FakeCacheService();
        var handler = new GetAllClientNaturalQueryHandler(domain, cache, _mapper);

        var result = await handler.Handle(
            new GetAllClientNaturalQuery { PageNumber = 1, PageSize = 10 },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["100", "200"], result.Value?.Select(client => client.DocumentNumber));
        Assert.Equal(1, domain.GetAllNaturalCalls);

        var cached = await cache.GetAsync<IEnumerable<ClientDto>>("clients:natural:page:1:size:10");
        Assert.NotNull(cached);
        Assert.Equal(2, cached.Count());
    }

    private sealed class FakeClientDomainService : IClientDomainService
    {
        public Result<VoidResult, Error> CreateLegalResult { get; set; } = VoidResult.Instance;
        public Result<VoidResult, Error> CreateNaturalResult { get; set; } = VoidResult.Instance;
        public Result<ClientEntity, Error> GetByIdResult { get; set; } = new ClientNaturalPosEntity();
        public List<ClientNaturalPosEntity> NaturalClients { get; set; } = [];
        public ClientNaturalPosEntity? CreatedNaturalClient { get; private set; }
        public ClientLegalPosEntity? CreatedLegalClient { get; private set; }
        public int GetByIdCalls { get; private set; }
        public int GetAllNaturalCalls { get; private set; }

        public Task<Result<VoidResult, Error>> CreateClientLegalAsync(ClientLegalPosEntity request, CancellationToken cancellationToken)
        {
            CreatedLegalClient = request;
            return Task.FromResult(CreateLegalResult);
        }

        public Task<Result<VoidResult, Error>> CreateClientNaturalAsync(ClientNaturalPosEntity request, CancellationToken cancellationToken)
        {
            CreatedNaturalClient = request;
            return Task.FromResult(CreateNaturalResult);
        }

        public Task<Result<IEnumerable<ClientLegalPosEntity>, Error>> GetAllLegalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return Task.FromResult<Result<IEnumerable<ClientLegalPosEntity>, Error>>(Array.Empty<ClientLegalPosEntity>());
        }

        public Task<Result<IEnumerable<ClientNaturalPosEntity>, Error>> GetAllNaturalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            GetAllNaturalCalls++;
            return Task.FromResult<Result<IEnumerable<ClientNaturalPosEntity>, Error>>(NaturalClients);
        }

        public Task<Result<ClientEntity, Error>> GetByIdAsync(int id, ClientType type, CancellationToken cancellationToken)
        {
            GetByIdCalls++;
            return Task.FromResult(GetByIdResult);
        }

        public Task<Result<ClientEntity, Error>> GetByDocumentNumberAsync(string documentNumber, ClientType type, CancellationToken cancellationToken)
        {
            return Task.FromResult<Result<ClientEntity, Error>>(new ClientNaturalPosEntity { DocumentNumber = documentNumber });
        }
    }

    private sealed class FakeCacheService : ICacheService
    {
        private readonly Dictionary<string, object> _values = [];
        public List<string> RemovedPrefixes { get; } = [];

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_values.TryGetValue(key, out var value) ? (T)value : default);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            _values[key] = value!;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _values.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            RemovedPrefixes.Add(prefix);
            return Task.CompletedTask;
        }
    }
}
