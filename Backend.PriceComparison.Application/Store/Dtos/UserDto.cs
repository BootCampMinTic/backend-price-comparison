namespace Backend.PriceComparison.Application.Store.Dtos;

public sealed class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TypeUserId { get; set; }
    public string? TypeUserDescription { get; set; }
}
