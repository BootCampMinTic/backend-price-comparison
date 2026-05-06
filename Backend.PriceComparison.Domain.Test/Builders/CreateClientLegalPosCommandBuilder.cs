using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

namespace Backend.PriceComparison.Domain.Test.Builders;

internal sealed class CreateClientLegalPosCommandBuilder
{
    private string _companyName = "Empresa SAS";
    private int _verificationDigit = 1;
    private string _documentNumber = "900123";
    private string _electronicInvoiceEmail = "billing@example.com";
    private bool _vatResponsibleParty = false;
    private readonly bool _selfRetainer = false;
    private bool _withholdingAgent = false;
    private readonly bool _simpleTaxRegime = false;
    private int _documentTypeId = 1;
    private bool _largeTaxpayer = false;
    private readonly string? _documentCountry = "CO";

    public CreateClientLegalPosCommandBuilder WithCompanyName(string name) { _companyName = name; return this; }
    public CreateClientLegalPosCommandBuilder WithDocumentNumber(string doc) { _documentNumber = doc; return this; }
    public CreateClientLegalPosCommandBuilder WithEmail(string email) { _electronicInvoiceEmail = email; return this; }
    public CreateClientLegalPosCommandBuilder WithDocumentTypeId(int id) { _documentTypeId = id; return this; }
    public CreateClientLegalPosCommandBuilder WithVerificationDigit(int digit) { _verificationDigit = digit; return this; }
    public CreateClientLegalPosCommandBuilder WithVatResponsibleParty(bool value) { _vatResponsibleParty = value; return this; }
    public CreateClientLegalPosCommandBuilder WithWithholdingAgent(bool value) { _withholdingAgent = value; return this; }
    public CreateClientLegalPosCommandBuilder WithLargeTaxpayer(bool value) { _largeTaxpayer = value; return this; }

    public CreateClientLegalPosCommand Build() =>
        new(_companyName, _verificationDigit, _documentNumber, _electronicInvoiceEmail,
            _vatResponsibleParty, _selfRetainer, _withholdingAgent, _simpleTaxRegime,
            _documentTypeId, _largeTaxpayer, _documentCountry);

    public static implicit operator CreateClientLegalPosCommand(CreateClientLegalPosCommandBuilder b) => b.Build();
}
