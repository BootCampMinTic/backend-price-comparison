using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Application.Client.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ElectronicInvoiceEmail { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentTypeEntity? DocumentType { get; set; }
        public string? CompanyName { get; set; }
        public int VerificationDigit { get; set; }
        public bool VATResponsibleParty { get; set; }
        public bool LargeTaxpayer { get; set; }
        public bool SelfRetainer { get; set; }
        public bool WithholdingAgent { get; set; }
        public bool SimpleTaxRegime { get; set; }
        public string? Name { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? SecondSurname { get; set; }
    }
}
