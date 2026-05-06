namespace Backend.PriceComparison.Domain.ClientPos.Entities
{
    public class ClientEntity
    {
        public int Id { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ElectronicInvoiceEmail { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentTypeEntity? DocumentType { get; set; }

        public virtual bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(DocumentNumber)) return false;
            if (DocumentTypeId <= 0) return false;
            return true;
        }

        public void UpdateElectronicInvoiceEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ArgumentException("Invalid email format", nameof(email));
            
            ElectronicInvoiceEmail = email;
        }
    }
}
