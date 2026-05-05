using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Client.Commands.CreateClientPos
{
    public record CreateClientNaturalPosCommand(
        string? Name,
        string? MiddleName,
        string? LastName,
        string? SecondSurname,
        string? DocumentNumber,
        string? ElectronicInvoiceEmail,
        int DocumentTypeId,
        string? DocumentCountry
        ) : IRequest<Result<VoidResult, Error>>;
}
