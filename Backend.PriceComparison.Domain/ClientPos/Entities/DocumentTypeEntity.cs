namespace Backend.PriceComparison.Domain.ClientPos.Entities
{
    public class DocumentTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DocumentType { get; set; }
        public string? HelpTextHeader { get; set; }
        public string? HelpText { get; set; }
        public string? Regex { get; set; }
        public string? Fields { get; set; }
    }
}
 