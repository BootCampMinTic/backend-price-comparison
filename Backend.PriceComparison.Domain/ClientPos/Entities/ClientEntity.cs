namespace Backend.PriceComparison.Domain.ClientPos.Entities
{
    public class ClientEntity
    {
        public int Id { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ElectronicInvoiceEmail { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentTypeEntity? DocumentType { get; set; }
    }
}
