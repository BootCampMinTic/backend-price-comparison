using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Client.Commands.CreateClientPos
{
    public record CreateClientLegalPosCommand(
        string? CompanyName,
        int? VerificationDigit,
        string? DocumentNumber,
        string? ElectronicInvoiceEmail,
        bool? VATResponsibleParty,
        bool? SelfRetainer,
        bool? WithholdingAgent,
        bool? SimpleTaxRegime,
        int DocumentTypeId,
        bool? LargeTaxpayer,
        string? DocumentCountry
    ) : IRequest<Result<VoidResult, Error>>;

}
