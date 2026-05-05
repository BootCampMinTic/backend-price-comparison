namespace Backend.PriceComparison.Application.Client.Dtos;

public class DocumentTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? HelpTextHeader { get; set; }
    public string? HelpText { get; set; }
    public string? Regex { get; set; }
    public string? Fields { get; set; }
}
