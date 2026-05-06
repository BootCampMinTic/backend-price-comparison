using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

namespace Backend.PriceComparison.Domain.Test.Builders;

internal sealed class CreateClientNaturalPosCommandBuilder
{
    private string _name = "Ana";
    private string? _middleName = null;
    private string _lastName = "Lopez";
    private string? _secondSurname = null;
    private string _documentNumber = "123";
    private string _electronicInvoiceEmail = "ana@example.com";
    private int _documentTypeId = 1;
    private string? _documentCountry = "CO";

    public CreateClientNaturalPosCommandBuilder WithName(string name) { _name = name; return this; }
    public CreateClientNaturalPosCommandBuilder WithDocumentNumber(string doc) { _documentNumber = doc; return this; }
    public CreateClientNaturalPosCommandBuilder WithEmail(string email) { _electronicInvoiceEmail = email; return this; }
    public CreateClientNaturalPosCommandBuilder WithDocumentTypeId(int id) { _documentTypeId = id; return this; }
    public CreateClientNaturalPosCommandBuilder WithMiddleName(string? middleName) { _middleName = middleName; return this; }
    public CreateClientNaturalPosCommandBuilder WithLastName(string lastName) { _lastName = lastName; return this; }
    public CreateClientNaturalPosCommandBuilder WithSecondSurname(string? secondSurname) { _secondSurname = secondSurname; return this; }
    public CreateClientNaturalPosCommandBuilder WithDocumentCountry(string? country) { _documentCountry = country; return this; }

    public CreateClientNaturalPosCommand Build() =>
        new(_name, _middleName, _lastName, _secondSurname, _documentNumber, _electronicInvoiceEmail, _documentTypeId, _documentCountry);

    public static implicit operator CreateClientNaturalPosCommand(CreateClientNaturalPosCommandBuilder b) => b.Build();
}
