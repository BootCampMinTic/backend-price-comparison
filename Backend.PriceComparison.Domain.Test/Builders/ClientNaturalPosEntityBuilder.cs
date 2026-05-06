using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Domain.Test.Builders;

internal sealed class ClientNaturalPosEntityBuilder
{
    private int _id = 1;
    private string _name = "Ana";
    private readonly string? _middleName = null;
    private readonly string _lastName = "Lopez";
    private readonly string? _secondSurname = null;
    private string _documentNumber = "123";
    private readonly string _electronicInvoiceEmail = "ana@example.com";
    private int _documentTypeId = 1;
    private readonly string? _documentCountry = "CO";

    public ClientNaturalPosEntityBuilder WithId(int id) { _id = id; return this; }
    public ClientNaturalPosEntityBuilder WithName(string name) { _name = name; return this; }
    public ClientNaturalPosEntityBuilder WithDocumentNumber(string doc) { _documentNumber = doc; return this; }
    public ClientNaturalPosEntityBuilder WithDocumentTypeId(int id) { _documentTypeId = id; return this; }

    public ClientNaturalPosEntity Build() => new()
    {
        Id = _id,
        Name = _name,
        MiddleName = _middleName,
        LastName = _lastName,
        SecondSurname = _secondSurname,
        DocumentNumber = _documentNumber,
        ElectronicInvoiceEmail = _electronicInvoiceEmail,
        DocumentTypeId = _documentTypeId,
        DocumentCountry = _documentCountry
    };

    public static implicit operator ClientNaturalPosEntity(ClientNaturalPosEntityBuilder b) => b.Build();
}
