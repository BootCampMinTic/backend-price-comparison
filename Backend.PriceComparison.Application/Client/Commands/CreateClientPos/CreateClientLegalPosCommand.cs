using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Client.Commands.CreateClientPos
{
    /// <summary>
    /// Command to create a new legal entity client in the billing system.
    /// </summary>
    public record CreateClientLegalPosCommand(
        /// <summary>Registered company name.</summary>
        string? CompanyName,
        /// <summary>Verification digit of the tax ID (NIT).</summary>
        int? VerificationDigit,
        /// <summary>Tax identification number (NIT) without verification digit.</summary>
        string? DocumentNumber,
        /// <summary>Email address for electronic invoice delivery.</summary>
        string? ElectronicInvoiceEmail,
        /// <summary>Whether the entity is a VAT responsible party.</summary>
        bool? VATResponsibleParty,
        /// <summary>Whether the entity acts as a self-retainer.</summary>
        bool? SelfRetainer,
        /// <summary>Whether the entity is a withholding agent.</summary>
        bool? WithholdingAgent,
        /// <summary>Whether the entity is under the simple tax regime.</summary>
        bool? SimpleTaxRegime,
        /// <summary>Foreign key to the document type catalog (e.g., NIT).</summary>
        int DocumentTypeId,
        /// <summary>Whether the entity is classified as a large taxpayer.</summary>
        bool? LargeTaxpayer,
        /// <summary>ISO country code of the document (e.g., CO). Not persisted to DB.</summary>
        string? DocumentCountry
    ) : IRequest<Result<VoidResult, Error>>;

}
