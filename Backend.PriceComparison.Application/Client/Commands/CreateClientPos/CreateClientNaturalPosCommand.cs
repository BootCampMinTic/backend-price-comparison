using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Client.Commands.CreateClientPos
{
    /// <summary>
    /// Command to create a new natural person client in the billing system.
    /// </summary>
    public record CreateClientNaturalPosCommand(
        /// <summary>First name of the natural person.</summary>
        string? Name,
        /// <summary>Middle name (optional).</summary>
        string? MiddleName,
        /// <summary>First last name.</summary>
        string? LastName,
        /// <summary>Second last name (optional).</summary>
        string? SecondSurname,
        /// <summary>National identification document number.</summary>
        string? DocumentNumber,
        /// <summary>Email address for electronic invoice delivery.</summary>
        string? ElectronicInvoiceEmail,
        /// <summary>Foreign key to the document type catalog (e.g., CC, CE).</summary>
        int DocumentTypeId,
        /// <summary>ISO country code of the document (e.g., CO). Not persisted to DB.</summary>
        string? DocumentCountry
        ) : IRequest<Result<VoidResult, Error>>;
}
