using System.Text.Json;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Backend.PriceComparison.Api.Tests;

public static class WireMockStubs
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static void SetupDefaultClientStubs(WireMockServer server)
    {
        var naturalClients = new List<ClientNaturalPosEntity>
        {
            new() { Id = 1, Name = "Juan", LastName = "Perez", DocumentNumber = "12345678", DocumentTypeId = 1 },
            new() { Id = 2, Name = "Maria", LastName = "Gomez", DocumentNumber = "87654321", DocumentTypeId = 1 }
        };

        var legalClients = new List<ClientLegalPosEntity>
        {
            new() { Id = 1, CompanyName = "Empresa ABC", DocumentNumber = "9001234567", DocumentTypeId = 3, VerificationDigit = 1 },
            new() { Id = 2, CompanyName = "Empresa XYZ", DocumentNumber = "9009876543", DocumentTypeId = 3, VerificationDigit = 7 }
        };

        server
            .Given(Request.Create().WithPath("/clients/natural").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(naturalClients, JsonOptions)));

        server
            .Given(Request.Create().WithPath("/clients/legal").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(legalClients, JsonOptions)));

        server
            .Given(Request.Create().WithPath("/clients/natural/*").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new ClientNaturalPosEntity
                {
                    Id = 1, Name = "Juan", LastName = "Perez", DocumentNumber = "12345678", DocumentTypeId = 1
                }, JsonOptions)));

        server
            .Given(Request.Create().WithPath("/clients/legal/*").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new ClientLegalPosEntity
                {
                    Id = 1, CompanyName = "Empresa ABC", DocumentNumber = "9001234567", DocumentTypeId = 3, VerificationDigit = 1
                }, JsonOptions)));

        server
            .Given(Request.Create().WithPath("/clients/natural/document/*").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new ClientNaturalPosEntity
                {
                    Id = 1, Name = "Juan", LastName = "Perez", DocumentNumber = "12345678", DocumentTypeId = 1
                }, JsonOptions)));

        server
            .Given(Request.Create().WithPath("/clients/legal/document/*").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new ClientLegalPosEntity
                {
                    Id = 1, CompanyName = "Empresa ABC", DocumentNumber = "9001234567", DocumentTypeId = 3, VerificationDigit = 1
                }, JsonOptions)));

        server
            .Given(Request.Create().WithPath("/clients/natural").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new ClientNaturalPosEntity
                {
                    Id = 99, Name = "Created", DocumentNumber = "99999999", DocumentTypeId = 1
                }, JsonOptions)));

        server
            .Given(Request.Create().WithPath("/clients/legal").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new ClientLegalPosEntity
                {
                    Id = 99, CompanyName = "Created", DocumentNumber = "800555123", DocumentTypeId = 3
                }, JsonOptions)));
    }

    public static void SetupDefaultDocumentTypeStubs(WireMockServer server)
    {
        var documentTypes = new List<DocumentTypeEntity>
        {
            new() { Id = 1, Name = "Cedula de ciudadania", DocumentType = "CC", HelpTextHeader = "Ingrese su cedula", HelpText = "Numero de identificacion", Regex = "^[0-9]{6,10}$", Fields = "numero" },
            new() { Id = 2, Name = "Cedula de extranjeria", DocumentType = "CE", HelpTextHeader = "Ingrese su cedula de extranjeria", HelpText = "Numero de identificacion extranjera", Regex = "^[0-9]{6,10}$", Fields = "numero" },
            new() { Id = 3, Name = "NIT", DocumentType = "NIT", HelpTextHeader = "Ingrese el NIT", HelpText = "Numero de identificacion tributaria", Regex = "^[0-9]{9,10}$", Fields = "numero,dv" }
        };

        server
            .Given(Request.Create().WithPath("/document-types").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(documentTypes, JsonOptions)));
    }
}
